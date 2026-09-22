// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
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

    public ref T Allocate<T>()
    where T : unmanaged, allows ref struct
    {
        var allocateSize = sizeof(T);
        if (_length < (_stackCount + allocateSize)) { throw new StackAreaOverflowException(); }

        var targetPtr = _ptr + _stackCount;

        for (var i = 0; allocateSize > i; i += 1) { targetPtr[i] = 0; }
        _stackCount += allocateSize;

        return ref Unsafe.AsRef<T>(targetPtr);
    }
}
