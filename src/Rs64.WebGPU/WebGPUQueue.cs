// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Threading.Tasks;

namespace Rs64.WebGPU;

[FFINote(typeof(FFI.WGPUQueue))]
public class WebGPUQueue : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPUQueue> Native { get; }
    internal WebGPUQueue(WGPUObjectHolder<FFI.WGPUQueue> holder) { Native = holder; }
    public void Dispose() { Native.Dispose(); }

    public void Submit(WebGPUCommandBuffer[] commandBuffers)
    {
        unsafe
        {
            var cb = stackalloc FFI.WGPUCommandBuffer*[commandBuffers.Length];
            for (var i = 0; commandBuffers.Length > i; i += 1)
                cb[i] = commandBuffers[i].Native.GetPtr();

            FFI.WGPUQueue.wgpuQueueSubmit(Native.GetPtr(), (nuint)commandBuffers.Length, cb);
        }
    }
    public Task OnSubmittedWorkDone()
    {
        var taskCompletionSource = new TaskCompletionSource();
        unsafe
        {
            var cb = new FFI.WGPUQueueWorkDoneCallbackInfo()
            {
                CallBackMode = FFI.WGPUCallbackMode.AllowSpontaneous,
                WGPUQueueWorkDoneCallback = &FFI.WGPUQueueWorkDoneCallbackManagedWrapper.CallBack,
                UserData1 = FFI.WGPUQueueWorkDoneCallbackManagedWrapper.CreateUserData(new QueueWorkDoneCallback(taskCompletionSource)),
            };
            FFI.WGPUQueue.wgpuQueueOnSubmittedWorkDone(Native.GetPtr(), cb);
        }
        return taskCompletionSource.Task;
    }
    class QueueWorkDoneCallback(TaskCompletionSource taskCompletionSource) : FFI.IWGPUQueueWorkDoneCallback
    {
        public TaskCompletionSource Task { get; } = taskCompletionSource;

        public void CallBack(FFI.WGPUQueueWorkDoneStatus status,
            [FFI.WebGPUPassedWithOwnership(false)]
            [FFI.WebGPUOutString]
            FFI.WGPUStringView message
        )
        {
            var managedMessage = message.ReadStringView();
            switch (status)
            {
                default: break;

                case FFI.WGPUQueueWorkDoneStatus.Success:
                    {
                        Task.SetResult();
                        return;
                    }
                case FFI.WGPUQueueWorkDoneStatus.CallbackCancelled:
                    {
                        Console.WriteLine("QueueWorkDoneCallback CallbackCancelled : " + managedMessage);
                        // TODO
                        Task.SetCanceled();
                        return;
                    }
                case FFI.WGPUQueueWorkDoneStatus.Error:
                    {
                        Task.SetException(new CallBackErrorException(managedMessage ?? "message not found"));
                        return;
                    }
            }
            throw new InvalidCallBackStatusException("QueueWorkDoneCallback : " + managedMessage);
        }
    }
    public void WriteBuffer(
        WebGPUBuffer buffer,
        ulong bufferOffset,
        ReadOnlySpan<byte> data
    )
    {
        unsafe
        {
            fixed (byte* bytePtr = data)
            {
                FFI.WGPUQueue.wgpuQueueWriteBuffer(Native.GetPtr(), buffer.Native.GetPtr(), bufferOffset, bytePtr, (nuint)data.Length);
            }
        }
    }
    public void WriteTexture(WebGPUTexelCopyTextureInfo destination,
        ReadOnlySpan<byte> data,
        WebGPUTexelCopyBufferLayout dataLayout,
        WebGPUExtent3d writeSize
    )
    {
        unsafe
        {
            fixed (byte* bytePtr = data)
            {
                var ffiDestination = destination.ToF();
                var ffiLayout = dataLayout.ToF();
                var ffiWriteSize = writeSize.ToF();
                FFI.WGPUQueue.wgpuQueueWriteTexture(Native.GetPtr(),
                    &ffiDestination,
                    bytePtr,
                    (nuint)data.Length,
                    &ffiLayout,
                    &ffiWriteSize
                );
            }
        }
    }
    public void SetLabel(string label)
    {
        unsafe
        {
            using var ffiMem = new FFIStackMemory(stackalloc nint[1]);
            FFI.WGPUQueue.wgpuQueueSetLabel(Native.GetPtr(), ffiMem.AllocateString(label));
        }
    }
}
