// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
namespace Rs64.WebGPU;

[FFINote(typeof(FFI.WGPUQueue))]
public class WebGPUQueue : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPUQueue> Native { get; }
    internal WebGPUQueue(WGPUObjectHolder<FFI.WGPUQueue> holder) { Native = holder; }
    public void Dispose() { Native.Dispose(); }

    //TODO : 

}
