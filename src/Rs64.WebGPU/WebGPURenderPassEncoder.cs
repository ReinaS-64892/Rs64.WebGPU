// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;

namespace Rs64.WebGPU;

[FFINote(typeof(FFI.WGPURenderPassEncoder))]
public class WebGPURenderPassEncoder : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPURenderPassEncoder> Native { get; }
    internal WebGPURenderPassEncoder(WGPUObjectHolder<FFI.WGPURenderPassEncoder> holder) { Native = holder; }
    public void Dispose()
    {
        End();
        Native.Dispose();
    }

    public void SetPipeline(WebGPURenderPipeline renderPipeline)
    {
        unsafe
        {
            FFI.WGPURenderPassEncoder.wgpuRenderPassEncoderSetPipeline(Native.GetPtr(), renderPipeline.Native.GetPtr());
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
                FFI.WGPURenderPassEncoder.wgpuRenderPassEncoderSetBindGroup(Native.GetPtr(),
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
                FFI.WGPURenderPassEncoder.wgpuRenderPassEncoderSetImmediates(Native.GetPtr(),
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
            FFI.WGPURenderPassEncoder.wgpuRenderPassEncoderDraw(Native.GetPtr(),
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
            FFI.WGPURenderPassEncoder.wgpuRenderPassEncoderDrawIndexed(Native.GetPtr(),
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
            FFI.WGPURenderPassEncoder.wgpuRenderPassEncoderDrawIndirect(Native.GetPtr(),
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
        unsafe { FFI.WGPURenderPassEncoder.wgpuRenderPassEncoderDrawIndexedIndirect(Native.GetPtr(), indirect_buffer.Native.GetPtr(), indirectOffset); }
    }
    public void ExecuteBundles(ReadOnlySpan<WebGPURenderBundle> renderBundles)
    {
        unsafe
        {
            var ffiBundles = stackalloc FFI.WGPURenderBundle*[renderBundles.Length];
            for (var i = 0; renderBundles.Length > i; i += 1)
                ffiBundles[i] = renderBundles[i].Native.GetPtr();
            FFI.WGPURenderPassEncoder.wgpuRenderPassEncoderExecuteBundles(Native.GetPtr(), (nuint)renderBundles.Length, ffiBundles);
        }
    }
    public void InsertDebugMarker(string markerLabel = "")
    {
        unsafe
        {
            fixed (byte* lp = FFI.WGPUStringView.ConvertWGPUStringParts(markerLabel, out var ll))
                FFI.WGPURenderPassEncoder.wgpuRenderPassEncoderInsertDebugMarker(Native.GetPtr(), new(lp, ll));
        }
    }
    public void PopDebugGroup()
    {
        unsafe
        {
            FFI.WGPURenderPassEncoder.wgpuRenderPassEncoderPopDebugGroup(Native.GetPtr());
        }
    }
    public void PushDebugGroup(string groupLabel = "")
    {
        unsafe
        {
            fixed (byte* lp = FFI.WGPUStringView.ConvertWGPUStringParts(groupLabel, out var ll))
                FFI.WGPURenderPassEncoder.wgpuRenderPassEncoderPushDebugGroup(Native.GetPtr(), new(lp, ll));
        }
    }
    public void SetStencilReference(uint reference)
    {
        unsafe
        {
            FFI.WGPURenderPassEncoder.wgpuRenderPassEncoderSetStencilReference(Native.GetPtr(), reference);
        }
    }
    public void SetBlendConstant(WebGPUColor color
    )
    {
        unsafe
        {
            var ffi = color.ToF();
            FFI.WGPURenderPassEncoder.wgpuRenderPassEncoderSetBlendConstant(Native.GetPtr(), &ffi);
        }
    }
    public void SetViewport(
        float x,
        float y,

        float width,
        float height,

        float min_depth,
        float max_depth
    )
    {
        unsafe
        {
            FFI.WGPURenderPassEncoder.wgpuRenderPassEncoderSetViewport(Native.GetPtr(),
                y,
                x,
                width,
                height,
                min_depth,
                max_depth
            );
        }
    }
    public void SetScissorRect(
        uint x,
        uint y,

        uint width,
        uint height
    )
    {
        unsafe
        {
            FFI.WGPURenderPassEncoder.wgpuRenderPassEncoderSetScissorRect(Native.GetPtr(),
                y,
                x,
                width,
                height
            );
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
            FFI.WGPURenderPassEncoder.wgpuRenderPassEncoderSetVertexBuffer(Native.GetPtr(),
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
            FFI.WGPURenderPassEncoder.wgpuRenderPassEncoderSetIndexBuffer(Native.GetPtr(),
                buffer.Native.GetPtr(),
                format.ToF(),
                offset,
                size
            );
        }
    }
    public void BeginOcclusionQuery(uint queryIndex)
    {
        unsafe
        {
            FFI.WGPURenderPassEncoder.wgpuRenderPassEncoderBeginOcclusionQuery(Native.GetPtr(), queryIndex);
        }
    }
    public void EndOcclusionQuery()
    {
        unsafe
        {
            FFI.WGPURenderPassEncoder.wgpuRenderPassEncoderEndOcclusionQuery(Native.GetPtr());
        }
    }
    public void End()
    {
        unsafe
        {
            FFI.WGPURenderPassEncoder.wgpuRenderPassEncoderEnd(Native.GetPtr());
        }
    }
    public void SetLabel(string label = "")
    {
        unsafe
        {
            fixed (byte* lp = FFI.WGPUStringView.ConvertWGPUStringParts(label, out var ll))
                FFI.WGPURenderPassEncoder.wgpuRenderPassEncoderSetLabel(Native.GetPtr(), new(lp, ll));
        }
    }

}


[FFINote(typeof(FFI.WGPURenderPassDescriptor))]
public class WebGPURenderPassDescriptor
{
    public string Label = "";

    public required WebGPURenderPassColorAttachment[] ColorAttachments;
    public WebGPURenderPassDepthStencilAttachment? DepthStencilAttachment;
    public WebGPUQuerySet? OcclusionQuerySet;
    public WebGPUPassTimestampWrites? TimestampWrites;
}


[FFINote(typeof(FFI.WGPURenderPassColorAttachment))]
public class WebGPURenderPassColorAttachment
{
    public WebGPUTextureView? View;
    public uint? DepthSlice;
    public WebGPUTextureView? ResolveTarget;
    public WebGPULoadOp? LoadOp;
    public WebGPUStoreOp? StoreOp;
    public WebGPUColor ClearValue;

    internal FFI.WGPURenderPassColorAttachment ToF()
    {
        unsafe
        {
            FFI.WGPURenderPassColorAttachment ffi = new();
            if (View is not null)
                ffi.View = View.Native.GetPtr();

            ffi.DepthSlice = DepthSlice.HasValue ? DepthSlice.Value : FFI.Webgpu.WGPU_DEPTH_SLICE_UNDEFINED;
            if (ResolveTarget is not null)
                ffi.ResolveTarget = ResolveTarget.Native.GetPtr();

            ffi.LoadOp = LoadOp.ToF();
            ffi.StoreOp = StoreOp.ToF();
            ffi.ClearValue = ClearValue.ToF();
            return ffi;
        }
    }
}


[FFINote(typeof(FFI.WGPURenderPassDepthStencilAttachment))]
public class WebGPURenderPassDepthStencilAttachment
{
    public required WebGPUTextureView View;
    public WebGPULoadOp? DepthLoadOp;
    public WebGPUStoreOp? DepthStoreOp;
    public float? DepthClearValue;
    public bool DepthReadOnly;
    public WebGPULoadOp? StencilLoadOp;
    public WebGPUStoreOp? StencilStoreOp;
    public uint StencilClearValue;
    public bool StencilReadOnly;
}
[FFINote(typeof(FFI.WGPUColor))]
public struct WebGPUColor
{
    public double R;
    public double G;
    public double B;
    public double A;

    internal FFI.WGPUColor ToF()
    {
        return new()
        {
            R = R,
            G = G,
            B = B,
            A = A,
        };
    }
}
