// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Runtime.InteropServices;

namespace Rs64.WebGPU.FFI;

internal unsafe interface IWGPUObject<TWGPUObject>
where TWGPUObject : unmanaged
{
    static abstract void AddRef(TWGPUObject* ptr);
    static abstract void Release(TWGPUObject* ptr);

}
