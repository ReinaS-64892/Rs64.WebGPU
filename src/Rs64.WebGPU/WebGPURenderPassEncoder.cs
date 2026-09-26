// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;

namespace Rs64.WebGPU;

[FFINote(typeof(FFI.WGPURenderPassEncoder))]
public class WebGPURenderPassEncoder : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPURenderPassEncoder> Native { get; }
    internal WebGPURenderPassEncoder(WGPUObjectHolder<FFI.WGPURenderPassEncoder> holder) { Native = holder; }
    public void Dispose() { Native.Dispose(); }

    // TODO 
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

[FFINote(typeof(FFI.WGPUTextureView))]
public class WebGPUTextureView : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPUTextureView> Native { get; }
    internal WebGPUTextureView(WGPUObjectHolder<FFI.WGPUTextureView> holder) { Native = holder; }
    public void Dispose() { Native.Dispose(); }
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
