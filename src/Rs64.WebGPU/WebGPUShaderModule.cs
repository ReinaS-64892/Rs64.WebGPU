// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;

namespace Rs64.WebGPU;

[FFINote(typeof(FFI.WGPUShaderModule))]
public class WebGPUShaderModule : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPUShaderModule> Native { get; }
    internal WebGPUShaderModule(WGPUObjectHolder<FFI.WGPUShaderModule> holder) { Native = holder; }
    public void Dispose() { Native.Dispose(); }
    //TODO : 

}


[FFINote(typeof(FFI.WGPUShaderModuleDescriptor))]
public abstract class WebGPUShaderModuleDescriptor
{
    public string Label = "";

    internal abstract FFI.WGPUShaderModuleDescriptor ToF(FFIMemoryManager ffiMem);
}

[FFINote(typeof(FFI.WGPUShaderSourceSpirv))]
public class WebGPUSpirvShaderModuleDescriptor : WebGPUShaderModuleDescriptor
{
    public required uint[] Code;
    internal unsafe override FFI.WGPUShaderModuleDescriptor ToF(FFIMemoryManager ffiMem)
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
    internal unsafe override FFI.WGPUShaderModuleDescriptor ToF(FFIMemoryManager ffiMem)
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
