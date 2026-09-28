// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Rs64.WebGPU;

[FFINote(typeof(FFI.IWGPUObject<>))]
internal unsafe class WGPUObjectHolder<TWGPUObject> : IDisposable
where TWGPUObject : unmanaged, FFI.IWGPUObject<TWGPUObject>
{
    IntPtr _ptr = IntPtr.Zero;
    public WGPUObjectHolder(TWGPUObject* ptr)
    {
        if (ptr is null) { throw new InvalidObjectPointerException(); }
        
        _ptr = (nint)ptr;
    }

    public TWGPUObject* GetPtr()
    {
        if (_ptr == IntPtr.Zero) { throw new ObjectUseAfterFreeException(); }
        return (TWGPUObject*)_ptr;
    }
    public bool TryGet(out TWGPUObject* ptr)
    {
        if (_ptr == IntPtr.Zero) { ptr = null; return false; }
        ptr = (TWGPUObject*)_ptr;
        return true;
    }

    public WGPUObjectHolder<TWGPUObject> AddNewReference()
    {
        var ptr = GetPtr();
        TWGPUObject.AddRef(ptr);
        return new(ptr);
    }

    public void Dispose()
    {
        var ptr = Interlocked.Exchange<IntPtr>(ref _ptr, IntPtr.Zero);
        if (ptr == IntPtr.Zero) { return; }
        TWGPUObject.Release((TWGPUObject*)ptr);
        GC.SuppressFinalize(this);
    }

    ~WGPUObjectHolder()
    {
        Dispose();
    }
}
