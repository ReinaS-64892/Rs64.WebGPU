// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Collections.Generic;
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
            FFI.WGPUBindGroupDescriptor ffiGroupDescriptor = new();
            fixed (byte* labelPtr = FFI.WGPUStringView.ConvertWGPUStringParts(bindGroupDescriptor.Label, out var labelLength))
            {
                ffiGroupDescriptor.Label = new(labelPtr, labelLength);
                ffiGroupDescriptor.Layout = bindGroupDescriptor.Layout.Native.GetPtr();
                var ffiEntries = stackalloc FFI.WGPUBindGroupEntry[bindGroupDescriptor.Entries.Length];
                for (var i = 0; bindGroupDescriptor.Entries.Length > i; i += 1)
                {
                    ffiEntries[i] = WebGPUBindGroupEntry.ToF(bindGroupDescriptor.Entries[i]);
                }
                ffiGroupDescriptor.EntriesCount = (nuint)bindGroupDescriptor.Entries.Length;
                ffiGroupDescriptor.Entries = ffiEntries;
                return new(new(FFI.WGPUDevice.wgpuDeviceCreateBindGroup(Native.GetPtr(), &ffiGroupDescriptor)));
            }
        }
    }

    public WebGPUBindGroupLayout CreateBindGroupLayout(WebGPUBindGroupLayoutDescriptor bindGroupLayoutDescriptor)
    {
        unsafe
        {
            FFI.WGPUBindGroupLayoutDescriptor ffiBindGroupLayoutDescriptor = default;
            fixed (byte* labelPtr = FFI.WGPUStringView.ConvertWGPUStringParts(bindGroupLayoutDescriptor.Label, out var lLength))
            {
                ffiBindGroupLayoutDescriptor.Label = new(labelPtr, lLength);

                var ffiEntries = stackalloc FFI.WGPUBindGroupLayoutEntry[bindGroupLayoutDescriptor.Entries.Length];
                for (var i = 0; bindGroupLayoutDescriptor.Entries.Length > i; i += 1)
                {
                    ffiEntries[i] = WebGPUBindGroupLayoutEntry.ToF(bindGroupLayoutDescriptor.Entries[i]);
                }
                ffiBindGroupLayoutDescriptor.EntriesCount = (nuint)bindGroupLayoutDescriptor.Entries.Length;
                ffiBindGroupLayoutDescriptor.Entries = ffiEntries;

                return new(new(FFI.WGPUDevice.wgpuDeviceCreateBindGroupLayout(Native.GetPtr(), &ffiBindGroupLayoutDescriptor)));
            }
        }
    }
    public WebGPUBuffer? CreateBuffer(WebGPUBufferDescriptor bufferDescriptor)
    {
        unsafe
        {
            FFI.WGPUBufferDescriptor ffiBufferDescriptor = new();
            fixed (byte* labelPtr = FFI.WGPUStringView.ConvertWGPUStringParts(bufferDescriptor.Label, out var lLength))
            {
                ffiBufferDescriptor.Label = new(labelPtr, lLength);
                ffiBufferDescriptor.Usage = bufferDescriptor.Usage.ToF();
                ffiBufferDescriptor.Size = bufferDescriptor.Size;
                ffiBufferDescriptor.MappedAtCreation = (FFI.WGPUBool)false;

                var buffer = FFI.WGPUDevice.wgpuDeviceCreateBuffer(Native.GetPtr(), &ffiBufferDescriptor);
                if (buffer is null) { return null; }
                return new(new(buffer));
            }
        }
    }
    public (WebGPUBuffer, WebGPUBuffer.Mapped)? CreateMappedBuffer(WebGPUBufferDescriptor bufferDescriptor)
    {
        unsafe
        {
            FFI.WGPUBufferDescriptor ffiBufferDescriptor = new();
            fixed (byte* labelPtr = FFI.WGPUStringView.ConvertWGPUStringParts(bufferDescriptor.Label, out var lLength))
            {
                ffiBufferDescriptor.Label = new(labelPtr, lLength);
                ffiBufferDescriptor.Usage = bufferDescriptor.Usage.ToF();
                ffiBufferDescriptor.Size = bufferDescriptor.Size;
                ffiBufferDescriptor.MappedAtCreation = (FFI.WGPUBool)true;

                var buffer = FFI.WGPUDevice.wgpuDeviceCreateBuffer(Native.GetPtr(), &ffiBufferDescriptor);
                if (buffer is null) { return null; }

                var wrappedBuffer = new WebGPUBuffer(new(buffer));
                return (wrappedBuffer, new(wrappedBuffer));
            }
        }
    }
    public WebGPUCommandEncoder CreateCommandEncoder(WebGPUCommandEncoderDescriptor? commandEncoderDescriptor = null)
    {
        unsafe
        {
            if (commandEncoderDescriptor is not null)
            {
                fixed (byte* labelPtr = FFI.WGPUStringView.ConvertWGPUStringParts(commandEncoderDescriptor.Label, out var length))
                {
                    FFI.WGPUCommandEncoderDescriptor ffiCommandEncoderDescriptor = new()
                    {
                        Label = new(labelPtr, length)
                    };
                    return new(new(FFI.WGPUDevice.wgpuDeviceCreateCommandEncoder(Native.GetPtr(), &ffiCommandEncoderDescriptor)));
                }
            }
            else
            {
                return new(new(FFI.WGPUDevice.wgpuDeviceCreateCommandEncoder(Native.GetPtr(), null)));
            }
        }
    }
    public WebGPUComputePipeline CreateComputePipeline(WebGPUComputePipelineDescriptor computePipelineDescriptor)
    {
        unsafe
        {
            using var ffiMem = new FFIMemoryManager(stackalloc byte[256]);
            FFI.WGPUComputePipelineDescriptor ffiComputePipelineDescriptor = computePipelineDescriptor.ToF(ffiMem);

            return new WebGPUComputePipeline(new(FFI.WGPUDevice.wgpuDeviceCreateComputePipeline(Native.GetPtr(), &ffiComputePipelineDescriptor)));
        }
    }
    public Task<WebGPUComputePipeline> CreateComputePipelineAsync(WebGPUComputePipelineDescriptor computePipelineDescriptor)
    {
        var taskCompletionSource = new TaskCompletionSource<WebGPUComputePipeline>();
        unsafe
        {
            using var ffiMem = new FFIMemoryManager(stackalloc byte[256]);
            FFI.WGPUCreateComputePipelineAsyncCallbackInfo callbackInfo = new()
            {
                CallBackMode = FFI.WGPUCallbackMode.AllowSpontaneous,
                WGPUCreateComputePipelineAsyncCallback = &FFI.WGPUCreateComputePipelineAsyncCallbackManagedWrapper.CallBack,
                UserData1 = FFI.WGPUCreateComputePipelineAsyncCallbackManagedWrapper.CreateUserData(
                    new CreateComputePipelineAsyncCallback(taskCompletionSource)
                )
            };

            var ffiComputePipelineDescriptor = computePipelineDescriptor.ToF(ffiMem);
            var _ = FFI.WGPUDevice.wgpuDeviceCreateComputePipelineAsync(Native.GetPtr(), &ffiComputePipelineDescriptor, callbackInfo);
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
            var ffiPipelineLayoutDescriptor = pipelineLayoutDescriptor.ToF(ffiMem);
            return new(new(FFI.WGPUDevice.wgpuDeviceCreatePipelineLayout(Native.GetPtr(), &ffiPipelineLayoutDescriptor)));
        }
    }
    public WebGPUQuerySet CreateQuerySet(WebGPUQuerySetDescriptor querySetDescriptor)
    {
        unsafe
        {
            fixed (byte* lPtr = FFI.WGPUStringView.ConvertWGPUStringParts(querySetDescriptor.Label, out var ll))
            {
                FFI.WGPUQuerySetDescriptor ffiQuerySetDescriptor = new()
                {
                    Label = new(lPtr, ll),
                    Type = querySetDescriptor.Type.ToF(),
                    Count = querySetDescriptor.Count,
                };
                return new(new(FFI.WGPUDevice.wgpuDeviceCreateQuerySet(Native.GetPtr(),)));
            }
        }
    }

    public Task<WebGPURenderPipeline> CreateRenderPipelineAsync(WebGPURenderPipelineDescriptor renderPipelineDescriptor)
    {
        var taskCompletionSource = new TaskCompletionSource<WebGPURenderPipeline>();
        unsafe
        {
            FFI.WGPUCreateRenderPipelineAsyncCallbackInfo callback = new()
            {
                CallBackMode = FFI.WGPUCallbackMode.AllowSpontaneous,
                WGPUCreateRenderPipelineAsyncCallback = &FFI.WGPUCreateRenderPipelineAsyncCallbackManagedWrapper.CallBack,
                UserData1 = FFI.WGPUCreateRenderPipelineAsyncCallbackManagedWrapper.CreateUserData(new CreateRenderPipelineAsyncCallback(taskCompletionSource))
            };
            using var ffiMem = new FFIMemoryManager(stackalloc byte[256]);
            var ffiPipelineLayoutDescriptor = renderPipelineDescriptor.ToF(ffiMem);
            var _ = FFI.WGPUDevice.wgpuDeviceCreateRenderPipelineAsync(Native.GetPtr(), &ffiPipelineLayoutDescriptor, callback);
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

        }
    }
    public void CreateRenderBundleEncoder()
    {
        unsafe { FFI.WGPUDevice.wgpuDeviceCreateRenderBundleEncoder(Native.GetPtr()); }
    }
    public void CreateRenderPipeline()
    {
        unsafe { FFI.WGPUDevice.wgpuDeviceCreateRenderPipeline(Native.GetPtr()); }
    }
    public void CreateSampler()
    {
        unsafe { FFI.WGPUDevice.wgpuDeviceCreateSampler(Native.GetPtr()); }
    }
    public void CreateShaderModule()
    {
        unsafe { FFI.WGPUDevice.wgpuDeviceCreateShaderModule(Native.GetPtr()); }
    }
    public void CreateTexture()
    {
        unsafe { FFI.WGPUDevice.wgpuDeviceCreateTexture(Native.GetPtr()); }
    }
    public void Destroy()
    {
        unsafe { FFI.WGPUDevice.wgpuDeviceDestroy(Native.GetPtr()); }
    }
    public void GetLostFuture()
    {
        unsafe { FFI.WGPUDevice.wgpuDeviceGetLostFuture(Native.GetPtr()); }
    }
    public void GetLimits()
    {
        unsafe { FFI.WGPUDevice.wgpuDeviceGetLimits(Native.GetPtr()); }
    }
    public void HasFeature()
    {
        unsafe { FFI.WGPUDevice.wgpuDeviceHasFeature(Native.GetPtr()); }
    }
    public void GetFeatures()
    {
        unsafe { FFI.WGPUDevice.wgpuDeviceGetFeatures(Native.GetPtr()); }
    }
    public void GetAdapterInfo()
    {
        unsafe { FFI.WGPUDevice.wgpuDeviceGetAdapterInfo(Native.GetPtr()); }
    }
    // public WebGPUQueue GetQueue()
    // {
    //     unsafe { return new(new(FFI.WGPUDevice.wgpuDeviceGetQueue(Native.GetPtr()))); }
    // }
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
                        Console.WriteLine("PopErrorScopeCallback CallbackCancelled : " + managedMessage);//TODO
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
            fixed (byte* labelPtr = FFI.WGPUStringView.ConvertWGPUStringParts(label, out var lLength))
                FFI.WGPUDevice.wgpuDeviceSetLabel(Native.GetPtr(), new FFI.WGPUStringView(labelPtr, lLength));
        }
    }

}


[FFINote(typeof(FFI.WGPUQuerySetDescriptor))]
public class WebGPUQuerySetDescriptor
{
    public string Label = "";
    public WebGPUQueryType Type;
    public uint Count;
}

[FFINote(typeof(FFI.WGPUPipelineLayoutDescriptor))]
public class WebGPUPipelineLayoutDescriptor
{
    public string Label = "";
    public required WebGPUBindGroupLayout[] BindGroupLayouts;
    public uint ImmediateSize = 0;

    internal unsafe FFI.WGPUPipelineLayoutDescriptor ToF(FFIMemoryManager ffiMem)
    {
        var bindGroupLayoutsPtr = (FFI.WGPUBindGroupLayout**)ffiMem.stackArea.Allocate<IntPtr>(BindGroupLayouts.Length);
        for (var i = 0; BindGroupLayouts.Length > i; i += 1)
        {
            bindGroupLayoutsPtr[i] = BindGroupLayouts[i].Native.GetPtr();
        }
        return new()
        {
            Label = ffiMem.AllocateString(Label),
            BindGroupLayoutsCount = (nuint)BindGroupLayouts.Length,
            BindGroupLayouts = bindGroupLayoutsPtr,
            ImmediateSize = ImmediateSize,
        };
    }
}
