// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Rs64.WebGPU;

public partial class WebGpu
{
    public static WebGPUInstance CreateInstance(WebGpuInstanceDescriptor? descriptor = null)
    {
        if (descriptor is null)
        {
            unsafe { return new(new(FFI.Webgpu.wgpuCreateInstance(null))); }
        }

        var features = descriptor.RequiredFeature.CreateFeatureNameArray();
        var limits = descriptor.RequiredLimit.CreateLimit();
        unsafe
        {
            fixed (FFI.WGPUInstanceFeatureName* featuresPtr = features)
            {
                var desc = new FFI.WGPUInstanceDescriptor
                {
                    RequiredFeaturesCount = (nuint)features.Length,
                    RequiredFeatures = featuresPtr,
                    RequiredLimits = &limits
                };

                return new(new(FFI.Webgpu.wgpuCreateInstance(&desc)));
            }
        }
    }

    public static WebGpuInstanceDescriptor GetInstanceDescriptor()
    {
        var f = new WebGpuInstanceDescriptor.Feature();
        unsafe
        {
            FFI.WGPUSupportedInstanceFeatures supportedInstanceFeatures;
            FFI.Webgpu.wgpuGetInstanceFeatures(&supportedInstanceFeatures);
            var names = new ReadOnlySpan<FFI.WGPUInstanceFeatureName>(supportedInstanceFeatures.Features, (int)supportedInstanceFeatures.FeaturesCount);
            foreach (var name in names)
            {
                switch (name)
                {
                    default: break;
                    case FFI.WGPUInstanceFeatureName.TimedWaitAny:
                        f.TimedWaitAny = true;
                        break;
                    case FFI.WGPUInstanceFeatureName.ShaderSourceSpirv:
                        f.ShaderSourceSpirv = true;
                        break;
                    case FFI.WGPUInstanceFeatureName.MultipleDevicesPerAdapter:
                        f.MultipleDevicesPerAdapter = true;
                        break;
                }
            }
            FFI.WGPUSupportedInstanceFeatures.FreeMembers(ref supportedInstanceFeatures);
        }
        var l = new WebGpuInstanceDescriptor.Limit();
        unsafe
        {
            FFI.WGPUInstanceLimits instanceLimits;
            FFI.Webgpu.wgpuGetInstanceLimits(&instanceLimits);
            l.TimedWaitAnyMaxCount = instanceLimits.TimedWaitAnyMaxCount;
        }
        return new() { RequiredFeature = f, RequiredLimit = l };
    }

    public static bool HasAllInstanceFeature(WebGpuInstanceDescriptor.Feature feature)
    {
        var nameArray = feature.CreateFeatureNameArray();
        return nameArray.Select(FFI.Webgpu.wgpuHasInstanceFeature).Select(wb => (bool)wb).All(b => b);
    }

}
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
