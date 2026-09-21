// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: BSD-3-Clause
// SPDX-FileCopyrightText: Copyright 2019-2023 WebGPU-Native developers
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Rs64.WebGPU.FFI;


/**
 * Nullable value defining a pointer+length view into a UTF-8 encoded string.
 *
 * Values passed into the API may use the special length value @ref WGPU_STRLEN
 * to indicate a null-terminated string.
 * Non-null values passed out of the API (for example as callback arguments)
 * always provide an explicit length and **may or may not be null-terminated**.
 *
 * Some inputs to the API accept null values. Those which do not accept null
 * values "default" to the empty string when null values are passed.
 *
 * Values are encoded as follows:
 * - `{NULL, WGPU_STRLEN}`: the null value.
 * - `{non_null_pointer, WGPU_STRLEN}`: a null-terminated string view.
 * - `{any, 0}`: the empty string.
 * - `{NULL, non_zero_length}`: not allowed (null dereference).
 * - `{non_null_pointer, non_zero_length}`: an explictly-sized string view with
 *   size `non_zero_length` (in bytes).
 *
 * For info on how this is used in various places, see \ref Strings.
 */
internal unsafe ref struct WGPUStringView
{
    // utf 8 string ... 
    public byte* StringData;
    public nuint Length;
    public WGPUStringView() { }
    public WGPUStringView(byte* ptr, int length) : this()
    {
        StringData = ptr;
        Length = (nuint)length;
    }
    public static WGPUStringView Null => new() { StringData = null, Length = Webgpu.WGPU_STRLEN };
    public static WGPUStringView Empty => new() { StringData = null, Length = 0 };

    internal static Span<byte> ConvertSpan(string? label)
    {
        if (label is null) { return Span<byte>.Empty; }
        return Encoding.UTF8.GetBytes(label);
    }
}
