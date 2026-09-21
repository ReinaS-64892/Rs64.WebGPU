// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;

namespace Rs64.WebGPU;

public partial class WebGpu
{
    public static WebGPUInstance CreateInstance()
    {
        unsafe
        {
            return new(new(FFI.Webgpu.wgpuCreateInstance(null)));
        }
    }
    public static WebGPUInstance CreateInstance(WebGpuInstanceDescriptor descriptor)
    {
        throw new WgpuNativeUnimplementedException();
#pragma warning disable CS0162 // Unreachable code detected
        var features = descriptor.CreateFeatureNameArray();
        var limits = descriptor.CreateLimit();
        unsafe
        {
            fixed (FFI.WGPUInstanceFeatureName* featuresPtr = features)
            {
                var desc = new FFI.WGPUInstanceDescriptor();

                desc.RequiredFeaturesCount = (nuint)features.Length;
                desc.RequiredFeatures = featuresPtr;

                desc.RequiredLimits = &limits;

                return new(new(FFI.Webgpu.wgpuCreateInstance(&desc)));
            }
        }
#pragma warning restore CS0162 // Unreachable code detected
    }

    public static WebGpuInstanceDescriptor GetInstanceDescriptor()
    {
        throw new WgpuNativeUnimplementedException();
#pragma warning disable CS0162 // Unreachable code detected
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

#pragma warning restore CS0162 // Unreachable code detected
    }
}
