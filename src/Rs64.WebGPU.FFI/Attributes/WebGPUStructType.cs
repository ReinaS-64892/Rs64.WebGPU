// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0
using System;
using System.Runtime.InteropServices;

namespace Rs64.WebGPU.FFI;

[System.AttributeUsage(System.AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
sealed class WebGPUStructTypeAttribute(WebGPUStructTypeAttribute.WebGPUStructType webGPUStructType) : System.Attribute
{
    public WebGPUStructType WebGPUStructType1 { get; } = webGPUStructType;

    public enum WebGPUStructType
    {
        Extensible,
        Extensible_Callback_Arg,
        Extension,
        Standalone,
    }
}
