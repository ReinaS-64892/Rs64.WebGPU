// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Rs64.WebGPU;

public partial class WebGPUAdapter
{
    class RequestDeviceCallBack(ChannelWriter<WebGPUFuture> futureChannel, TaskCompletionSource<WebGPUDevice> task) : FFI.IWGPURequestDeviceCallback
    {
        public ChannelWriter<WebGPUFuture> FutureChannel { get; } = futureChannel;
        public TaskCompletionSource<WebGPUDevice> Task { get; } = task;
        public unsafe void CallBack(
            FFI.WGPURequestDeviceStatus status
            ,
            [FFI.WebGPUPassedWithOwnership(true)]
            FFI.WGPUDevice* device
            ,
            [FFI.WebGPUPassedWithOwnership(false)]
            [FFI.WebGPUOutString]
            FFI.WGPUStringView message
        )
        {
            switch (status)
            {
                case FFI.WGPURequestDeviceStatus.Success:
                    {
                        Task.SetResult(new(new(device),FutureChannel));
                        return;
                    }
                case FFI.WGPURequestDeviceStatus.Error:
                    {
                        Task.SetException(new CallBackErrorException(message.ReadStringView() ?? "message not found"));
                        return;
                    }
                case FFI.WGPURequestDeviceStatus.CallbackCancelled:
                    {
                        Task.SetCanceled();
                        return;
                    }
            }
            throw new InvalidCallBackStatusException(message.ReadStringView() ?? "message not found");
        }
    }
    class DeviceLostCallback(Action? lostCallBack) : FFI.IWGPUDeviceLostCallback
    {
        private readonly Action? _lostCallBack = lostCallBack;

        public unsafe void CallBack(
            [FFI.WebGPUPassedWithOwnership(false)]
            [FFI.WebGPUImmutablePointer]
            FFI.WGPUDevice* device,

            FFI.WGPUDeviceLostReason reason,

            [FFI.WebGPUPassedWithOwnership(false)]
            [FFI.WebGPUOutString]
            FFI.WGPUStringView message
        )
        {
            var outLogString = reason.ToString() + " : " + message.ReadStringView();
            Console.WriteLine(outLogString);
            // TODO : Log の仕組み！！！
            _lostCallBack?.Invoke();
        }
    }
    class UncapturedErrorCallback() : FFI.IWGPUUncapturedErrorCallback
    {
        public unsafe void CallBack(
            [FFI.WebGPUPassedWithOwnership(false)]
            [FFI.WebGPUImmutablePointer]
            FFI.WGPUDevice* device,

            FFI.WGPUErrorType type,

            [FFI.WebGPUPassedWithOwnership(false)]
            [FFI.WebGPUOutString]
            FFI.WGPUStringView message
        )
        {
            var outLogString = type.ToString() + " : " + message.ReadStringView();
            Console.WriteLine(outLogString);
            // TODO : Log の仕組み2！！！
        }
    }
}
