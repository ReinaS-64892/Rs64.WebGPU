// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Runtime.InteropServices;

namespace Rs64.WebGPU;

internal static class WebGPUStringUtil
{
    public static string? ReadStringView(this FFI.WGPUStringView stringView)
    {
        unsafe
        {
            if (stringView.StringData is not null && stringView.Length is not 0)
            {
                if (stringView.Length == FFI.Webgpu.WGPU_STRLEN)
                {
                    return Marshal.PtrToStringUTF8((nint)stringView.StringData);
                }
                else
                {
                    return Marshal.PtrToStringUTF8((nint)stringView.StringData, (int)stringView.Length);
                }
            }
        }
        return null;
    }
}
