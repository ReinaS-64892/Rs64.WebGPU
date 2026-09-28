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
