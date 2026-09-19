// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0
using System;
using System.Runtime.InteropServices;

namespace Rs64.WebGPU.FFI;

[System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = false)]
sealed class WebGPUNullableFloat32Attribute : System.Attribute
{
}

[System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = false)]
sealed class WebGPUFloat64SuperTypeAttribute : System.Attribute
{
}
