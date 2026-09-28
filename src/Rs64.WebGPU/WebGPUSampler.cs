// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;

namespace Rs64.WebGPU;

[FFINote(typeof(FFI.WGPUSampler))]
public class WebGPUSampler : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPUSampler> Native { get; }
    internal WebGPUSampler(WGPUObjectHolder<FFI.WGPUSampler> holder) { Native = holder; }
    public void Dispose() { Native.Dispose(); }

    //TODO : 
}

[FFINote(typeof(FFI.WGPUSamplerDescriptor))]
public class WebGPUSamplerDescriptor
{
    public string Label = "";
    public WebGPUAddressMode? AddressModeU;
    public WebGPUAddressMode? AddressModeV;
    public WebGPUAddressMode? AddressModeW;
    public WebGPUFilterMode? MagFilter;


    public WebGPUFilterMode? MinFilter;


    public WebGPUMipmapFilterMode? MipmapFilter;

    public float LodMinClamp = 0;
    public float LodMaxClamp = 32;


    public WebGPUCompareFunction? Compare;


    public ushort MaxAnisotropy = 1;

    internal FFI.WGPUSamplerDescriptor ToF(FFIMemoryManager ffiMem)
    {
        return new()
        {
            Label = ffiMem.AllocateString(Label),
            AddressModeU = AddressModeU.ToF(),
            AddressModeV = AddressModeV.ToF(),
            AddressModeW = AddressModeW.ToF(),
            MagFilter = MagFilter.ToF(),
            MinFilter = MinFilter.ToF(),
            MipmapFilter = MipmapFilter.ToF(),
            LodMinClamp = LodMinClamp,
            LodMaxClamp = LodMaxClamp,
            Compare = Compare.ToF(),
        };
    }
}
