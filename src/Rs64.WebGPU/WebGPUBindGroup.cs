// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;

namespace Rs64.WebGPU;

[FFINote(typeof(FFI.WGPUBindGroupDescriptor))]
public class WebGPUBindGroupDescriptor
{
    public string Label = "";
    public required WebGPUBindGroupLayout Layout;
    public required WebGPUBindGroupEntry[] Entries;

    internal unsafe FFI.WGPUBindGroupDescriptor ToF(FFIMemoryManager ffiMem)
    {
        var ffiEntries = ffiMem.AllocateArea<FFI.WGPUBindGroupEntry>(Entries.Length);
        for (var i = 0; Entries.Length > i; i += 1)
        {
            ffiEntries[i] = WebGPUBindGroupEntry.ToF(Entries[i]);
        }
        return new FFI.WGPUBindGroupDescriptor()
        {
            Label = ffiMem.AllocateString(Label),
            Layout = Layout.Native.GetPtr(),
            EntriesCount = (nuint)Entries.Length,
            Entries = ffiEntries
        };
    }
}
[FFINote(typeof(FFI.WGPUBindGroupEntry))]
public class WebGPUBindGroupEntry
{
    public uint Binding;
    public required BindResource Resource;

    public abstract class BindResource { }
    public class BindBuffer : BindResource
    {
        public required WebGPUBuffer Buffer;
        public ulong Offset;
        public ulong? Size = null;
    }
    public class BindSampler : BindResource
    {
        public required WebGPUSampler Sampler;
    }
    public class BindTextureView : BindResource
    {
        public required WebGPUTextureView TextureView;
    }

    internal static unsafe FFI.WGPUBindGroupEntry ToF(WebGPUBindGroupEntry webGPUBindGroupEntry)
    {
        FFI.WGPUBindGroupEntry entry = new();

        entry.Binding = webGPUBindGroupEntry.Binding;

        switch (webGPUBindGroupEntry.Resource)
        {
            default: throw new Exception();
            case BindBuffer buffer:
                {
                    entry.Buffer = buffer.Buffer.Native.GetPtr();

                    entry.Offset = buffer.Offset;

                    if (buffer.Size is not null)
                        entry.Size = buffer.Size.Value;

                    break;
                }
            case BindSampler sampler:
                {
                    entry.Sampler = sampler.Native.GetPtr();
                    break;
                }
            case BindTextureView textureView:
                {
                    entry.TextureView = textureView.Native.GetPtr();
                    break;
                }
        }
        return entry;
    }
}
[FFINote(typeof(FFI.WGPUBindGroupLayout))]
public class WebGPUBindGroupLayout : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPUBindGroupLayout> Native { get; }
    internal WebGPUBindGroupLayout(WGPUObjectHolder<FFI.WGPUBindGroupLayout> holder) { Native = holder; }
    public void Dispose() { Native.Dispose(); }

    // TODO 
}
[FFINote(typeof(FFI.WGPUBindGroup))]
public class WebGPUBindGroup : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPUBindGroup> Native { get; }
    internal WebGPUBindGroup(WGPUObjectHolder<FFI.WGPUBindGroup> holder) { Native = holder; }
    public void Dispose() { Native.Dispose(); }
}

[FFINote(typeof(FFI.WGPUBindGroupLayoutDescriptor))]
public class WebGPUBindGroupLayoutDescriptor
{
    public string Label = "";
    public required WebGPUBindGroupLayoutEntry[] Entries;
    internal unsafe FFI.WGPUBindGroupLayoutDescriptor ToF(FFIMemoryManager ffiMem)
    {
        var ffiEntries = ffiMem.AllocateArea<FFI.WGPUBindGroupLayoutEntry>(Entries.Length);
        for (var i = 0; Entries.Length > i; i += 1)
        {
            ffiEntries[i] = WebGPUBindGroupLayoutEntry.ToF(Entries[i]);
        }
        return new FFI.WGPUBindGroupLayoutDescriptor()
        {
            Label = ffiMem.AllocateString(Label),
            EntriesCount = (nuint)Entries.Length,
            Entries = ffiEntries
        };

    }
}
[FFINote(typeof(FFI.WGPUBindGroupLayoutEntry))]
public class WebGPUBindGroupLayoutEntry
{
    public uint Binding;
    public WebGPUShaderStage Visibility = WebGPUShaderStage.None;
    public uint BindingArraySize;
    public required WebGPUAbstractBufferBindingLayout BindingLayout;
    internal static FFI.WGPUBindGroupLayoutEntry ToF(WebGPUBindGroupLayoutEntry webGPUBindGroupLayoutEntry)
    {
        var ffiBindGroupLayoutEntry = new FFI.WGPUBindGroupLayoutEntry();
        ffiBindGroupLayoutEntry.Binding = webGPUBindGroupLayoutEntry.Binding;
        ffiBindGroupLayoutEntry.Visibility = webGPUBindGroupLayoutEntry.Visibility.ToF();
        ffiBindGroupLayoutEntry.BindingArraySize = webGPUBindGroupLayoutEntry.BindingArraySize;

        switch (webGPUBindGroupLayoutEntry.BindingLayout)
        {
            default: throw new Exception();
            case WebGPUBufferBindingLayout bufferBindingLayout:
                {
                    ffiBindGroupLayoutEntry.Buffer = bufferBindingLayout.ToF();
                    break;
                }
            case WebGPUSamplerBindingLayout samplerBindingLayout:
                {
                    ffiBindGroupLayoutEntry.Sampler = samplerBindingLayout.ToF();
                    break;
                }
            case WebGPUTextureBindingLayout textureBindingLayout:
                {
                    ffiBindGroupLayoutEntry.Texture = textureBindingLayout.ToF();
                    break;
                }
            case WebGPUStorageTextureBindingLayout storageTextureBindingLayout:
                {
                    ffiBindGroupLayoutEntry.StorageTexture = storageTextureBindingLayout.ToF();
                    break;
                }
        }
        return ffiBindGroupLayoutEntry;
    }
}

public abstract class WebGPUAbstractBufferBindingLayout { }
[FFINote(typeof(FFI.WGPUBufferBindingLayout))]
public class WebGPUBufferBindingLayout : WebGPUAbstractBufferBindingLayout
{
    public WebGPUBufferBindingType Type;
    public bool HasDynamicOffset = false;
    public ulong MinBindingSize = 0;

    public WebGPUBufferBindingLayout() { }
    internal FFI.WGPUBufferBindingLayout ToF()
    {
        return new FFI.WGPUBufferBindingLayout
        {
            Type = new WebGPUBufferBindingType?(Type).ToF(),
            HasDynamicOffset = (FFI.WGPUBool)HasDynamicOffset,
            MinBindingSize = MinBindingSize
        };
    }
}

[FFINote(typeof(FFI.WGPUSamplerBindingLayout))]
public class WebGPUSamplerBindingLayout : WebGPUAbstractBufferBindingLayout
{
    public WebGPUSamplerBindingType Type;

    public WebGPUSamplerBindingLayout()
    {
    }

    internal FFI.WGPUSamplerBindingLayout ToF()
    {
        return new FFI.WGPUSamplerBindingLayout
        {
            Type = new WebGPUSamplerBindingType?(Type).ToF(),
        };
    }
}
[FFINote(typeof(FFI.WGPUTextureBindingLayout))]
public class WebGPUTextureBindingLayout : WebGPUAbstractBufferBindingLayout
{
    public WebGPUTextureSampleType SampleType;
    public WebGPUTextureViewDimension? ViewDimension;

    public bool Multisampled = false;

    public WebGPUTextureBindingLayout() { }

    internal FFI.WGPUTextureBindingLayout ToF()
    {
        return new FFI.WGPUTextureBindingLayout
        {
            SampleType = new WebGPUTextureSampleType?(SampleType).ToF(),
            ViewDimension = ViewDimension.ToF(),
            Multisampled = (FFI.WGPUBool)Multisampled,
        };
    }
}


[FFINote(typeof(FFI.WGPUStorageTextureBindingLayout))]
public class WebGPUStorageTextureBindingLayout : WebGPUAbstractBufferBindingLayout
{
    public WebGPUStorageTextureAccess Access;
    public WebGPUTextureFormat? Format;
    public WebGPUTextureViewDimension? ViewDimension;

    public WebGPUStorageTextureBindingLayout() { }
    internal FFI.WGPUStorageTextureBindingLayout ToF()
    {
        return new FFI.WGPUStorageTextureBindingLayout
        {
            Access = new WebGPUStorageTextureAccess?(Access).ToF(),
            Format = Format.ToF(),
            ViewDimension = ViewDimension.ToF(),
        };
    }
}
