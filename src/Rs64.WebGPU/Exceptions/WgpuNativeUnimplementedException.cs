// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

namespace Rs64.WebGPU;

[System.Serializable]
public class WgpuNativeUnimplementedException : System.Exception
{
    public WgpuNativeUnimplementedException() { }
    public WgpuNativeUnimplementedException(string message) : base(message) { }
    public WgpuNativeUnimplementedException(string message, System.Exception inner) : base(message, inner) { }
}
