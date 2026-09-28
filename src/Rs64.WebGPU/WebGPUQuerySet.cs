// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;

namespace Rs64.WebGPU;

[FFINote(typeof(FFI.WGPUQuerySet))]
public class WebGPUQuerySet : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPUQuerySet> Native { get; }
    internal WebGPUQuerySet(WGPUObjectHolder<FFI.WGPUQuerySet> holder) { Native = holder; }
    public void Dispose() { Native.Dispose(); }

    public void SetLabel(string label)
    {
        unsafe
        {
            using var ffiMem = new FFIMemoryManager(stackalloc byte[8]);
            FFI.WGPUQuerySet.wgpuQuerySetSetLabel(Native.GetPtr(), ffiMem.AllocateString(label));
        }
    }
    public WebGPUQueryType GetQuerySetType()
    {
        unsafe { return FFI.WGPUQuerySet.wgpuQuerySetGetType(Native.GetPtr()).ToW(); }
    }
    public uint GetCount()
    {
        unsafe { return FFI.WGPUQuerySet.wgpuQuerySetGetCount(Native.GetPtr()); }
    }
    public void Destroy()
    {
        unsafe { FFI.WGPUQuerySet.wgpuQuerySetDestroy(Native.GetPtr()); }
    }
}

[FFINote(typeof(FFI.WGPUQuerySetDescriptor))]
public class WebGPUQuerySetDescriptor
{
    public string Label = "";
    public WebGPUQueryType Type;
    public uint Count;

    internal FFI.WGPUQuerySetDescriptor ToF(FFIMemoryManager ffiMem)
    {
        return new()
        {
            Label = ffiMem.AllocateString(Label),
            Type = Type.ToF(),
            Count = Count,
        };
    }
}
