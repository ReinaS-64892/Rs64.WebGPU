// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Rs64.WebGPU;

[FFINote(typeof(FFI.WGPUShaderModule))]
public class WebGPUShaderModule : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPUShaderModule> Native { get; }
    internal ChannelWriter<WebGPUFuture> FutureChannel { get; }
    internal WebGPUShaderModule(WGPUObjectHolder<FFI.WGPUShaderModule> holder, ChannelWriter<WebGPUFuture> futureChannel)
    {
        Native = holder;
        FutureChannel = futureChannel;
    }
    public void Dispose() { Native.Dispose(); }
    public Task<WebGPUCompilationInfo> GetCompilationInfo()
    {
        var taskCompletionSource = new TaskCompletionSource<WebGPUCompilationInfo>(TaskCreationOptions.RunContinuationsAsynchronously);
        unsafe
        {
            var cb = new FFI.WGPUCompilationInfoCallbackInfo()
            {
                CallBackMode = FFI.WGPUCallbackMode.AllowSpontaneous,
                WGPUCompilationInfoCallback = &FFI.WGPUCompilationInfoCallbackManagedWrapper.CallBack,
                UserData1 = FFI.WGPUCompilationInfoCallbackManagedWrapper.CreateUserData(new CompilationInfoCallback(taskCompletionSource)),
            };
            var future = FFI.WGPUShaderModule.wgpuShaderModuleGetCompilationInfo(Native.GetPtr(), cb);
            if (FutureChannel.TryWrite(new(future)) is false) { Console.WriteLine(" failed : future send to manager"); }
        }
        return taskCompletionSource.Task;
    }
    public void SetLabel(string label)
    {
        unsafe
        {
            using var ffiMem = new FFIStackMemory(stackalloc nint[1]);
            FFI.WGPUShaderModule.wgpuShaderModuleSetLabel(Native.GetPtr(), ffiMem.AllocateString(label));
        }
    }

    private class CompilationInfoCallback : FFI.IWGPUCompilationInfoCallback
    {
        private TaskCompletionSource<WebGPUCompilationInfo> Task;

        public CompilationInfoCallback(TaskCompletionSource<WebGPUCompilationInfo> taskCompletionSource)
        {
            Task = taskCompletionSource;
        }

        public unsafe void CallBack(
            FFI.WGPUCompilationInfoRequestStatus status,

            [FFI.WebGPUPassedWithOwnership(false)]
            [FFI.WebGPUImmutablePointer]
            FFI.WGPUCompilationInfo* compilation_info
        )
        {
            switch (status)
            {
                default: break;

                case FFI.WGPUCompilationInfoRequestStatus.Success:
                    {

                        Task.SetResult(WebGPUCompilationInfo.ToW(ref Unsafe.AsRef<FFI.WGPUCompilationInfo>(compilation_info)));
                        return;
                    }
                case FFI.WGPUCompilationInfoRequestStatus.CallbackCancelled:
                    {
                        Console.WriteLine("CompilationInfoCallback CallbackCancelled");
                        // TODO
                        Task.SetCanceled();
                        return;
                    }
            }
            throw new InvalidCallBackStatusException("CompilationInfoCallback");
        }
    }
}
[FFINote(typeof(FFI.WGPUCompilationInfo))]
public class WebGPUCompilationInfo
{
    public required WebGPUCompilationMessage[] Messages;

    internal static unsafe WebGPUCompilationInfo ToW(ref FFI.WGPUCompilationInfo ffi)
    {
        var len = (int)ffi.MessagesCount;
        var ptr = ffi.Messages;
        var managed = new WebGPUCompilationMessage[len];
        for (var i = 0; len > i; i += 1)
        {
            managed[i] = WebGPUCompilationMessage.ToW(ptr[i]);
        }
        return new WebGPUCompilationInfo()
        {
            Messages = managed,
        };
    }
}
[FFINote(typeof(FFI.WGPUCompilationMessage))]
public class WebGPUCompilationMessage
{
    public required string Message;
    public WebGPUCompilationMessageType Type;
    public ulong LineNum;
    public ulong LinePos;
    public ulong Offset;
    public ulong Length;
    internal static WebGPUCompilationMessage ToW(FFI.WGPUCompilationMessage wGPUCompilationMessage)
    {
        return new()
        {
            Message = wGPUCompilationMessage.Message.ReadStringView() ?? "",
            Type = wGPUCompilationMessage.Type.ToW(),
            LineNum = wGPUCompilationMessage.LineNum,
            LinePos = wGPUCompilationMessage.LinePos,
            Offset = wGPUCompilationMessage.Offset,
            Length = wGPUCompilationMessage.Length,
        };
    }
}

[FFINote(typeof(FFI.WGPUShaderModuleDescriptor))]
public abstract class WebGPUShaderModuleDescriptor
{
    public string Label = "";

    internal abstract FFI.WGPUShaderModuleDescriptor ToF(ref FFIStackMemory ffiMem);
}

[FFINote(typeof(FFI.WGPUShaderSourceSpirv))]
public class WebGPUSpirvShaderModuleDescriptor : WebGPUShaderModuleDescriptor
{
    public required uint[] Code;
    internal unsafe override FFI.WGPUShaderModuleDescriptor ToF(ref FFIStackMemory ffiMem)
    {
        var source = ffiMem.AllocateArea<FFI.WGPUShaderSourceSpirv>();
        source->NextInChain.StructType = FFI.WGPUSType.ShaderSourceSpirv;
        source->CodeSize = (uint)Code.Length;
        source->Code = ffiMem.PinArray(Code);
        return new()
        {
            NextInChain = (FFI.WGPUChainedStruct*)source,
            Label = ffiMem.AllocateString(Label),
        };
    }
}

[FFINote(typeof(FFI.WGPUShaderSourceWgsl))]
public class WebGPUWgslShaderModuleDescriptor : WebGPUShaderModuleDescriptor
{
    public string Code = "";
    internal unsafe override FFI.WGPUShaderModuleDescriptor ToF(ref FFIStackMemory ffiMem)
    {
        var source = ffiMem.AllocateArea<FFI.WGPUShaderSourceWgsl>();
        source->NextInChain.StructType = FFI.WGPUSType.ShaderSourceWgsl;
        source->Code = ffiMem.AllocateString(Code);
        return new()
        {
            NextInChain = (FFI.WGPUChainedStruct*)source,
            Label = ffiMem.AllocateString(Label),
        };
    }
}
