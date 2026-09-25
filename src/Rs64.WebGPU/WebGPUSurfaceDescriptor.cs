// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Runtime.CompilerServices;

namespace Rs64.WebGPU;

[FFINote(typeof(FFI.WGPUSurfaceDescriptor))]
public abstract class WebGPUSurfaceDescriptor
{
    public string? Label { get; set; }
    internal abstract unsafe FFI.WGPUChainedStruct* GetExtensionSurfaceSource(StackAllocAsFFIArea area);
}

// プラットフォーム固有になるらへんは ... 少なくとも単純に safe にすることは難しいよね ... 

[FFINote(typeof(FFI.WGPUSurfaceSourceWaylandSurface))]
public unsafe class WebGPUWaylandSurfaceDescriptor : WebGPUSurfaceDescriptor
{
    public void* Display;
    public void* Surface;

    internal override FFI.WGPUChainedStruct* GetExtensionSurfaceSource(StackAllocAsFFIArea area)
    {
        var ssWayland = area.Allocate<FFI.WGPUSurfaceSourceWaylandSurface>();
        ssWayland->NextInChain.StructType = FFI.WGPUSType.SurfaceSourceWaylandSurface;
        ssWayland->Display = Display;
        ssWayland->Surface = Surface;

        return (FFI.WGPUChainedStruct*)ssWayland;
    }
}
