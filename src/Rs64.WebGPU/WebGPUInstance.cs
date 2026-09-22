// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Rs64.WebGPU;

public class WebGPUInstance : IDisposable
{
    private WGPUObjectHolder<FFI.WGPUInstance> Native { get; }
    internal WebGPUInstance(WGPUObjectHolder<FFI.WGPUInstance> instanceHolder) { Native = instanceHolder; }
    public void Dispose() { Native.Dispose(); }



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
    internal void InstanceProcessEvents()
    {
        unsafe { FFI.WGPUInstance.wgpuInstanceProcessEvents(Native.GetPtr()); }
    }

    public Task<WebGPUAdapter> RequestAdapter(WebGPURequestAdapterOptions? requestAdapterOptions = null)
    {
        var callBack = new RequestAdapterCallBack(new TaskCompletionSource<WebGPUAdapter>());
        unsafe
        {
            var ffiCallBack = new FFI.WGPURequestAdapterCallbackInfo
            {
                CallBackMode = FFI.WGPUCallbackMode.AllowSpontaneous,
                WGPURequestAdapterCallback = &FFI.WGPURequestAdapterCallbackManagedWrapper.CallBack,
                UserData1 = FFI.WGPURequestAdapterCallbackManagedWrapper.CreateUserData(callBack)
            };
            FFI.WGPUInstance.wgpuInstanceRequestAdapter(Native.GetPtr(), null, ffiCallBack);
        }
        return callBack.Task.Task;
    }
    class RequestAdapterCallBack(TaskCompletionSource<WebGPUAdapter> task) : FFI.IWGPURequestAdapterCallback
    {
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
                        Task.SetResult(new(new(adapter)));
                        return;
                    }
                case FFI.WGPURequestAdapterStatus.Error:
                    {
                        Task.SetException(new RequestAdapterErrorException(message.ReadStringView() ?? "message not found"));
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

    public void CreateSurface(WebGPUSurfaceDescriptor webGPUSurfaceDescriptor)
    {
        unsafe
        {
            var stack = new StackAllocAsFFIArea(stackalloc byte[256]);
            FFI.WGPUSurfaceDescriptor surfaceDescriptor = new();

            fixed (byte* ptr = FFI.WGPUStringView.ConvertWGPUStringParts(webGPUSurfaceDescriptor.Label, out var strLen))
            {
                surfaceDescriptor.Label = new(ptr, strLen);
                surfaceDescriptor.NextInChain = (FFI.WGPUChainedStruct*)Unsafe.AsPointer(ref webGPUSurfaceDescriptor.GetExtensionSurfaceSource(stack));

                var surface = FFI.WGPUInstance.wgpuInstanceCreateSurface(Native.GetPtr(), &surfaceDescriptor);
            }
        }
    }

    internal void InstanceWaitAny()
    {
        unsafe
        {
            nuint length = 0;
            var features = stackalloc FFI.WGPUFutureWaitInfo[(int)length];
            var timeout_nanosecond = 0ul;
            FFI.WGPUInstance.wgpuInstanceWaitAny(Native.GetPtr(), length, features, timeout_nanosecond);
        }

    }



}

public class WebGPURequestAdapterOptions
{
}

public class WebGpuInstanceDescriptor
{
    public HashSet<WebGPUInstanceFeatureName> RequiredFeatures = [];
    public WebGPUInstanceLimits RequiredLimit = new();
    internal FFI.WGPUInstanceFeatureName[] ConvertToFFIRequiredFeature() { return ConvertToFFIRequiredFeature(RequiredFeatures); }
    internal static FFI.WGPUInstanceFeatureName[] ConvertToFFIRequiredFeature(HashSet<WebGPUInstanceFeatureName> featureNames)
    {
        return featureNames.Select(WebGPUInstanceFeatureNameUtil.ToF).ToArray();
    }
}
public class WebGPUInstanceLimits
{
    public nuint TimedWaitAnyMaxCount = 0;

    internal FFI.WGPUInstanceLimits CreateLimit()
    {
        var limit = new FFI.WGPUInstanceLimits();
        limit.TimedWaitAnyMaxCount = TimedWaitAnyMaxCount;
        return limit;
    }
}
