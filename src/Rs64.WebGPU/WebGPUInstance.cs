// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Rs64.WebGPU;

public class WebGPUInstance : IDisposable
{
    private WGPUObjectHolder<FFI.WGPUInstance> Native { get; }
    internal WebGPUInstance(WGPUObjectHolder<FFI.WGPUInstance> instanceHolder) { Native = instanceHolder; }
    public void Dispose() { Native.Dispose(); }



    public Task<WebGPUAdapter> RequestAdapter()
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

    internal void InstanceProcessEvents()
    {
        unsafe { FFI.WGPUInstance.wgpuInstanceProcessEvents(Native.GetPtr()); }
    }


    internal void CreateSurface(WebGPUSurfaceDescriptor webGPUSurfaceDescriptor)
    {
        unsafe
        {
            var stack = new StackAllocAsFFIArea(stackalloc byte[256]);
            FFI.WGPUSurfaceDescriptor surfaceDescriptor = new();

            var labelSpan = FFI.WGPUStringView.ConvertSpan(webGPUSurfaceDescriptor.Label);
            fixed (byte* ptr = labelSpan)
            {
                surfaceDescriptor.Label = new(ptr, labelSpan.Length);
                surfaceDescriptor.NextInChain = (FFI.WGPUChainedStruct*)Unsafe.AsPointer(ref webGPUSurfaceDescriptor.GetExtensionSurfaceSource(stack));

                var surface = FFI.WGPUInstance.wgpuInstanceCreateSurface(Native.GetPtr(), &surfaceDescriptor);
            }
        }
    }




}

public class WebGpuInstanceDescriptor
{
    public Feature RequiredFeature = new();
    public class Feature
    {
        public bool TimedWaitAny = false;

        public bool ShaderSourceSpirv = false;

        public bool MultipleDevicesPerAdapter = false;


        internal FFI.WGPUInstanceFeatureName[] CreateFeatureNameArray()
        {
            var enableFutureCount = new bool[]
            {
                TimedWaitAny,
                ShaderSourceSpirv,
                MultipleDevicesPerAdapter,
            }.Count(d => d);
            var futuresArray = new FFI.WGPUInstanceFeatureName[enableFutureCount];

            var i = 0;
            if (TimedWaitAny)
            {
                futuresArray[i] = FFI.WGPUInstanceFeatureName.TimedWaitAny;
                i += 1;
            }

            if (ShaderSourceSpirv)
            {
                futuresArray[i] = FFI.WGPUInstanceFeatureName.ShaderSourceSpirv;
                i += 1;
            }

            if (MultipleDevicesPerAdapter)
            {
                futuresArray[i] = FFI.WGPUInstanceFeatureName.MultipleDevicesPerAdapter;
                i += 1;
            }
            return futuresArray;
        }
    }
    public Limit RequiredLimit = new();
    public class Limit
    {
        public nuint TimedWaitAnyMaxCount = 0;

        internal FFI.WGPUInstanceLimits CreateLimit()
        {
            var limit = new FFI.WGPUInstanceLimits();
            limit.TimedWaitAnyMaxCount = TimedWaitAnyMaxCount;
            return limit;
        }
    }


}
