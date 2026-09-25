// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

namespace Rs64.WebGPU;

[FFINote(typeof(FFI.WGPURequestAdapterOptions))]
public class WebGPURequestAdapterOptions
{
    public WebGPUFeatureLevel? FeatureLevel;
    public WebGPUPowerPreference? PowerPreference;
    public bool ForceFallbackAdapter;
    public WebGPUBackendType? BackendType;
    public WebGPUSurface? CompatibleSurface;

}
