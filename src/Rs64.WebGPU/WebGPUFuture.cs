// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Rs64.WebGPU;

internal struct WebGPUFuture(FFI.WGPUFuture wGPUFuture)
{
    public ulong FutureID = wGPUFuture.Id;
}
