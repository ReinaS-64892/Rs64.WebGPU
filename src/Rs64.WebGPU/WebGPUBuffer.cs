// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Threading.Tasks;

namespace Rs64.WebGPU;

[FFINote(typeof(FFI.WGPUBuffer))]
public class WebGPUBuffer : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPUBuffer> Native { get; }
    internal WebGPUBuffer(WGPUObjectHolder<FFI.WGPUBuffer> holder) { Native = holder; }
    public void Dispose() { Native.Dispose(); }


    public Task<Mapped> MapAsync(WebGPUMapMode mapMode, nuint offset, nuint size)
    {
        var task = new TaskCompletionSource<Mapped>();
        unsafe
        {
            FFI.WGPUBufferMapCallbackInfo bufferMapCallbackInfo = new()
            {
                CallBackMode = FFI.WGPUCallbackMode.AllowSpontaneous,
                WGPUBufferMapCallback = &FFI.WGPUBufferMapCallbackManagedWrapper.CallBack,
                UserData1 = FFI.WGPUBufferMapCallbackManagedWrapper.CreateUserData(new BufferMapCallback(this, task)),
            };
            FFI.WGPUBuffer.wgpuBufferMapAsync(Native.GetPtr(), mapMode.ToF(), offset, size, bufferMapCallbackInfo);
        }
        return task.Task;
    }
    class BufferMapCallback(WebGPUBuffer webGPUBuffer, TaskCompletionSource<WebGPUBuffer.Mapped> task) : FFI.IWGPUBufferMapCallback
    {
        public WebGPUBuffer WebGPUBuffer { get; } = webGPUBuffer;
        public TaskCompletionSource<Mapped> Task { get; } = task;

        public void CallBack(
            FFI.WGPUMapAsyncStatus status,

            [FFI.WebGPUPassedWithOwnership(false)]
            [FFI.WebGPUOutString]
            FFI.WGPUStringView message
        )
        {
            var managedMessage = message.ReadStringView() ?? "message not found";
            switch (status)
            {
                case FFI.WGPUMapAsyncStatus.Success:
                    {
                        Task.SetResult(new(WebGPUBuffer));
                        break;
                    }
                case FFI.WGPUMapAsyncStatus.CallbackCancelled:
                    {
                        // TODO : log !!!
                        Console.WriteLine("MapAsync CallBackCancelled : " + managedMessage);
                        Task.SetCanceled();
                        break;
                    }
                case FFI.WGPUMapAsyncStatus.Error:
                    {
                        Task.SetException(new CallBackErrorException("MapAsync Error : " + managedMessage));
                        break;
                    }
                case FFI.WGPUMapAsyncStatus.Aborted:
                    {
                        Task.SetException(new CallBackAbortException("MapAsync Aborted : " + managedMessage));
                        break;
                    }
            }
            throw new InvalidCallBackStatusException("MapAsync : " + managedMessage);
        }
    }
    public class Mapped : IDisposable
    {
        private WebGPUBuffer? buffer;
        internal Mapped(WebGPUBuffer buffer)
        {
            this.buffer = buffer;
        }

        public Span<byte> GetMappedRange(nuint offset = 0, nuint? size = null)
        {
            unsafe
            {
                var actualSize = size ?? (nuint)(buffer!.GetSize() - offset);
                var ptr = FFI.WGPUBuffer.wgpuBufferGetMappedRange(buffer!.Native.GetPtr(), offset, actualSize);

                if (int.MaxValue < actualSize) { throw new Exception(); }
                return new(ptr, (int)actualSize);
            }
        }
        public ReadOnlySpan<byte> GetConstMappedRange(nuint offset = 0, nuint? size = null)
        {
            unsafe
            {
                var actualSize = size ?? (nuint)(buffer!.GetSize() - offset);
                var ptr = FFI.WGPUBuffer.wgpuBufferGetMappedRange(buffer!.Native.GetPtr(), offset, actualSize);

                if (int.MaxValue < actualSize) { throw new Exception(); }
                return new(ptr, (int)actualSize);
            }
        }
        public bool ReadMappedRange(Span<byte> destinationSpan, nuint sourceOffset = 0)
        {
            unsafe
            {
                fixed (byte* destinationPtr = destinationSpan)
                {
                    var result = FFI.WGPUBuffer.wgpuBufferReadMappedRange(buffer!.Native.GetPtr(), sourceOffset, destinationPtr, (nuint)destinationSpan.Length);
                    return result is FFI.WGPUStatus.Success;
                }
            }
        }
        public bool WriteMappedRange(ReadOnlySpan<byte> sourceSpan, nuint destinationOffset = 0)
        {
            unsafe
            {
                fixed (byte* destinationPtr = sourceSpan)
                {
                    var result = FFI.WGPUBuffer.wgpuBufferWriteMappedRange(buffer!.Native.GetPtr(), destinationOffset, destinationPtr, (nuint)sourceSpan.Length);
                    return result is FFI.WGPUStatus.Success;
                }
            }
        }

        public void Dispose()
        {
            if (buffer is not null)
            {
                Unmap();
                buffer = null;
            }
        }
        public void Unmap()
        {
            unsafe { FFI.WGPUBuffer.wgpuBufferUnmap(buffer!.Native.GetPtr()); }
        }
    }


    public void SetLabel(string label)
    {
        unsafe
        {
            fixed (byte* labelPtr = FFI.WGPUStringView.ConvertWGPUStringParts(label, out var lLength))
                FFI.WGPUBuffer.wgpuBufferSetLabel(Native.GetPtr(), new FFI.WGPUStringView(labelPtr, lLength));
        }
    }
    public WebGPUBufferUsage GetUsage()
    {
        unsafe { return FFI.WGPUBuffer.wgpuBufferGetUsage(Native.GetPtr()).ToW(); }
    }
    public ulong GetSize()
    {
        unsafe { return FFI.WGPUBuffer.wgpuBufferGetSize(Native.GetPtr()); }
    }
    public WebGPUBufferMapState GetMapState()
    {
        unsafe { return FFI.WGPUBuffer.wgpuBufferGetMapState(Native.GetPtr()).ToW(); }
    }
    public void Destroy()
    {
        unsafe { FFI.WGPUBuffer.wgpuBufferDestroy(Native.GetPtr()); }

    }

}

[FFINote(typeof(FFI.WGPUBufferDescriptor))]
public class WebGPUBufferDescriptor
{
    public string Label = "";
    public WebGPUBufferUsage Usage = WebGPUBufferUsage.None;
    public ulong Size;

    internal FFI.WGPUBufferDescriptor ToF(ref FFIStackMemory ffiMem, bool mappedAtCreation)
    {
        return new FFI.WGPUBufferDescriptor()
        {
            Label = ffiMem.AllocateString(Label),
            Usage = Usage.ToF(),
            Size = Size,
            MappedAtCreation = (FFI.WGPUBool)mappedAtCreation
        };
    }
}
