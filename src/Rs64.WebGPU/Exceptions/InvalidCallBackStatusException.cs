// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

namespace Rs64.WebGPU;
[System.Serializable]
public class InvalidCallBackStatusException : System.Exception
{
    public InvalidCallBackStatusException() { }
    public InvalidCallBackStatusException(string message) : base(message) { }
    public InvalidCallBackStatusException(string message, System.Exception inner) : base(message, inner) { }
}
