// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: BSD-3-Clause
// SPDX-FileCopyrightText: Copyright 2019-2023 WebGPU-Native developers

using System;
using System.Runtime.InteropServices;

namespace Rs64.WebGPU.FFI;

internal struct WGPUBool
{
   uint _value = 0u;
   public WGPUBool(bool val)
   {
      _value = val ? 1u : 0u;
   }
   public WGPUBool(uint val)
   {
      _value = val;
   }

   public static explicit operator bool(WGPUBool val) => val._value is not 0;
   public static explicit operator WGPUBool(bool val) => new(val);

   /**
   * 'True' value of @ref WGPUBool.
   *
   * @remark It's not usually necessary to use this, as `true` (from
   * `stdbool.h` or C++) casts to the same value.
   */
   public static WGPUBool WGPU_TRUE => new(1);
   /**
    * 'False' value of @ref WGPUBool.
    *
    * @remark It's not usually necessary to use this, as `false` (from
    * `stdbool.h` or C++) casts to the same value.
    */
   public static WGPUBool WGPU_FALSE => new(0);
}
