// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Runtime.InteropServices;

namespace Rs64.WebGPU;

internal unsafe class WGPUObjectHolder<TWGPUObject> : IDisposable
where TWGPUObject : unmanaged, FFI.IWGPUObject<TWGPUObject>
{
    TWGPUObject* _ptr = null;
    public WGPUObjectHolder(TWGPUObject* ptr)
    {
        _ptr = ptr;
    }

    public TWGPUObject* GetPtr()
    {
        if (_ptr is null) { throw new ObjectUseAfterFreeException(); }
        return _ptr;
    }
    public bool TryGet(out TWGPUObject* ptr)
    {
        if (_ptr is null) { ptr = null; return false; }
        ptr = _ptr;
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
        if (_ptr is null) { return; }
        var ptr = _ptr;
        _ptr = null;
        TWGPUObject.Release(ptr);
        GC.SuppressFinalize(this);
    }

    ~WGPUObjectHolder()
    {
        Dispose();
    }
}
