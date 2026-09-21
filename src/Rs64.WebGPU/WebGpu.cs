// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Linq;

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
