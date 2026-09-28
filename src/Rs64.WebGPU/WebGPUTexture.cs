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

    public WebGPUTextureView CreateView(WebGPUTextureViewDescriptor textureViewDescriptor)
    {
        unsafe
        {
            using var ffiMem = new FFIMemoryManager(stackalloc byte[16]);
            var ffiDesc = textureViewDescriptor.ToF(ffiMem);
            return new(new(FFI.WGPUTexture.wgpuTextureCreateView(Native.GetPtr(), &ffiDesc)));
        }
    }
    public void SetLabel(string label)
    {
        unsafe
        {
            using var ffiMem = new FFIMemoryManager(stackalloc byte[16]);
            FFI.WGPUTexture.wgpuTextureSetLabel(Native.GetPtr(), ffiMem.AllocateString(label));
        }
    }
    public uint GetWidth()
    {
        unsafe
        {
            return FFI.WGPUTexture.wgpuTextureGetWidth(Native.GetPtr());
        }
    }
    public uint GetHeight()
    {
        unsafe
        {
            return FFI.WGPUTexture.wgpuTextureGetHeight(Native.GetPtr());
        }
    }
    public uint GetDepthOrArrayLayers()
    {
        unsafe
        {
            return FFI.WGPUTexture.wgpuTextureGetDepthOrArrayLayers(Native.GetPtr());
        }
    }
    public uint GetMipLevelCount()
    {
        unsafe
        {
            return FFI.WGPUTexture.wgpuTextureGetMipLevelCount(Native.GetPtr());
        }
    }
    public uint GetSampleCount()
    {
        unsafe
        {
            return FFI.WGPUTexture.wgpuTextureGetSampleCount(Native.GetPtr());
        }
    }
    public WebGPUTextureDimension? GetDimension()
    {
        unsafe
        {
            return FFI.WGPUTexture.wgpuTextureGetDimension(Native.GetPtr()).ToW();
        }
    }
    public WebGPUTextureViewDimension? GetTextureBindingViewDimension()
    {
        unsafe
        {
            return FFI.WGPUTexture.wgpuTextureGetTextureBindingViewDimension(Native.GetPtr()).ToW();
        }
    }
    public WebGPUTextureFormat? GetFormat()
    {
        unsafe
        {
            return FFI.WGPUTexture.wgpuTextureGetFormat(Native.GetPtr()).ToW();
        }
    }
    public WebGPUTextureUsage GetUsage()
    {
        unsafe
        {
            return FFI.WGPUTexture.wgpuTextureGetUsage(Native.GetPtr()).ToW();
        }
    }
    public void Destroy()
    {
        unsafe
        {
            FFI.WGPUTexture.wgpuTextureDestroy(Native.GetPtr());
        }
    }


}

[FFINote(typeof(FFI.WGPUTextureView))]
public class WebGPUTextureView : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPUTextureView> Native { get; }
    internal WebGPUTextureView(WGPUObjectHolder<FFI.WGPUTextureView> holder) { Native = holder; }
    public void Dispose() { Native.Dispose(); }
    public void SetLabel(string label)
    {
        unsafe
        {
            using var ffiMem = new FFIMemoryManager(stackalloc byte[16]);
            FFI.WGPUTextureView.wgpuTextureViewSetLabel(Native.GetPtr(), ffiMem.AllocateString(label));
        }
    }
}
[FFINote(typeof(FFI.WGPUTextureViewDescriptor))]
public class WebGPUTextureViewDescriptor
{
    public string Label = "";
    public WebGPUTextureFormat? Format;
    public WebGPUTextureViewDimension? Dimension;
    public uint BaseMipLevel = 0;
    public uint? MipLevelCount = null;
    public uint BaseArrayLayer = 0;
    public uint? ArrayLayerCount = null;
    public WebGPUTextureAspect? Aspect;
    public WebGPUTextureUsage Usage = WebGPUTextureUsage.None;

    internal FFI.WGPUTextureViewDescriptor ToF(FFIMemoryManager ffiMem)
    {
        return new()
        {
            Label = ffiMem.AllocateString(Label),
            Format = Format.ToF(),
            Dimension = Dimension.ToF(),
            BaseMipLevel = BaseMipLevel,
            MipLevelCount = MipLevelCount ?? FFI.Webgpu.WGPU_MIP_LEVEL_COUNT_UNDEFINED,
            BaseArrayLayer = BaseArrayLayer,
            ArrayLayerCount = ArrayLayerCount ?? FFI.Webgpu.WGPU_ARRAY_LAYER_COUNT_UNDEFINED,
            Aspect = Aspect.ToF(),
            Usage = Usage.ToF(),
        };
    }

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
