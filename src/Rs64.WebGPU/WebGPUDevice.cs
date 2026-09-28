// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Rs64.WebGPU;

[FFINote(typeof(FFI.WGPUDevice))]
public class WebGPUDevice : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPUDevice> Native { get; }
    internal WebGPUDevice(WGPUObjectHolder<FFI.WGPUDevice> holder) { Native = holder; }
    public void Dispose() { Native.Dispose(); }

    public WebGPUBindGroup CreateBindGroup(WebGPUBindGroupDescriptor bindGroupDescriptor)
    {
        unsafe
        {
            using var ffiMem = new FFIMemoryManager(stackalloc byte[256]);
            var ffi = bindGroupDescriptor.ToF(ffiMem);
            return new(new(FFI.WGPUDevice.wgpuDeviceCreateBindGroup(Native.GetPtr(), &ffi)));
        }
    }

    public WebGPUBindGroupLayout CreateBindGroupLayout(WebGPUBindGroupLayoutDescriptor bindGroupLayoutDescriptor)
    {
        unsafe
        {
            using var ffiMem = new FFIMemoryManager(stackalloc byte[256]);
            var ffi = bindGroupLayoutDescriptor.ToF(ffiMem);
            return new(new(FFI.WGPUDevice.wgpuDeviceCreateBindGroupLayout(Native.GetPtr(), &ffi)));
        }
    }
    public WebGPUBuffer? CreateBuffer(WebGPUBufferDescriptor bufferDescriptor)
    {
        unsafe
        {
            using var ffiMem = new FFIMemoryManager(stackalloc byte[256]);
            var ffi = bufferDescriptor.ToF(ffiMem, false);
            var buffer = FFI.WGPUDevice.wgpuDeviceCreateBuffer(Native.GetPtr(), &ffi);

            if (buffer is null) { return null; }
            return new(new(buffer));
        }
    }
    public (WebGPUBuffer, WebGPUBuffer.Mapped)? CreateMappedBuffer(WebGPUBufferDescriptor bufferDescriptor)
    {
        unsafe
        {
            using var ffiMem = new FFIMemoryManager(stackalloc byte[256]);
            var ffi = bufferDescriptor.ToF(ffiMem, true);
            var buffer = FFI.WGPUDevice.wgpuDeviceCreateBuffer(Native.GetPtr(), &ffi);

            if (buffer is null) { return null; }
            var wrappedBuffer = new WebGPUBuffer(new(buffer));
            return (wrappedBuffer, new(wrappedBuffer));
        }
    }
    public WebGPUCommandEncoder CreateCommandEncoder(WebGPUCommandEncoderDescriptor? commandEncoderDescriptor = null)
    {
        unsafe
        {
            using var ffiMem = new FFIMemoryManager(stackalloc byte[32]);
            var ffi = commandEncoderDescriptor is not null ? commandEncoderDescriptor.ToF(ffiMem) : default;
            var ptr = commandEncoderDescriptor is not null ? &ffi : null;
            return new(new(FFI.WGPUDevice.wgpuDeviceCreateCommandEncoder(Native.GetPtr(), ptr)));
        }
    }
    public WebGPUComputePipeline CreateComputePipeline(WebGPUComputePipelineDescriptor computePipelineDescriptor)
    {
        unsafe
        {
            using var ffiMem = new FFIMemoryManager(stackalloc byte[256]);
            var ffi = computePipelineDescriptor.ToF(ffiMem);
            return new(new(FFI.WGPUDevice.wgpuDeviceCreateComputePipeline(Native.GetPtr(), &ffi)));
        }
    }
    public Task<WebGPUComputePipeline> CreateComputePipelineAsync(WebGPUComputePipelineDescriptor computePipelineDescriptor)
    {
        var taskCompletionSource = new TaskCompletionSource<WebGPUComputePipeline>();
        unsafe
        {
            using var ffiMem = new FFIMemoryManager(stackalloc byte[256]);
            var ffiCb = new FFI.WGPUCreateComputePipelineAsyncCallbackInfo()
            {
                CallBackMode = FFI.WGPUCallbackMode.AllowSpontaneous,
                WGPUCreateComputePipelineAsyncCallback = &FFI.WGPUCreateComputePipelineAsyncCallbackManagedWrapper.CallBack,
                UserData1 = FFI.WGPUCreateComputePipelineAsyncCallbackManagedWrapper.CreateUserData(
                    new CreateComputePipelineAsyncCallback(taskCompletionSource)
                )
            };

            var ffiDesc = computePipelineDescriptor.ToF(ffiMem);
            var _ = FFI.WGPUDevice.wgpuDeviceCreateComputePipelineAsync(Native.GetPtr(), &ffiDesc, ffiCb);
        }
        return taskCompletionSource.Task;
    }
    class CreateComputePipelineAsyncCallback(TaskCompletionSource<WebGPUComputePipeline> taskCompletionSource) : FFI.IWGPUCreateComputePipelineAsyncCallback
    {
        public TaskCompletionSource<WebGPUComputePipeline> TaskCompletionSource { get; } = taskCompletionSource;

        public unsafe void CallBack(
            FFI.WGPUCreatePipelineAsyncStatus status,

            [FFI.WebGPUPassedWithOwnership(true)]
            FFI.WGPUComputePipeline* pipeline,

            [FFI.WebGPUOutString]
            FFI.WGPUStringView message
        )
        {
            switch (status)
            {
                case FFI.WGPUCreatePipelineAsyncStatus.Success:
                    {
                        TaskCompletionSource.SetResult(new(new(pipeline)));
                        return;
                    }
                case FFI.WGPUCreatePipelineAsyncStatus.InternalError:
                    {
                        TaskCompletionSource.SetException(new CallBackErrorException("InternalError : " + message.ReadStringView() ?? "message not found"));
                        return;
                    }
                case FFI.WGPUCreatePipelineAsyncStatus.ValidationError:
                    {
                        TaskCompletionSource.SetException(new CallBackErrorException("ValidationError : " + message.ReadStringView() ?? "message not found"));
                        return;
                    }
                case FFI.WGPUCreatePipelineAsyncStatus.CallbackCancelled:
                    {
                        TaskCompletionSource.SetCanceled();
                        return;
                    }
            }
            throw new InvalidCallBackStatusException(message.ReadStringView() ?? "message not found");
        }
    }
    public WebGPUPipelineLayout CreatePipelineLayout(WebGPUPipelineLayoutDescriptor pipelineLayoutDescriptor)
    {
        unsafe
        {
            using var ffiMem = new FFIMemoryManager(stackalloc byte[256]);
            var ffi = pipelineLayoutDescriptor.ToF(ffiMem);
            return new(new(FFI.WGPUDevice.wgpuDeviceCreatePipelineLayout(Native.GetPtr(), &ffi)));
        }
    }
    public WebGPUQuerySet CreateQuerySet(WebGPUQuerySetDescriptor querySetDescriptor)
    {
        unsafe
        {
            using var ffiMem = new FFIMemoryManager(stackalloc byte[64]);
            var ffi = querySetDescriptor.ToF(ffiMem);
            return new(new(FFI.WGPUDevice.wgpuDeviceCreateQuerySet(Native.GetPtr(), &ffi)));
        }
    }

    public Task<WebGPURenderPipeline> CreateRenderPipelineAsync(WebGPURenderPipelineDescriptor renderPipelineDescriptor)
    {
        var taskCompletionSource = new TaskCompletionSource<WebGPURenderPipeline>();
        unsafe
        {
            var ffiCb = new FFI.WGPUCreateRenderPipelineAsyncCallbackInfo()
            {
                CallBackMode = FFI.WGPUCallbackMode.AllowSpontaneous,
                WGPUCreateRenderPipelineAsyncCallback = &FFI.WGPUCreateRenderPipelineAsyncCallbackManagedWrapper.CallBack,
                UserData1 = FFI.WGPUCreateRenderPipelineAsyncCallbackManagedWrapper.CreateUserData(new CreateRenderPipelineAsyncCallback(taskCompletionSource))
            };
            using var ffiMem = new FFIMemoryManager(stackalloc byte[256]);
            var ffiDesc = renderPipelineDescriptor.ToF(ffiMem);
            var _ = FFI.WGPUDevice.wgpuDeviceCreateRenderPipelineAsync(Native.GetPtr(), &ffiDesc, ffiCb);
        }
        return taskCompletionSource.Task;
    }
    class CreateRenderPipelineAsyncCallback(TaskCompletionSource<WebGPURenderPipeline> taskCompletionSource) : FFI.IWGPUCreateRenderPipelineAsyncCallback
    {
        public TaskCompletionSource<WebGPURenderPipeline> TaskCompletionSource { get; } = taskCompletionSource;

        public unsafe void CallBack(
            FFI.WGPUCreatePipelineAsyncStatus status,

            [FFI.WebGPUPassedWithOwnership(true)]
            FFI.WGPURenderPipeline* pipeline,

            [FFI.WebGPUOutString]
            FFI.WGPUStringView message
        )
        {
            switch (status)
            {
                case FFI.WGPUCreatePipelineAsyncStatus.Success:
                    {
                        TaskCompletionSource.SetResult(new(new(pipeline)));
                        return;
                    }
                case FFI.WGPUCreatePipelineAsyncStatus.InternalError:
                    {
                        TaskCompletionSource.SetException(new CallBackErrorException("InternalError : " + message.ReadStringView() ?? "message not found"));
                        return;
                    }
                case FFI.WGPUCreatePipelineAsyncStatus.ValidationError:
                    {
                        TaskCompletionSource.SetException(new CallBackErrorException("ValidationError : " + message.ReadStringView() ?? "message not found"));
                        return;
                    }
                case FFI.WGPUCreatePipelineAsyncStatus.CallbackCancelled:
                    {
                        TaskCompletionSource.SetCanceled();
                        return;
                    }

                    throw new InvalidCallBackStatusException(message.ReadStringView() ?? "message not found");
            }
        }
    }
    public WebGPURenderBundleEncoder CreateRenderBundleEncoder(WebGPURenderBundleEncoderDescriptor renderBundleEncoderDescriptor)
    {
        unsafe
        {
            using var ffiMem = new FFIMemoryManager(stackalloc byte[256]);
            var ffi = renderBundleEncoderDescriptor.ToF(ffiMem);
            return new(new(FFI.WGPUDevice.wgpuDeviceCreateRenderBundleEncoder(Native.GetPtr(), &ffi)));
        }
    }
    public WebGPURenderPipeline CreateRenderPipeline(WebGPURenderPipelineDescriptor renderPipelineDescriptor)
    {
        unsafe
        {
            using var ffiMem = new FFIMemoryManager(stackalloc byte[64]);
            var ffiDesc = renderPipelineDescriptor.ToF(ffiMem);
            return new(new(FFI.WGPUDevice.wgpuDeviceCreateRenderPipeline(Native.GetPtr(), &ffiDesc)));
        }
    }
    public WebGPUSampler CreateSampler(WebGPUSamplerDescriptor samplerDescriptor)
    {
        unsafe
        {
            using var ffiMem = new FFIMemoryManager(stackalloc byte[64]);
            var ffi = samplerDescriptor.ToF(ffiMem);
            return new(new(FFI.WGPUDevice.wgpuDeviceCreateSampler(Native.GetPtr(), &ffi)));
        }
    }
    public WebGPUShaderModule CreateShaderModule(WebGPUShaderModuleDescriptor shaderModuleDescriptor)
    {
        unsafe
        {
            using var ffiMem = new FFIMemoryManager(stackalloc byte[128]);
            var ffi = shaderModuleDescriptor.ToF(ffiMem);
            return new(new(FFI.WGPUDevice.wgpuDeviceCreateShaderModule(Native.GetPtr(), &ffi)));
        }
    }
    public WebGPUTexture CreateTexture(WebGPUTextureDescriptor textureDescriptor)
    {
        unsafe
        {
            using var ffiMem = new FFIMemoryManager(stackalloc byte[128]);
            var ffi = textureDescriptor.ToF(ffiMem);
            return new(new(FFI.WGPUDevice.wgpuDeviceCreateTexture(Native.GetPtr(), &ffi)));
        }
    }
    public void Destroy()
    {
        unsafe { FFI.WGPUDevice.wgpuDeviceDestroy(Native.GetPtr()); }
    }
    internal void GetLostFuture()
    {
        unsafe
        {
            // TODO
            _ = FFI.WGPUDevice.wgpuDeviceGetLostFuture(Native.GetPtr());
        }
    }
    public WebGPULimits GetLimits()
    {
        unsafe
        {
            FFI.WGPULimits ffiLimits = default;
            FFI.WGPUDevice.wgpuDeviceGetLimits(Native.GetPtr(), &ffiLimits);
            return WebGPULimits.ToW(ffiLimits);
        }
    }
    public bool HasFeature(WebGPUFeatureName webGPUFeatureName)
    {
        unsafe { return (bool)FFI.WGPUDevice.wgpuDeviceHasFeature(Native.GetPtr(), webGPUFeatureName.ToF()); }
    }
    public WebGPUFeatureName[] GetFeatures()
    {
        unsafe
        {
            FFI.WGPUSupportedFeatures features = default;
            FFI.WGPUDevice.wgpuDeviceGetFeatures(Native.GetPtr(), &features);
            var managedFeatuers = new ReadOnlySpan<FFI.WGPUFeatureName>(features.Features, (int)features.FeaturesCount).ToArray().Select(WebGPUFeatureNameUtil.ToW).ToArray();
            FFI.WGPUSupportedFeatures.FreeMembers(ref features);
            return managedFeatuers;
        }
    }
    public WebGPUAdapterInfo GetAdapterInfo()
    {
        unsafe
        {
            FFI.WGPUAdapterInfo ffi = default;
            FFI.WGPUDevice.wgpuDeviceGetAdapterInfo(Native.GetPtr(), &ffi);
            var managed = WebGPUAdapterInfo.ToW(ffi);
            FFI.WGPUAdapterInfo.FreeMembers(ref ffi);
            return managed;
        }
    }
    public WebGPUQueue GetQueue()
    {
        unsafe { return new(new(FFI.WGPUDevice.wgpuDeviceGetQueue(Native.GetPtr()))); }
    }
    public void PushErrorScope(WebGPUErrorFilter errorFilter)
    {
        unsafe
        {
            FFI.WGPUDevice.wgpuDevicePushErrorScope(Native.GetPtr(), errorFilter.ToF());
        }
    }
    public Task<(WebGPUErrorType, string)> PopErrorScope()
    {
        var task = new TaskCompletionSource<(WebGPUErrorType, string)>();
        unsafe
        {
            var managedCallbackReceiver = new PopErrorScopeCallback(task);
            FFI.WGPUPopErrorScopeCallbackInfo popErrorScopeCallbackInfo = new()
            {
                CallBackMode = FFI.WGPUCallbackMode.AllowSpontaneous,
                WGPUPopErrorScopeCallback = &FFI.WGPUPopErrorScopeCallbackManagedWrapper.CallBack,
                UserData1 = FFI.WGPUPopErrorScopeCallbackManagedWrapper.CreateUserData(managedCallbackReceiver)
            };
            FFI.WGPUDevice.wgpuDevicePopErrorScope(Native.GetPtr(), popErrorScopeCallbackInfo);
        }
        return task.Task;
    }
    [FFINote(typeof(FFI.WGPUPopErrorScopeCallbackInfo))]
    class PopErrorScopeCallback(TaskCompletionSource<(WebGPUErrorType, string)> task) : FFI.IWGPUPopErrorScopeCallback
    {
        public TaskCompletionSource<(WebGPUErrorType, string)> Task { get; } = task;

        public void CallBack(FFI.WGPUPopErrorScopeStatus status,
             FFI.WGPUErrorType type,

             [FFI.WebGPUPassedWithOwnership(false)]
            [FFI.WebGPUOutString]
            FFI.WGPUStringView message
         )
        {
            var managedMessage = message.ReadStringView();
            switch (status)
            {
                default: break;

                case FFI.WGPUPopErrorScopeStatus.Success:
                    {
                        Task.SetResult((type.ToW(), managedMessage ?? "message not found"));
                        return;
                    }
                case FFI.WGPUPopErrorScopeStatus.CallbackCancelled:
                    {
                        Console.WriteLine("PopErrorScopeCallback CallbackCancelled : " + managedMessage);
                        // TODO
                        Task.SetCanceled();
                        return;
                    }
                case FFI.WGPUPopErrorScopeStatus.Error:
                    {
                        Task.SetException(new CallBackErrorException(managedMessage ?? "message not found"));
                        return;
                    }
            }
            throw new InvalidCallBackStatusException("PopErrorScopeCallback : " + managedMessage);
        }
    }
    public void SetLabel(string label)
    {
        unsafe
        {
            using var ffiMem = new FFIMemoryManager(stackalloc byte[16]);
            FFI.WGPUDevice.wgpuDeviceSetLabel(Native.GetPtr(), ffiMem.AllocateString(label));
        }
    }

}
