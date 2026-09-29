// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Rs64.WebGPU;

[FFINote(typeof(FFI.WGPUInstance))]
public class WebGPUInstance : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPUInstance> Native { get; }
    internal WebGPUInstance(WGPUObjectHolder<FFI.WGPUInstance> holder)
    {
        Native = holder;

        var futureChannel = Channel.CreateUnbounded<WebGPUFuture>();
        FutureChannel = futureChannel;
        var futureChannelReader = futureChannel.Reader;

        CancellationTokenSource = new();
        var cancelToken = CancellationTokenSource.Token;
        FutureWaiter = Task.Run(async () =>
        {
            // よくわかってないが Dawn 実装で最初すごい勢いでやると死ぬっぽいので安全のための 5ms
            // 下1行 無しで 1 ms loop にすると死ぬことがわかっている
            await Task.Delay(5, cancelToken);

            var waitList = new List<WebGPUFuture>();
            while (cancelToken.IsCancellationRequested is false)
            {
                if (futureChannelReader.TryRead(out var newFuture))
                {
                    waitList.Add(newFuture);
                    continue;
                }

                // Console.WriteLine("wait any 0 !");
                // DoWaitAny(waitList);
                // Console.WriteLine("wait any exit !");

                // Console.WriteLine("process events !");
                ProcessEvents();
                // Console.WriteLine("process exit !");

                await Task.Delay(5, cancelToken);
                if (cancelToken.IsCancellationRequested) { break; }
            }
            Console.WriteLine("exit process events loop");
        }, cancelToken);
        FutureWaiter.ContinueWith(t => { if (t.IsFaulted) { Console.WriteLine(t.Exception); } });

        // wait any が wgpu で使えません ... ぬん
#pragma warning disable CS8321 // Local function is declared but never used
        void DoWaitAny(List<WebGPUFuture> waitList)
        {
            Span<WebGPUFuture> futures = stackalloc WebGPUFuture[waitList.Count];
            Span<bool> completes = stackalloc bool[waitList.Count];
            for (var i = 0; futures.Length > i; i += 1) { futures[i] = waitList[i]; }

            var result = InstanceWaitAny(futures, completes, 0);

            if (result is FFI.WGPUWaitStatus.Error) { Console.WriteLine(result); }
            else if (result is FFI.WGPUWaitStatus.TimedOut) { }
            else if (result is FFI.WGPUWaitStatus.Success)
            {
                waitList.Clear();
                for (var i = 0; futures.Length > i; i += 1)
                {
                    if (completes[i] is false) { continue; }
                    waitList.Add(futures[i]);
                }
            }
        }
#pragma warning restore CS8321 // Local function is declared but never used
    }


    public void Dispose()
    {
        CancellationTokenSource.Cancel();
        Native.Dispose();

    }

    internal ChannelWriter<WebGPUFuture> FutureChannel;
    private Task FutureWaiter;
    private CancellationTokenSource CancellationTokenSource;

    public HashSet<WebGPUWgslLanguageFeatureName> GetWgslLanguageFeatures()
    {
        unsafe
        {
            FFI.WGPUSupportedWgslLanguageFeatures lf = default;
            FFI.WGPUInstance.wgpuInstanceGetWgslLanguageFeatures(Native.GetPtr(), &lf);

            var lnfSpan = new Span<FFI.WGPUWgslLanguageFeatureName>(lf.Features, (int)lf.FeaturesCount);
            var wgslLanguageFeatureNames = lnfSpan.ToArray().Select(WebGPUWgslLanguageFeatureNameUtil.ToW).ToHashSet();

            FFI.WGPUSupportedWgslLanguageFeatures.FreeMembers(ref lf);

            return wgslLanguageFeatureNames;
        }
    }
    public bool HasWgslLanguageFeature(WebGPUWgslLanguageFeatureName languageFeatureName)
    {
        unsafe
        {
            return (bool)FFI.WGPUInstance.wgpuInstanceHasWgslLanguageFeature(Native.GetPtr(), languageFeatureName.ToF());
        }
    }
    public bool HasAllWgslLanguageFeature(HashSet<WebGPUWgslLanguageFeatureName> languageFeatureNames)
    {
        return languageFeatureNames.All(HasWgslLanguageFeature);
    }
    internal void ProcessEvents()
    {
        unsafe { FFI.WGPUInstance.wgpuInstanceProcessEvents(Native.GetPtr()); }
    }

    public Task<WebGPUAdapter> RequestAdapter(WebGPURequestAdapterOptions? requestAdapterOptions = null)
    {
        var callBack = new RequestAdapterCallBack(FutureChannel, new TaskCompletionSource<WebGPUAdapter>(TaskCreationOptions.RunContinuationsAsynchronously));
        unsafe
        {
            var ffiCallBack = new FFI.WGPURequestAdapterCallbackInfo
            {
                CallBackMode = FFI.WGPUCallbackMode.AllowSpontaneous,
                WGPURequestAdapterCallback = &FFI.WGPURequestAdapterCallbackManagedWrapper.CallBack,
                UserData1 = FFI.WGPURequestAdapterCallbackManagedWrapper.CreateUserData(callBack)
            };
            FFI.WGPURequestAdapterOptions* adapterOptionPtr;
            FFI.WGPURequestAdapterOptions adapterOption;
            if (requestAdapterOptions is null) { adapterOptionPtr = null; }
            else
            {
                adapterOptionPtr = &adapterOption;

                adapterOption.FeatureLevel = requestAdapterOptions.FeatureLevel.ToF();
                adapterOption.PowerPreference = requestAdapterOptions.PowerPreference.ToF();
                adapterOption.ForceFallbackAdapter = (FFI.WGPUBool)requestAdapterOptions.ForceFallbackAdapter;
                adapterOption.BackendType = requestAdapterOptions.BackendType.ToF();
                adapterOption.CompatibleSurface = requestAdapterOptions.CompatibleSurface is not null ? requestAdapterOptions.CompatibleSurface.Native.GetPtr() : null;
            }

            var future = FFI.WGPUInstance.wgpuInstanceRequestAdapter(Native.GetPtr(), adapterOptionPtr, ffiCallBack);
            if (FutureChannel.TryWrite(new(future)) is false) { Console.WriteLine(" failed : future send to manager"); }
        }
        return callBack.Task.Task;
    }
    class RequestAdapterCallBack(ChannelWriter<WebGPUFuture> futureChannel, TaskCompletionSource<WebGPUAdapter> task) : FFI.IWGPURequestAdapterCallback
    {
        public ChannelWriter<WebGPUFuture> FutureChannel { get; } = futureChannel;
        public TaskCompletionSource<WebGPUAdapter> Task { get; } = task;

        public unsafe void CallBack(
            FFI.WGPURequestAdapterStatus status,
            [FFI.WebGPUPassedWithOwnership(true)] FFI.WGPUAdapter* adapter,
            [FFI.WebGPUOutString, FFI.WebGPUPassedWithOwnership(false)] FFI.WGPUStringView message
        )
        {
            switch (status)
            {
                case FFI.WGPURequestAdapterStatus.Success:
                    {
                        Task.SetResult(new(new(adapter), FutureChannel));
                        return;
                    }
                case FFI.WGPURequestAdapterStatus.Error:
                    {
                        Task.SetException(new CallBackErrorException(message.ReadStringView() ?? "message not found"));
                        return;
                    }
                case FFI.WGPURequestAdapterStatus.Unavailable:
                    {
                        Task.SetException(new RequestAdapterUnavailableException(message.ReadStringView() ?? "message not found"));
                        return;
                    }
                case FFI.WGPURequestAdapterStatus.CallbackCancelled:
                    {
                        Task.SetCanceled();
                        return;
                    }
            }
            throw new InvalidCallBackStatusException(message.ReadStringView() ?? "message not found");
        }
    }

    public WebGPUSurface CreateSurface(WebGPUSurfaceDescriptor webGPUSurfaceDescriptor)
    {
        unsafe
        {
            var ffiMem = new FFIStackMemory(stackalloc nint[16]); using var s = FFIStackMemory.BindScope(ref ffiMem);
            FFI.WGPUSurfaceDescriptor surfaceDescriptor = new();

            fixed (byte* ptr = FFI.WGPUStringView.ConvertWGPUStringParts(webGPUSurfaceDescriptor.Label, out var strLen))
            {
                surfaceDescriptor.Label = new(ptr, strLen);
                surfaceDescriptor.NextInChain = webGPUSurfaceDescriptor.GetExtensionSurfaceSource(ffiMem);

                return new(new(FFI.WGPUInstance.wgpuInstanceCreateSurface(Native.GetPtr(), &surfaceDescriptor)));
            }
        }
    }

    internal FFI.WGPUWaitStatus InstanceWaitAny(ReadOnlySpan<WebGPUFuture> future, Span<bool> completedOut, ulong timeout_NS)
    {
        unsafe
        {
            var ffiFutures = stackalloc FFI.WGPUFutureWaitInfo[future.Length];
            for (var i = 0; future.Length > i; i += 1)
            {
                ffiFutures[i].Future.Id = future[i].FutureID;
            }
            var result = FFI.WGPUInstance.wgpuInstanceWaitAny(Native.GetPtr(), (nuint)future.Length, ffiFutures, timeout_NS);

            for (var i = 0; future.Length > i; i += 1)
            {
                completedOut[i] = (bool)ffiFutures[i].Completed;
            }
            return result;
        }

    }

}
