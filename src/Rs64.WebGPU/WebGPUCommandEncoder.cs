// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;

namespace Rs64.WebGPU;

[FFINote(typeof(FFI.WGPUCommandEncoder))]
public class WebGPUCommandEncoder : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPUCommandEncoder> Native { get; }
    internal WebGPUCommandEncoder(WGPUObjectHolder<FFI.WGPUCommandEncoder> holder) { Native = holder; }
    public void Dispose() { Native.Dispose(); }

    public WebGPUCommandBuffer Finish(WebGPUCommandBufferDescriptor? commandBufferDescriptor = null)
    {
        unsafe
        {
            if (commandBufferDescriptor is not null)
            {
                FFI.WGPUCommandBufferDescriptor ffiCommandBufferDescriptor = new();
                fixed (byte* labelPtr = FFI.WGPUStringView.ConvertWGPUStringParts(commandBufferDescriptor.Label, out var length))
                {
                    ffiCommandBufferDescriptor.Label = new(labelPtr, length);
                    return new(new(FFI.WGPUCommandEncoder.wgpuCommandEncoderFinish(Native.GetPtr(), &ffiCommandBufferDescriptor)));
                }
            }
            else
            {
                return new(new(FFI.WGPUCommandEncoder.wgpuCommandEncoderFinish(Native.GetPtr(), null)));
            }
        }
    }
    public WebGPUComputePassEncoder BeginComputePass(WebGPUComputePassDescriptor? computePassDescriptor = null)
    {
        unsafe
        {
            if (computePassDescriptor is not null)
            {
                FFI.WGPUComputePassDescriptor ffiComputePassDescriptor = new();
                FFI.WGPUPassTimestampWrites passTimestampWrites = new();

                fixed (byte* labelPtr = FFI.WGPUStringView.ConvertWGPUStringParts(computePassDescriptor.Label, out var length))
                {
                    ffiComputePassDescriptor.Label = new(labelPtr, length);
                    if (computePassDescriptor.TimestampWrites is not null)
                    {
                        passTimestampWrites.QuerySet = computePassDescriptor.TimestampWrites.QuerySet.Native.GetPtr();
                        passTimestampWrites.BeginningOfPassWriteIndex = computePassDescriptor.TimestampWrites.BeginningOfPassWriteIndex ?? FFI.Webgpu.WGPU_QUERY_SET_INDEX_UNDEFINED;
                        passTimestampWrites.EndOfPassWriteIndex = computePassDescriptor.TimestampWrites.EndOfPassWriteIndex ?? FFI.Webgpu.WGPU_QUERY_SET_INDEX_UNDEFINED;
                        ffiComputePassDescriptor.TimestampWrites = &passTimestampWrites;
                    }
                    return new(new(FFI.WGPUCommandEncoder.wgpuCommandEncoderBeginComputePass(Native.GetPtr(), null)));
                }
            }
            else
            {
                return new(new(FFI.WGPUCommandEncoder.wgpuCommandEncoderBeginComputePass(Native.GetPtr(), null)));
            }
        }
    }
    public void BeginRenderPass()
    {
        FFI.WGPUCommandEncoder.wgpuCommandEncoderBeginRenderPass(Native.GetPtr());
    }
    public void CopyBufferToBuffer()
    {
        FFI.WGPUCommandEncoder.wgpuCommandEncoderCopyBufferToBuffer(Native.GetPtr());
    }
    public void CopyBufferToTexture()
    {
        FFI.WGPUCommandEncoder.wgpuCommandEncoderCopyBufferToTexture(Native.GetPtr());
    }
    public void CopyTextureToBuffer()
    {
        FFI.WGPUCommandEncoder.wgpuCommandEncoderCopyTextureToBuffer(Native.GetPtr());
    }
    public void CopyTextureToTexture()
    {
        FFI.WGPUCommandEncoder.wgpuCommandEncoderCopyTextureToTexture(Native.GetPtr());
    }
    public void ClearBuffer()
    {
        FFI.WGPUCommandEncoder.wgpuCommandEncoderClearBuffer(Native.GetPtr());
    }
    public void InsertDebugMarker()
    {
        FFI.WGPUCommandEncoder.wgpuCommandEncoderInsertDebugMarker(Native.GetPtr());
    }
    public void PopDebugGroup()
    {
        FFI.WGPUCommandEncoder.wgpuCommandEncoderPopDebugGroup(Native.GetPtr());
    }
    public void PushDebugGroup()
    {
        FFI.WGPUCommandEncoder.wgpuCommandEncoderPushDebugGroup(Native.GetPtr());
    }
    public void ResolveQuerySet()
    {
        FFI.WGPUCommandEncoder.wgpuCommandEncoderResolveQuerySet(Native.GetPtr());
    }
    public void WriteTimestamp()
    {
        FFI.WGPUCommandEncoder.wgpuCommandEncoderWriteTimestamp(Native.GetPtr());
    }
    public void SetLabel()
    {
        FFI.WGPUCommandEncoder.wgpuCommandEncoderSetLabel(Native.GetPtr());
    }


}


[FFINote(typeof(FFI.WGPUCommandBufferDescriptor))]
public class WebGPUCommandBufferDescriptor
{
    public string Label = "";
}

[FFINote(typeof(FFI.WGPUCommandBuffer))]
public class WebGPUCommandBuffer
{
    internal WGPUObjectHolder<FFI.WGPUCommandBuffer> Native { get; }
    internal WebGPUCommandBuffer(WGPUObjectHolder<FFI.WGPUCommandBuffer> holder) { Native = holder; }
    public void Dispose() { Native.Dispose(); }
}

[FFINote(typeof(FFI.WGPUCommandEncoderDescriptor))]
public class WebGPUCommandEncoderDescriptor
{
    public string Label = "";
}
