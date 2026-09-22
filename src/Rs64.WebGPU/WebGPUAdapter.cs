// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;

namespace Rs64.WebGPU;

public class WebGPUAdapter : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPUAdapter> Native { get; }
    internal WebGPUAdapter(WGPUObjectHolder<FFI.WGPUAdapter> holder) { Native = holder; }
    public void Dispose() { Native.Dispose(); }





}
