// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Rs64.WebGPU;

[FFINote(typeof(FFI.WGPUSurface))]
public class WebGPUSurface : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPUSurface> Native { get; }
    internal WebGPUSurface(WGPUObjectHolder<FFI.WGPUSurface> holder) { Native = holder; }
    public void Dispose() { Native.Dispose(); }


    public void Configure(WebGPUSurfaceConfiguration configuration)
    {
        unsafe
        {
            FFI.WGPUSurfaceConfiguration ffiSurfaceConfiguration = new()
            {
                Device = configuration.Device.Native.GetPtr(),
                Format = configuration.Format.ToF(),
                Usage = configuration.Usage.ToF(),
                Width = configuration.Width,
                Height = configuration.Height,

                AlphaMode = configuration.AlphaMode.ToF(),
                PresentMode = configuration.PresentMode.ToF(),
            };
            var viewFormats = configuration.ViewFormats.Select(f => new WebGPUTextureFormat?(f)).Select(WebGPUTextureFormatUtil.ToF).ToArray();
            fixed (FFI.WGPUTextureFormat* vfPtr = viewFormats)
            {
                ffiSurfaceConfiguration.ViewFormatsCount = (nuint)viewFormats.Length;
                ffiSurfaceConfiguration.ViewFormats = vfPtr;

                FFI.WGPUSurface.wgpuSurfaceConfigure(Native.GetPtr(), &ffiSurfaceConfiguration);
            }
        }
    }
    public WebGPUSurfaceCapabilities GetCapabilities(WebGPUAdapter adapter)
    {
        FFI.WGPUSurfaceCapabilities ffiSurfaceCapabilities = default;
        var surfaceCapabilities = new WebGPUSurfaceCapabilities();
        unsafe
        {
            FFI.WGPUSurface.wgpuSurfaceGetCapabilities(Native.GetPtr(), adapter.Native.GetPtr(), &ffiSurfaceCapabilities);

            surfaceCapabilities.Usages = ffiSurfaceCapabilities.Usages.ToW();
            surfaceCapabilities.Formats =
                new Span<FFI.WGPUTextureFormat>(ffiSurfaceCapabilities.Formats, (int)ffiSurfaceCapabilities.FormatsCount)
                .ToArray()
                .Select(WebGPUTextureFormatUtil.ToW)
                .OfType<WebGPUTextureFormat>()
                .ToArray();
            surfaceCapabilities.PresentModes =
                new Span<FFI.WGPUPresentMode>(ffiSurfaceCapabilities.PresentModes, (int)ffiSurfaceCapabilities.PresentModesCount)
                .ToArray()
                .Select(WebGPUPresentModeUtil.ToW)
                .OfType<WebGPUPresentMode>()
                .ToArray();
            surfaceCapabilities.AlphaModes =
                new Span<FFI.WGPUCompositeAlphaMode>(ffiSurfaceCapabilities.AlphaModes, (int)ffiSurfaceCapabilities.AlphaModesCount)
                .ToArray()
                .Select(WebGPUCompositeAlphaModeUtil.ToW)
                .OfType<WebGPUCompositeAlphaMode>()
                .ToArray();

            FFI.WGPUSurfaceCapabilities.FreeMembers(ref ffiSurfaceCapabilities);
        }
        return surfaceCapabilities;
    }

    public WebGPUSurfaceTexture GetCurrentTexture()
    {
        FFI.WGPUSurfaceTexture ffiSurfaceTexture = default;
        var currentSurfaceTexture = new WebGPUSurfaceTexture();
        unsafe
        {
            FFI.WGPUSurface.wgpuSurfaceGetCurrentTexture(Native.GetPtr(), &ffiSurfaceTexture);

            currentSurfaceTexture.Texture = new(new(ffiSurfaceTexture.Texture));
            currentSurfaceTexture.Status = ffiSurfaceTexture.Status.ToW();
        }
        return currentSurfaceTexture;
    }
    public WebGPUStatus Present()
    {
        unsafe
        {
            return FFI.WGPUSurface.wgpuSurfacePresent(Native.GetPtr()).ToW();
        }
    }
    public void Unconfigure()
    {
        unsafe
        {
            FFI.WGPUSurface.wgpuSurfaceUnconfigure(Native.GetPtr());
        }
    }
    public void SetLabel(string label)
    {
        unsafe
        {
            fixed (byte* labelPtr = FFI.WGPUStringView.ConvertWGPUStringParts(label, out var length))
                FFI.WGPUSurface.wgpuSurfaceSetLabel(Native.GetPtr(), new(labelPtr, length));
        }
    }
}

[FFINote(typeof(FFI.WGPUSurfaceCapabilities))]
public class WebGPUSurfaceCapabilities
{
    internal WebGPUSurfaceCapabilities() { }
    public WebGPUTextureUsage Usages { get; internal set; }
    public WebGPUTextureFormat[] Formats { get; internal set; } = [];
    public WebGPUPresentMode[] PresentModes { get; internal set; } = [];
    public WebGPUCompositeAlphaMode[] AlphaModes { get; internal set; } = [];
}

[FFINote(typeof(FFI.WGPUSurfaceConfiguration))]
public class WebGPUSurfaceConfiguration
{
    public required WebGPUDevice Device;
    public required WebGPUTextureFormat? Format;
    public WebGPUTextureUsage Usage = WebGPUTextureUsage.RenderAttachment;
    public uint Width;
    public uint Height;
    public WebGPUTextureFormat[] ViewFormats = [];
    public WebGPUCompositeAlphaMode AlphaMode = WebGPUCompositeAlphaMode.Auto;
    public WebGPUPresentMode? PresentMode;
}
[FFINote(typeof(FFI.WGPUSurfaceTexture))]
public class WebGPUSurfaceTexture : IDisposable
{
    internal WebGPUSurfaceTexture() { }
    public WebGPUTexture Texture { get; internal set; } = null!;
    public WebGPUSurfaceGetCurrentTextureStatus Status { get; internal set; }

    public void Dispose()
    {
        Texture.Dispose();
    }
}

[FFINote(typeof(FFI.WGPUTexture))]
public class WebGPUTexture : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPUTexture> Native { get; }
    internal WebGPUTexture(WGPUObjectHolder<FFI.WGPUTexture> holder) { Native = holder; }
    public void Dispose() { Native.Dispose(); }
}
