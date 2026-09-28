// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Rs64.WebGPU;

/// <summary>
/// stackalloc の領域を使用し、ライフタイムが少し長い struct とを作るためにある。
/// 必要ないのであれば、使わないに超したことはない。
/// </summary>
/// <param name="bytes">MUST BE FROM STACKALLOC</param>
internal unsafe ref struct StackAllocAsFFIArea(Span<byte> bytes)
{
    private readonly byte* _ptr = (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(bytes));
    private readonly int _length = bytes.Length;
    private int _stackCount;

    public T* Allocate<T>(int length = 1)
    where T : unmanaged, allows ref struct
    {

        var allocateSize = sizeof(T) * length;
        if (_length < (_stackCount + allocateSize)) { throw new StackAreaOverflowException(); }

        var targetPtr = _ptr + _stackCount;

        for (var i = 0; allocateSize > i; i += 1) { targetPtr[i] = 0; }
        _stackCount += allocateSize;

        return (T*)targetPtr;
    }
}

/// <param name="bytes">MUST BE FROM STACKALLOC</param>
internal unsafe ref struct FFIMemoryManager(Span<byte> bytes) : IDisposable
{
    public StackAllocAsFFIArea stackArea = new(bytes);
    public FFI.WGPUStringView AllocateString(string? str)
    {
        if (str is null) { return FFI.WGPUStringView.Null; }
        if (str.Length is 0) { return FFI.WGPUStringView.Empty; }

        var h = FFI.WGPUStringView.ConvertPinnedString(str);
        _holders.Add(h);
        return h.GetStringView();
    }
    public T* AllocateArea<T>(int length = 1)
    where T : unmanaged, allows ref struct
    {
        return stackArea.Allocate<T>(length);
    }

    public T* PinArray<T>(T[] target)
    where T : unmanaged
    {
        var handle = GCHandle.Alloc(target);
        _pinedExternals.Add(handle);
        return (T*)handle.AddrOfPinnedObject();
    }

    List<FFI.WGPUStringView.WGPUPinnedStringHolder> _holders = new();
    List<GCHandle> _pinedExternals = new();
    public void Dispose()
    {
        foreach (var h in _holders) { h.Dispose(); }
        _holders.Clear();

        foreach (var h in _pinedExternals) { h.Free(); }
        _pinedExternals.Clear();
    }
}
