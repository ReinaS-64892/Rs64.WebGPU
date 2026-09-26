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

    // TODO 
}
