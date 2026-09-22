// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

namespace Rs64.WebGPU;


[System.Serializable]
public class InvalidEnumValueException : System.Exception
{
    public InvalidEnumValueException() { }
    public InvalidEnumValueException(string message) : base(message) { }
    public InvalidEnumValueException(string message, System.Exception inner) : base(message, inner) { }
}
