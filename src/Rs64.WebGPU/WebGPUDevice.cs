// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
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
    public void CreateComputePipeline()
    {

    }
    public void CreateComputePipelineAsync()
    {

    }
    public void CreatePipelineLayout()
    {

    }
    public void CreateQuerySet()
    {

    }
    public void CreateRenderPipelineAsync()
    {

    }
    public void CreateRenderBundleEncoder()
    {

    }
    public void CreateRenderPipeline()
    {

    }
    public void CreateSampler()
    {

    }
    public void CreateShaderModule()
    {

    }
    public void CreateTexture()
    {

    }
    public void Destroy()
    {

    }
    public void GetLostFuture()
    {

    }
    public void GetLimits()
    {

    }
    public void HasFeature()
    {

    }
    public void GetFeatures()
    {

    }
    public void GetAdapterInfo()
    {

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
