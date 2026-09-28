// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
namespace Rs64.WebGPU;

[FFINote(typeof(FFI.WGPURenderBundleEncoder))]
public class WebGPURenderBundleEncoder : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPURenderBundleEncoder> Native { get; }
    internal WebGPURenderBundle? ResultRenderBundle { get; private set; }
    bool _encodingStarted = false;
    internal WebGPURenderBundleEncoder(WGPUObjectHolder<FFI.WGPURenderBundleEncoder> holder) { Native = holder; }
    public void Dispose() { }

    // こやつのライフタイム管理は少々グチャッとしてる
    // Encoding を正しく一回始めてもらう必要がある (でないと WebGPURenderBundleEncoder が リーク)
    // Encoding が終わった後 GetBundle を使用し、所有権を受け取ってもらわないと困る (でないと WebGPURenderBundle がリーク)
    // まぁどちらにせと WGPUObjectHolder が最終的に回収してくれるしダブルフリーも問題はないので、最悪の事態にはならない。

    public IDisposable BeginEncoding(WebGPURenderBundleDescriptor? renderBundleDescriptor)
    {
        if (_encodingStarted) { throw new Exception(); }
        _encodingStarted = true;
        return new Encoding(this, Native, renderBundleDescriptor);
    }
    class Encoding : IDisposable
    {
        internal WebGPURenderBundleEncoder? Encoder { get; }
        internal WGPUObjectHolder<FFI.WGPURenderBundleEncoder> Native { get; }
        internal WebGPURenderBundleDescriptor? RenderBundleDescriptor { get; }

        internal Encoding(WebGPURenderBundleEncoder encoder, WGPUObjectHolder<FFI.WGPURenderBundleEncoder> holder, WebGPURenderBundleDescriptor? renderBundleDescriptor)
        {
            Encoder = encoder;
            Native = holder;
            RenderBundleDescriptor = renderBundleDescriptor;
        }

        public void Dispose()
        {
            if (Encoder is null) { return; }
            Encoder.SetBundle(Finish(RenderBundleDescriptor));
            Native.Dispose();
        }
        public void SetPipeline(WebGPURenderPipeline renderPipeline)
        {
            unsafe
            {
                FFI.WGPURenderBundleEncoder.wgpuRenderBundleEncoderSetPipeline(Native.GetPtr(), renderPipeline.Native.GetPtr());
            }
        }
        public void SetBindGroup(
            uint groupIndex,
            WebGPUBindGroup? group,
            ReadOnlySpan<uint> dynamicOffsets
        )
        {
            unsafe
            {
                fixed (uint* ptr = dynamicOffsets)
                    FFI.WGPURenderBundleEncoder.wgpuRenderBundleEncoderSetBindGroup(Native.GetPtr(),
                        groupIndex,
                        group is not null ? group.Native.GetPtr() : null,
                        (nuint)dynamicOffsets.Length,
                        ptr
                    );
            }
        }
        public void SetImmediates(
            uint offset,
            ReadOnlySpan<byte> data
        )
        {
            unsafe
            {
                fixed (byte* dataPtr = data)
                    FFI.WGPURenderBundleEncoder.wgpuRenderBundleEncoderSetImmediates(Native.GetPtr(),
                        offset,
                        dataPtr,
                        (nuint)data.Length
                    );
            }
        }
        public void Draw(
            uint vertexCount,
            uint instanceCount,
            uint firstVertex,
            uint firstInstance
        )
        {
            unsafe
            {
                FFI.WGPURenderBundleEncoder.wgpuRenderBundleEncoderDraw(Native.GetPtr(),
                    vertexCount,
                    instanceCount,
                    firstVertex,
                    firstInstance
                );
            }
        }
        public void DrawIndexed(
            uint indexCount,
            uint instanceCount,
            uint firstIndex,
            int baseVertex,
            uint firstInstance
        )
        {
            unsafe
            {
                FFI.WGPURenderBundleEncoder.wgpuRenderBundleEncoderDrawIndexed(Native.GetPtr(),
                    indexCount,
                    instanceCount,
                    firstIndex,
                    baseVertex,
                    firstInstance
                );
            }
        }
        public void DrawIndirect(
            WebGPUBuffer indirectBuffer,
            ulong indirectOffset
        )
        {
            unsafe
            {
                FFI.WGPURenderBundleEncoder.wgpuRenderBundleEncoderDrawIndirect(Native.GetPtr(),
                    indirectBuffer.Native.GetPtr(),
                    indirectOffset
                );
            }
        }
        public void DrawIndexedIndirect(
            WebGPUBuffer indirect_buffer,
            ulong indirectOffset
        )
        {
            unsafe { FFI.WGPURenderBundleEncoder.wgpuRenderBundleEncoderDrawIndexedIndirect(Native.GetPtr(), indirect_buffer.Native.GetPtr(), indirectOffset); }
        }
        public void InsertDebugMarker(string markerLabel = "")
        {
            unsafe
            {
                fixed (byte* lp = FFI.WGPUStringView.ConvertWGPUStringParts(markerLabel, out var ll))
                    FFI.WGPURenderBundleEncoder.wgpuRenderBundleEncoderInsertDebugMarker(Native.GetPtr(), new(lp, ll));
            }
        }
        public void PopDebugGroup()
        {
            unsafe { FFI.WGPURenderBundleEncoder.wgpuRenderBundleEncoderPopDebugGroup(Native.GetPtr()); }
        }
        public void PushDebugGroup(string groupLabel = "")
        {
            unsafe
            {
                fixed (byte* lp = FFI.WGPUStringView.ConvertWGPUStringParts(groupLabel, out var ll))
                    FFI.WGPURenderBundleEncoder.wgpuRenderBundleEncoderPushDebugGroup(Native.GetPtr(), new(lp, ll));
            }
        }
        public void SetVertexBuffer(
            uint slot,
            WebGPUBuffer? buffer,
            ulong offset,
            ulong size
        )
        {
            unsafe
            {
                FFI.WGPURenderBundleEncoder.wgpuRenderBundleEncoderSetVertexBuffer(Native.GetPtr(),
                    slot,
                    buffer is not null ? buffer.Native.GetPtr() : null,
                    offset,
                    size
                );
            }
        }
        public void SetIndexBuffer(
            WebGPUBuffer buffer,
            WebGPUIndexFormat? format,
            ulong offset,
            ulong size
        )
        {
            unsafe
            {
                FFI.WGPURenderBundleEncoder.wgpuRenderBundleEncoderSetIndexBuffer(Native.GetPtr(),
                    buffer.Native.GetPtr(),
                    format.ToF(),
                    offset,
                    size
                );
            }
        }
        internal WebGPURenderBundle Finish(WebGPURenderBundleDescriptor? renderBundleDescriptor)
        {
            unsafe
            {
                if (renderBundleDescriptor is null)
                {
                    return new(new(FFI.WGPURenderBundleEncoder.wgpuRenderBundleEncoderFinish(Native.GetPtr(), null)));
                }
                else
                {
                    using var ffiMem = new FFIMemoryManager(stackalloc byte[16]);
                    var ffiDesc = renderBundleDescriptor.ToF(ffiMem);
                    return new(new(FFI.WGPURenderBundleEncoder.wgpuRenderBundleEncoderFinish(Native.GetPtr(), &ffiDesc)));
                }
            }
        }
    }

    private void SetBundle(WebGPURenderBundle renderBundle)
    {
        if (ResultRenderBundle is not null) { throw new Exception(); }
        ResultRenderBundle = renderBundle;
    }
    public WebGPURenderBundle GetBundle()
    {
        if (ResultRenderBundle is null) { throw new Exception(); }
        return ResultRenderBundle;
    }

    public void SetLabel(string label = "")
    {
        unsafe
        {
            fixed (byte* lp = FFI.WGPUStringView.ConvertWGPUStringParts(label, out var ll))
                FFI.WGPURenderBundleEncoder.wgpuRenderBundleEncoderSetLabel(Native.GetPtr(), new(lp, ll));
        }
    }
}

[FFINote(typeof(FFI.WGPURenderBundle))]
public class WebGPURenderBundle : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPURenderBundle> Native { get; }
    internal WebGPURenderBundle(WGPUObjectHolder<FFI.WGPURenderBundle> holder) { Native = holder; }
    public void Dispose() { Native.Dispose(); }

    public void SetLabel(string label = "")
    {
        unsafe
        {
            fixed (byte* lp = FFI.WGPUStringView.ConvertWGPUStringParts(label, out var ll))
                FFI.WGPURenderBundle.wgpuRenderBundleSetLabel(Native.GetPtr(), new(lp, ll));
        }
    }
}

[FFINote(typeof(FFI.WGPURenderBundleDescriptor))]
public class WebGPURenderBundleDescriptor
{
    public string Label = "";
    internal unsafe FFI.WGPURenderBundleDescriptor ToF(FFIMemoryManager ffiMem)
    {
        return new()
        {
            Label = ffiMem.AllocateString(Label),
        };
    }
}

[FFINote(typeof(FFI.WGPURenderBundleEncoderDescriptor))]
public class WebGPURenderBundleEncoderDescriptor
{
    public string Label = "";

    public required WebGPUTextureFormat?[] ColorFormats;

    public WebGPUTextureFormat? DepthStencilFormat;

    public uint SampleCount = 1;

    public bool DepthReadOnly = false;

    public bool StencilReadOnly = false;

    internal unsafe FFI.WGPURenderBundleEncoderDescriptor ToF(FFIMemoryManager ffiMem)
    {
        var colorFormats = ffiMem.AllocateArea<FFI.WGPUTextureFormat>(ColorFormats.Length);
        for (var i = 0; ColorFormats.Length > i; i += 1)
        {
            colorFormats[i] = ColorFormats[i].ToF();
        }
        return new()
        {
            Label = ffiMem.AllocateString(Label),
            ColorFormatsCount = (nuint)ColorFormats.Length,
            ColorFormats = colorFormats,
            DepthStencilFormat = DepthStencilFormat.ToF(),
            SampleCount = SampleCount,
            DepthReadOnly = (FFI.WGPUBool)DepthReadOnly,
            StencilReadOnly = (FFI.WGPUBool)StencilReadOnly,
        };
    }

}
