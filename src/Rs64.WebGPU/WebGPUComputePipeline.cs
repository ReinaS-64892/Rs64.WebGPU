// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Collections.Generic;

namespace Rs64.WebGPU;

[FFINote(typeof(FFI.WGPUComputePipeline))]
public class WebGPUComputePipeline : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPUComputePipeline> Native { get; }
    internal WebGPUComputePipeline(WGPUObjectHolder<FFI.WGPUComputePipeline> holder) { Native = holder; }
    public void Dispose() { Native.Dispose(); }

    // TODO 
}

[FFINote(typeof(FFI.WGPUComputePipelineDescriptor))]
public class WebGPUComputePipelineDescriptor
{
    public string Label = "";
    public WebGPUPipelineLayout? Layout;
    public required WebGPUComputeState Compute;

    internal unsafe FFI.WGPUComputePipelineDescriptor ToF(FFIMemoryManager ffiMem)
    {
        FFI.WGPUComputePipelineDescriptor ffiComputePipelineDescriptor = new();

        ffiComputePipelineDescriptor.Label = ffiMem.AllocateString(Label);

        if (Layout is not null)
            ffiComputePipelineDescriptor.Layout = Layout.Native.GetPtr();


        FFI.WGPUComputeState computeState = new();

        computeState.Module = Compute.Module.Native.GetPtr();
        computeState.EntryPoint = ffiMem.AllocateString(Compute.EntryPoint);
        var constants = ffiMem.stackArea.Allocate<FFI.WGPUConstantEntry>(Compute.Constants.Length);
        for (var i = 0; Compute.Constants.Length > i; i += 1)
        {
            var mEntry = Compute.Constants[i];

            constants[i] = new()
            {
                Key = ffiMem.AllocateString(mEntry.Key),
                Value = mEntry.Value,
            };
        }
        computeState.ConstantsCount = (nuint)Compute.Constants.Length;
        computeState.Constants = constants;

        ffiComputePipelineDescriptor.Compute = computeState;

        return ffiComputePipelineDescriptor;
    }
}

[FFINote(typeof(FFI.WGPUPipelineLayout))]
public class WebGPUPipelineLayout : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPUPipelineLayout> Native { get; }
    internal WebGPUPipelineLayout(WGPUObjectHolder<FFI.WGPUPipelineLayout> holder) { Native = holder; }
    public void Dispose() { Native.Dispose(); }

    // TODO 

}
[FFINote(typeof(FFI.WGPUComputeState))]
public class WebGPUComputeState
{
    public required WebGPUShaderModule Module;

    public string? EntryPoint = null;

    public WebGPUConstantEntry[] Constants = [];
}

[FFINote(typeof(FFI.WGPUConstantEntry))]
public class WebGPUConstantEntry
{
    public string Key = "";
    public double Value;
}

[FFINote(typeof(FFI.WGPUShaderModule))]
public class WebGPUShaderModule : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPUShaderModule> Native { get; }
    internal WebGPUShaderModule(WGPUObjectHolder<FFI.WGPUShaderModule> holder) { Native = holder; }
    public void Dispose() { Native.Dispose(); }
}
