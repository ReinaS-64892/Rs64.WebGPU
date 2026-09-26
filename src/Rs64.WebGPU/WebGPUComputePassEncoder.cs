// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;

namespace Rs64.WebGPU;

[FFINote(typeof(FFI.WGPUComputePassEncoder))]
public class WebGPUComputePassEncoder : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPUComputePassEncoder> Native { get; }
    internal WebGPUComputePassEncoder(WGPUObjectHolder<FFI.WGPUComputePassEncoder> holder) { Native = holder; }
    public void Dispose() { Native.Dispose(); }

    public void InsertDebugMarker(string markerLabel)
    {
        unsafe
        {
            fixed (byte* labelPtr = FFI.WGPUStringView.ConvertWGPUStringParts(markerLabel, out var ll))
                FFI.WGPUComputePassEncoder.wgpuComputePassEncoderInsertDebugMarker(Native.GetPtr(), new(labelPtr, ll));
        }
    }
    public void PopDebugGroup()
    {
        unsafe { FFI.WGPUComputePassEncoder.wgpuComputePassEncoderPopDebugGroup(Native.GetPtr()); }
    }
    public void PushDebugGroup(string groupLabel)
    {
        unsafe
        {
            fixed (byte* labelPtr = FFI.WGPUStringView.ConvertWGPUStringParts(groupLabel, out var ll))
                FFI.WGPUComputePassEncoder.wgpuComputePassEncoderPushDebugGroup(Native.GetPtr(), new(labelPtr, ll));
        }
    }
    public void SetPipeline(WebGPUComputePipeline computePipeline)
    {
        unsafe { FFI.WGPUComputePassEncoder.wgpuComputePassEncoderSetPipeline(Native.GetPtr(), computePipeline.Native.GetPtr()); }
    }
    public void SetBindGroup(uint groupIndex, WebGPUBindGroup? bindGroup, uint[] dynamicOffsets)
    {
        unsafe
        {
            FFI.WGPUBindGroup* bg = bindGroup is not null ? bindGroup.Native.GetPtr() : null;
            fixed (uint* offsetPtr = dynamicOffsets)
                FFI.WGPUComputePassEncoder.wgpuComputePassEncoderSetBindGroup(Native.GetPtr(),
                    groupIndex,
                    bg,
                    (nuint)dynamicOffsets.Length,
                    offsetPtr
                );
        }
    }
    public void SetImmediates(uint offset, ReadOnlySpan<byte> data)
    {
        unsafe
        {
            fixed (byte* dataPtr = data)
                FFI.WGPUComputePassEncoder.wgpuComputePassEncoderSetImmediates(Native.GetPtr(), offset, dataPtr, (nuint)data.Length);
        }
    }
    public void DispatchWorkgroups(uint workgroupCountX, uint workgroupCountY, uint workgroupCountZ)
    {
        unsafe
        {
            FFI.WGPUComputePassEncoder.wgpuComputePassEncoderDispatchWorkgroups(Native.GetPtr(), workgroupCountX, workgroupCountY, workgroupCountZ);
        }
    }
    public void DispatchWorkgroupsIndirect(WebGPUBuffer buffer, ulong offset)
    {
        unsafe
        {
            FFI.WGPUComputePassEncoder.wgpuComputePassEncoderDispatchWorkgroupsIndirect(Native.GetPtr(), buffer.Native.GetPtr(), offset);
        }
    }
    public void End()
    {
        unsafe { FFI.WGPUComputePassEncoder.wgpuComputePassEncoderEnd(Native.GetPtr()); }
    }
    public void SetLabel(string label)
    {
        unsafe
        {
            fixed (byte* lPtr = FFI.WGPUStringView.ConvertWGPUStringParts(label, out var ll))
                FFI.WGPUComputePassEncoder.wgpuComputePassEncoderSetLabel(Native.GetPtr(), new(lPtr, ll));
        }
    }
}

[FFINote(typeof(FFI.WGPUComputePassDescriptor))]
public class WebGPUComputePassDescriptor
{
    public string Label = "";
    public WebGPUPassTimestampWrites? TimestampWrites;
}
[FFINote(typeof(FFI.WGPUPassTimestampWrites))]
public class WebGPUPassTimestampWrites
{
    public required WebGPUQuerySet QuerySet;
    public uint? BeginningOfPassWriteIndex;
    public uint? EndOfPassWriteIndex;

    internal void Write(ref FFI.WGPUPassTimestampWrites ffi)
    {
        unsafe
        {
            ffi.QuerySet = QuerySet.Native.GetPtr();
            ffi.BeginningOfPassWriteIndex = BeginningOfPassWriteIndex ?? FFI.Webgpu.WGPU_QUERY_SET_INDEX_UNDEFINED;
            ffi.EndOfPassWriteIndex = EndOfPassWriteIndex ?? FFI.Webgpu.WGPU_QUERY_SET_INDEX_UNDEFINED;
        }
    }
}
