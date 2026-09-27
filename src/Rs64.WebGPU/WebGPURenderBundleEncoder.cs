// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
namespace Rs64.WebGPU;

[FFINote(typeof(FFI.WGPURenderBundleEncoder))]
public class WebGPURenderBundleEncoder : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPURenderBundleEncoder> Native { get; }
    internal WebGPURenderBundleEncoder(WGPUObjectHolder<FFI.WGPURenderBundleEncoder> holder) { Native = holder; }
    public void Dispose() { Native.Dispose(); }

    // TODO

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

}
