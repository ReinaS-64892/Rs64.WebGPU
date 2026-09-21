// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

namespace Rs64.WebGPU;

[System.Serializable]
public class StackAreaOverflowException : System.Exception
{
    public StackAreaOverflowException() { }
    public StackAreaOverflowException(string message) : base(message) { }
    public StackAreaOverflowException(string message, System.Exception inner) : base(message, inner) { }
}
