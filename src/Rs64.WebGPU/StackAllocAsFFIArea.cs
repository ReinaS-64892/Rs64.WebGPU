// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Rs64.WebGPU;

internal unsafe struct FFIStackMemory : IDisposable
{
    public static FFIStackMemoryScope BindScope(ref FFIStackMemory stackMemory)
    {
        return new(ref stackMemory);
    }
    internal ref struct FFIStackMemoryScope : IDisposable
    {
        private ref FFIStackMemory _ffiMem;

        internal FFIStackMemoryScope(ref FFIStackMemory fFIStackMemory)
        {
            _ffiMem = ref fFIStackMemory;
        }

        public void Dispose()
        {
            _ffiMem.Dispose();
        }
    }

    List<FFI.WGPUStringView.WGPUPinnedStringHolder> _holders = new();
    List<GCHandle> _pinedExternals = new();

    /// <param name="bytes">MUST BE FROM STACKALLOC</param>
    public FFIStackMemory(Span<nint> bytes)
    {
        stackArea = new(bytes);
    }

    public void Dispose()
    {
        if (_holders is not null)
        {
            foreach (var h in _holders) { h.Dispose(); }
            _holders.Clear();
        }

        if (_pinedExternals is not null)
        {
            foreach (var h in _pinedExternals) { h.Free(); }
            _pinedExternals.Clear();
        }
    }




    private StackAllocAsFFIArea stackArea;

    /// <summary>
    /// stackalloc の領域を使用し、ライフタイムが少し長い struct とを作るためにある。
    /// 必要ないのであれば、使わないに超したことはない。
    /// </summary>
    private struct StackAllocAsFFIArea
    {
        private readonly byte* _ptr;
        private readonly int _length;
        private int _stackCount;

        /// <param name="bytes">MUST BE FROM STACKALLOC</param>
        public StackAllocAsFFIArea(Span<nint> bytes)
        {
            _ptr = (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(bytes));
            _length = sizeof(nint) * bytes.Length;
        }

        public T* Allocate<T>(int length = 1)
        where T : unmanaged, allows ref struct
        {
            if (length is 0 || length < 0) { return null; }

            var allocateSize = sizeof(T) * length;

            if ((allocateSize % sizeof(nint)) is not 0) { allocateSize += sizeof(nint) - (allocateSize % sizeof(nint)); }

            if (_length < (_stackCount + allocateSize)) { throw new StackAreaOverflowException(); }

            var targetPtr = _ptr + _stackCount;

            for (var i = 0; allocateSize > i; i += 1) { targetPtr[i] = 0; }
            _stackCount += allocateSize;

            return (T*)targetPtr;
        }
    }
    public FFI.WGPUStringView AllocateString(string? str)
    {
        if (str is null) { return FFI.WGPUStringView.Null; }
        if (str.Length is 0) { return FFI.WGPUStringView.Empty; }

        var h = FFI.WGPUStringView.ConvertPinnedString(str);
        _holders ??= new();
        _holders.Add(h);
        return h.GetStringView();
    }
    public T* AllocateArea<T>(int length = 1)
    where T : unmanaged, allows ref struct
    {
        return stackArea.Allocate<T>(length);
    }
    public T* CopyToAllocate<T>(T val)
    where T : unmanaged, allows ref struct
    {
        var area = stackArea.Allocate<T>();
        *area = val;
        return area;
    }

    public T* PinArray<T>(T[] target)
    where T : unmanaged
    {
        var handle = GCHandle.Alloc(target);
        _pinedExternals ??= new();
        _pinedExternals.Add(handle);
        return (T*)handle.AddrOfPinnedObject();
    }

}
