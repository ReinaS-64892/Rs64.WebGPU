// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: BSD-3-Clause
// SPDX-FileCopyrightText: Copyright 2019-2023 WebGPU-Native developers
using System;
using System.Runtime.InteropServices;

namespace Rs64.WebGPU.FFI;


/**
 * \defgroup ChainedStructures Chained Structures
 * \brief Structures used to extend descriptors.
 *
 */
internal unsafe ref struct WGPUChainedStruct
{
    public WGPUChainedStruct* Next;
    public WGPUSType StructType;
}
