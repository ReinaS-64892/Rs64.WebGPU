// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Runtime.InteropServices;

namespace Rs64.WebGPU.FFI;

internal interface IWGPUStructHaveFreeMembers<TWGPUStruct>
where TWGPUStruct : allows ref struct
{
    static abstract void FreeMembers(ref TWGPUStruct wgpuStruct);
}

