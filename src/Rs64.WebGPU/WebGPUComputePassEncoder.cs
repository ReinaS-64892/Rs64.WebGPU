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

    // TODO 
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
    public WebGPUQuerySet QuerySet;
    public uint? BeginningOfPassWriteIndex;
    public uint? EndOfPassWriteIndex;
}
