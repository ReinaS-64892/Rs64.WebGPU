// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

namespace Rs64.WebGPU;


[System.Serializable]
public class RequestAdapterUnavailableException : System.Exception
{
    public RequestAdapterUnavailableException() { }
    public RequestAdapterUnavailableException(string message) : base(message) { }
    public RequestAdapterUnavailableException(string message, System.Exception inner) : base(message, inner) { }
}
