// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;

namespace Rs64.WebGPU;

[FFINote(typeof(FFI.WGPUTexture))]
public class WebGPUTexture : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPUTexture> Native { get; }
    internal WebGPUTexture(WGPUObjectHolder<FFI.WGPUTexture> holder) { Native = holder; }
    public void Dispose() { Native.Dispose(); }
    //TODO : 
}

[FFINote(typeof(FFI.WGPUTextureDescriptor))]
public class WebGPUTextureDescriptor
{
    public string Label = "";
    public WebGPUTextureUsage Usage = WebGPUTextureUsage.None;
    public WebGPUTextureDimension? Dimension;
    public WebGPUExtent3d Size = new();
    public WebGPUTextureFormat? Format;

    public uint MipLevelCount = 1;
    public uint SampleCount = 1;
    public WebGPUTextureFormat?[] ViewFormats = [];
    internal unsafe virtual FFI.WGPUTextureDescriptor ToF(FFIMemoryManager ffiMem)
    {
        var viewFormats = ffiMem.AllocateArea<FFI.WGPUTextureFormat>(ViewFormats.Length);
        for (var i = 0; ViewFormats.Length > i; i += 1)
        {
            viewFormats[i] = ViewFormats[i].ToF();
        }
        return new()
        {
            Label = ffiMem.AllocateString(Label),
            Usage = Usage.ToF(),
            Dimension = Dimension.ToF(),
            Size = Size.ToF(),
            Format = Format.ToF(),
            MipLevelCount = MipLevelCount,
            SampleCount = SampleCount,
            ViewFormatsCount = (nuint)ViewFormats.Length,
            ViewFormats = viewFormats,
        };
    }
}
[FFINote(typeof(FFI.WGPUTextureBindingViewDimension))]
internal class WebGPUTextureDescriptorWithBindingViewDimension : WebGPUTextureDescriptor
{
    public WebGPUTextureViewDimension? TextureBindingViewDimension;
    internal unsafe override FFI.WGPUTextureDescriptor ToF(FFIMemoryManager ffiMem)
    {
        var ffi = base.ToF(ffiMem);
        var bvd = ffiMem.AllocateArea<FFI.WGPUTextureBindingViewDimension>();
        bvd->NextInChain.StructType = FFI.WGPUSType.TextureBindingViewDimension;
        bvd->TextureBindingViewDimension = TextureBindingViewDimension.ToF();
        ffi.NextInChain = (FFI.WGPUChainedStruct*)bvd;
        return ffi;
    }
}
