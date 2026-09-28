// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;

namespace Rs64.WebGPU;

[FFINote(typeof(FFI.WGPUCommandEncoder))]
public class WebGPUCommandEncoder : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPUCommandEncoder> Native { get; }
    internal WebGPUCommandEncoder(WGPUObjectHolder<FFI.WGPUCommandEncoder> holder) { Native = holder; }
    public void Dispose() { Native.Dispose(); }

    public WebGPUCommandBuffer Finish(WebGPUCommandBufferDescriptor? commandBufferDescriptor = null)
    {
        unsafe
        {
            if (commandBufferDescriptor is not null)
            {
                FFI.WGPUCommandBufferDescriptor ffiCommandBufferDescriptor = new();
                fixed (byte* labelPtr = FFI.WGPUStringView.ConvertWGPUStringParts(commandBufferDescriptor.Label, out var length))
                {
                    ffiCommandBufferDescriptor.Label = new(labelPtr, length);
                    return new(new(FFI.WGPUCommandEncoder.wgpuCommandEncoderFinish(Native.GetPtr(), &ffiCommandBufferDescriptor)));
                }
            }
            else
            {
                return new(new(FFI.WGPUCommandEncoder.wgpuCommandEncoderFinish(Native.GetPtr(), null)));
            }
        }
    }
    public WebGPUComputePassEncoder BeginComputePass(WebGPUComputePassDescriptor? computePassDescriptor = null)
    {
        unsafe
        {
            if (computePassDescriptor is not null)
            {
                FFI.WGPUComputePassDescriptor ffiComputePassDescriptor = new();
                FFI.WGPUPassTimestampWrites passTimestampWrites = new();

                fixed (byte* labelPtr = FFI.WGPUStringView.ConvertWGPUStringParts(computePassDescriptor.Label, out var length))
                {
                    ffiComputePassDescriptor.Label = new(labelPtr, length);
                    if (computePassDescriptor.TimestampWrites is not null)
                    {
                        computePassDescriptor.TimestampWrites.Write(ref passTimestampWrites);
                        ffiComputePassDescriptor.TimestampWrites = &passTimestampWrites;
                    }
                    return new(new(FFI.WGPUCommandEncoder.wgpuCommandEncoderBeginComputePass(Native.GetPtr(), &ffiComputePassDescriptor)));
                }
            }
            else
            {
                return new(new(FFI.WGPUCommandEncoder.wgpuCommandEncoderBeginComputePass(Native.GetPtr(), null)));
            }
        }
    }
    public WebGPURenderPassEncoder BeginRenderPass(WebGPURenderPassDescriptor renderPassDescriptor)
    {
        unsafe
        {
            FFI.WGPURenderPassDescriptor ffiRenderPassDescriptor = new();
            FFI.WGPUPassTimestampWrites ffiTimestampWrites;
            FFI.WGPURenderPassDepthStencilAttachment ffiDepthStencilAttachment;

            var caPtr = stackalloc FFI.WGPURenderPassColorAttachment[renderPassDescriptor.ColorAttachments.Length];
            for (var i = 0; renderPassDescriptor.ColorAttachments.Length > i; i += 1)
            {
                caPtr[i] = renderPassDescriptor.ColorAttachments[i].ToF();
            }
            ffiRenderPassDescriptor.ColorAttachmentsCount = (nuint)renderPassDescriptor.ColorAttachments.Length;
            ffiRenderPassDescriptor.ColorAttachments = caPtr;
            fixed (byte* lPtr = FFI.WGPUStringView.ConvertWGPUStringParts(renderPassDescriptor.Label, out var ll))
            {
                ffiRenderPassDescriptor.Label = new(lPtr, ll);
                if (renderPassDescriptor.DepthStencilAttachment is not null)
                {
                    ffiDepthStencilAttachment = new()
                    {
                        View = renderPassDescriptor.DepthStencilAttachment.View.Native.GetPtr(),
                        DepthLoadOp = renderPassDescriptor.DepthStencilAttachment.DepthLoadOp.ToF(),
                        DepthStoreOp = renderPassDescriptor.DepthStencilAttachment.DepthStoreOp.ToF(),
                        DepthClearValue = renderPassDescriptor.DepthStencilAttachment.DepthClearValue.HasValue ? renderPassDescriptor.DepthStencilAttachment.DepthClearValue.Value : FFI.Webgpu.WGPU_DEPTH_CLEAR_VALUE_UNDEFINED,
                        DepthReadOnly = (FFI.WGPUBool)renderPassDescriptor.DepthStencilAttachment.DepthReadOnly,
                        StencilLoadOp = renderPassDescriptor.DepthStencilAttachment.StencilLoadOp.ToF(),
                        StencilStoreOp = renderPassDescriptor.DepthStencilAttachment.StencilStoreOp.ToF(),
                        StencilClearValue = renderPassDescriptor.DepthStencilAttachment.StencilClearValue,
                        StencilReadOnly = (FFI.WGPUBool)renderPassDescriptor.DepthStencilAttachment.StencilReadOnly,
                    };
                    ffiRenderPassDescriptor.DepthStencilAttachment = &ffiDepthStencilAttachment;
                }
                if (renderPassDescriptor.OcclusionQuerySet is not null)
                    ffiRenderPassDescriptor.OcclusionQuerySet = renderPassDescriptor.OcclusionQuerySet.Native.GetPtr();
                if (renderPassDescriptor.TimestampWrites is not null)
                {
                    ffiTimestampWrites = new();
                    renderPassDescriptor.TimestampWrites.Write(ref ffiTimestampWrites);
                    ffiRenderPassDescriptor.TimestampWrites = &ffiTimestampWrites;
                }
                return new(new(FFI.WGPUCommandEncoder.wgpuCommandEncoderBeginRenderPass(Native.GetPtr(), &ffiRenderPassDescriptor)));
            }
        }
    }
    public void CopyBufferToBuffer(
        WebGPUBuffer source, ulong sourceOffset,
        WebGPUBuffer destination, ulong destinationOffset,
        ulong size
    )
    {
        unsafe
        {
            FFI.WGPUCommandEncoder.wgpuCommandEncoderCopyBufferToBuffer(Native.GetPtr(),
                source.Native.GetPtr(), sourceOffset,
                destination.Native.GetPtr(), destinationOffset,
                size
            );
        }
    }
    public void CopyBufferToTexture(
        WebGPUTexelCopyBufferInfo source,
        WebGPUTexelCopyTextureInfo destination,
        WebGPUExtent3d copySize
    )
    {
        unsafe
        {
            var ffiSource = source.ToF();
            var ffiDestination = destination.ToF();
            var ffiCopySize = copySize.ToF();
            FFI.WGPUCommandEncoder.wgpuCommandEncoderCopyBufferToTexture(Native.GetPtr(),
                &ffiSource,
                &ffiDestination,
                &ffiCopySize
            );
        }
    }
    public void CopyTextureToBuffer(
        WebGPUTexelCopyTextureInfo source,
        WebGPUTexelCopyBufferInfo destination,
        WebGPUExtent3d copySize
    )
    {
        unsafe
        {
            var ffiSource = source.ToF();
            var ffiDestination = destination.ToF();
            var ffiCopySize = copySize.ToF();
            FFI.WGPUCommandEncoder.wgpuCommandEncoderCopyTextureToBuffer(Native.GetPtr(),
                &ffiSource,
                &ffiDestination,
                &ffiCopySize
            );
        }
    }
    public void CopyTextureToTexture(
        WebGPUTexelCopyTextureInfo source,
        WebGPUTexelCopyTextureInfo destination,
        WebGPUExtent3d copySize
    )
    {
        unsafe
        {
            var ffiSource = source.ToF();
            var ffiDestination = destination.ToF();
            var ffiCopySize = copySize.ToF();
            FFI.WGPUCommandEncoder.wgpuCommandEncoderCopyTextureToTexture(Native.GetPtr(),
                &ffiSource,
                &ffiDestination,
                &ffiCopySize
            );
        }
    }
    public void ClearBuffer(
        WebGPUBuffer buffer,
        ulong offset,
        ulong size
    )
    {
        unsafe { FFI.WGPUCommandEncoder.wgpuCommandEncoderClearBuffer(Native.GetPtr(), buffer.Native.GetPtr(), offset, size); }
    }
    public void InsertDebugMarker(string markerLabel)
    {
        unsafe
        {
            fixed (byte* lPtr = FFI.WGPUStringView.ConvertWGPUStringParts(markerLabel, out var ll))
                FFI.WGPUCommandEncoder.wgpuCommandEncoderInsertDebugMarker(Native.GetPtr(), new(lPtr, ll));
        }
    }
    public void PopDebugGroup()
    {
        unsafe { FFI.WGPUCommandEncoder.wgpuCommandEncoderPopDebugGroup(Native.GetPtr()); }
    }
    public void PushDebugGroup(string groupLabel)
    {
        unsafe
        {
            fixed (byte* lPtr = FFI.WGPUStringView.ConvertWGPUStringParts(groupLabel, out var ll))
                FFI.WGPUCommandEncoder.wgpuCommandEncoderPushDebugGroup(Native.GetPtr(), new(lPtr, ll));
        }
    }
    public void ResolveQuerySet(
        WebGPUQuerySet querySet,
        uint firstQuery,
        uint queryCount,
        WebGPUBuffer destination,
        ulong destinationOffset
    )
    {
        unsafe
        {
            FFI.WGPUCommandEncoder.wgpuCommandEncoderResolveQuerySet(Native.GetPtr(),
                querySet.Native.GetPtr(),
                firstQuery,
                queryCount,
                destination.Native.GetPtr(),
                destinationOffset
            );
        }
    }
    public void WriteTimestamp(
        WebGPUQuerySet querySet,
        uint queryIndex
    )
    {
        unsafe
        {
            FFI.WGPUCommandEncoder.wgpuCommandEncoderWriteTimestamp(Native.GetPtr(),
                querySet.Native.GetPtr(),
                queryIndex
            );
        }
    }
    public void SetLabel(string label)
    {
        unsafe
        {
            fixed (byte* lPtr = FFI.WGPUStringView.ConvertWGPUStringParts(label, out var ll))
                FFI.WGPUCommandEncoder.wgpuCommandEncoderSetLabel(Native.GetPtr(), new(lPtr, ll));
        }
    }


}

[FFINote(typeof(FFI.WGPUTexelCopyBufferInfo))]
public class WebGPUTexelCopyBufferInfo
{
    public WebGPUTexelCopyBufferLayout Layout;
    public required WebGPUBuffer Buffer;
    internal FFI.WGPUTexelCopyBufferInfo ToF()
    {
        unsafe
        {
            return new()
            {
                Layout = Layout.ToF(),
                Buffer = Buffer.Native.GetPtr(),
            };
        }
    }
}

[FFINote(typeof(FFI.WGPUTexelCopyBufferLayout))]
public struct WebGPUTexelCopyBufferLayout
{
    public ulong Offset;
    public uint? BytesPerRow;
    public uint? RowsPerImage;
    internal FFI.WGPUTexelCopyBufferLayout ToF()
    {
        return new()
        {
            Offset = Offset,
            BytesPerRow = BytesPerRow.HasValue ? BytesPerRow.Value : FFI.Webgpu.WGPU_COPY_STRIDE_UNDEFINED,
            RowsPerImage = RowsPerImage.HasValue ? RowsPerImage.Value : FFI.Webgpu.WGPU_COPY_STRIDE_UNDEFINED,
        };
    }
}

[FFINote(typeof(FFI.WGPUTexelCopyTextureInfo))]
public class WebGPUTexelCopyTextureInfo
{
    public required WebGPUTexture Texture;
    public uint MipLevel = 0;
    public WebGPUOrigin3d Origin;
    public WebGPUTextureAspect? Aspect;

    internal FFI.WGPUTexelCopyTextureInfo ToF()
    {
        unsafe
        {
            return new()
            {
                Texture = Texture.Native.GetPtr(),
                MipLevel = MipLevel,
                Origin = Origin.ToF(),
                Aspect = Aspect.ToF(),
            };
        }
    }
}

[FFINote(typeof(FFI.WGPUOrigin3d))]
public struct WebGPUOrigin3d
{
    public uint X;
    public uint Y;
    public uint Z;
    internal FFI.WGPUOrigin3d ToF() { return new() { X = X, Y = Y, Z = Z }; }
}

[FFINote(typeof(FFI.WGPUExtent3d))]
public struct WebGPUExtent3d
{
    public uint Width;
    public uint Height = 1;
    public uint DepthOrArrayLayers = 1;
    public WebGPUExtent3d() { }

    internal FFI.WGPUExtent3d ToF()
    {
        return new()
        {
            Width = Width,
            Height = Height,
            DepthOrArrayLayers = DepthOrArrayLayers,
        };
    }
}


[FFINote(typeof(FFI.WGPUCommandBufferDescriptor))]
public class WebGPUCommandBufferDescriptor
{
    public string Label = "";
}

[FFINote(typeof(FFI.WGPUCommandBuffer))]
public class WebGPUCommandBuffer
{
    internal WGPUObjectHolder<FFI.WGPUCommandBuffer> Native { get; }
    internal WebGPUCommandBuffer(WGPUObjectHolder<FFI.WGPUCommandBuffer> holder) { Native = holder; }
    public void Dispose() { Native.Dispose(); }

    // TODO 
}

[FFINote(typeof(FFI.WGPUCommandEncoderDescriptor))]
public class WebGPUCommandEncoderDescriptor
{
    public string Label = "";
    internal FFI.WGPUCommandEncoderDescriptor ToF(FFIMemoryManager ffiMem)
    {
        return new()
        {
            Label = ffiMem.AllocateString(Label),
        };
    }
}
