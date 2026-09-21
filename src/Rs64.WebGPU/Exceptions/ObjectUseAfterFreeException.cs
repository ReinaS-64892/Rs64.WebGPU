// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

namespace Rs64.WebGPU;

[System.Serializable]
public class ObjectUseAfterFreeException : System.Exception
{
    public ObjectUseAfterFreeException() { }
    public ObjectUseAfterFreeException(string message) : base(message) { }
    public ObjectUseAfterFreeException(string message, System.Exception inner) : base(message, inner) { }
}
