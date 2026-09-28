// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

namespace Rs64.WebGPU;

[System.Serializable]
public class InvalidObjectPointerException : System.Exception
{
    public InvalidObjectPointerException() { }
    public InvalidObjectPointerException(string message) : base(message) { }
    public InvalidObjectPointerException(string message, System.Exception inner) : base(message, inner) { }
}
