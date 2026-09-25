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
[System.Serializable]
public class RequestAdapterErrorException : System.Exception
{
    public RequestAdapterErrorException() { }
    public RequestAdapterErrorException(string message) : base(message) { }
    public RequestAdapterErrorException(string message, System.Exception inner) : base(message, inner) { }
}
[System.Serializable]
public class RequestDeviceErrorException : System.Exception
{
    public RequestDeviceErrorException() { }
    public RequestDeviceErrorException(string message) : base(message) { }
    public RequestDeviceErrorException(string message, System.Exception inner) : base(message, inner) { }
}
