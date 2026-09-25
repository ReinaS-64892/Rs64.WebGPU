// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

namespace Rs64.WebGPU;

[System.Serializable]
public class CallBackErrorException : System.Exception
{
    public CallBackErrorException() { }
    public CallBackErrorException(string message) : base(message) { }
    public CallBackErrorException(string message, System.Exception inner) : base(message, inner) { }
}


[System.Serializable]
public class CallBackAbortException : System.Exception
{
    public CallBackAbortException() { }
    public CallBackAbortException(string message) : base(message) { }
    public CallBackAbortException(string message, System.Exception inner) : base(message, inner) { }
    
}
