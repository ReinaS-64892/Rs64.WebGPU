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
        return new()
        {
            Layout = Layout is not null ? Layout.Native.GetPtr() : null,
            Label = ffiMem.AllocateString(Label),
            Compute = Compute.ToF(ffiMem)
        };
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

[FFINote(typeof(FFI.WGPUPipelineLayoutDescriptor))]
public class WebGPUPipelineLayoutDescriptor
{
    public string Label = "";
    public required WebGPUBindGroupLayout[] BindGroupLayouts;
    public uint ImmediateSize = 0;

    internal unsafe FFI.WGPUPipelineLayoutDescriptor ToF(FFIMemoryManager ffiMem)
    {
        var bindGroupLayoutsPtr = (FFI.WGPUBindGroupLayout**)ffiMem.stackArea.Allocate<IntPtr>(BindGroupLayouts.Length);
        for (var i = 0; BindGroupLayouts.Length > i; i += 1)
        {
            bindGroupLayoutsPtr[i] = BindGroupLayouts[i].Native.GetPtr();
        }
        return new()
        {
            Label = ffiMem.AllocateString(Label),
            BindGroupLayoutsCount = (nuint)BindGroupLayouts.Length,
            BindGroupLayouts = bindGroupLayoutsPtr,
            ImmediateSize = ImmediateSize,
        };
    }
}


[FFINote(typeof(FFI.WGPUComputeState))]
public class WebGPUComputeState
{
    public required WebGPUShaderModule Module;
    public string? EntryPoint = null;
    public WebGPUConstantEntry[] Constants = [];

    internal unsafe FFI.WGPUComputeState ToF(FFIMemoryManager ffiMem)
    {
        var constants = ffiMem.stackArea.Allocate<FFI.WGPUConstantEntry>(Constants.Length);
        for (var i = 0; Constants.Length > i; i += 1)
        {
            var mEntry = Constants[i];

            constants[i] = new()
            {
                Key = ffiMem.AllocateString(mEntry.Key),
                Value = mEntry.Value,
            };
        }
        return new()
        {
            Module = Module.Native.GetPtr(),
            EntryPoint = ffiMem.AllocateString(EntryPoint),
            ConstantsCount = (nuint)Constants.Length,
            Constants = constants
        };
    }
}

[FFINote(typeof(FFI.WGPUConstantEntry))]
public class WebGPUConstantEntry
{
    public string Key = "";
    public double Value;
}
