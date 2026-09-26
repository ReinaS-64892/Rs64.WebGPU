// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;

namespace Rs64.WebGPU;

[FFINote(typeof(FFI.WGPURenderPipeline))]
public class WebGPURenderPipeline : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPURenderPipeline> Native { get; }
    internal WebGPURenderPipeline(WGPUObjectHolder<FFI.WGPURenderPipeline> holder) { Native = holder; }
    public void Dispose() { Native.Dispose(); }

}

[FFINote(typeof(FFI.WGPURenderPipelineDescriptor))]
public class WebGPURenderPipelineDescriptor
{
    internal FFI.WGPURenderPipelineDescriptor ToF(FFIMemoryManager ffiMem)
    {
        throw new NotImplementedException();
    }
}
