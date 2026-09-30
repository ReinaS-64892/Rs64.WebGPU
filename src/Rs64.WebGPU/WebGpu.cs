// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Collections.Generic;
using System.Linq;

namespace Rs64.WebGPU;

[FFINote(typeof(FFI.Webgpu))]
public partial class WebGpu
{
    public static WebGPUInstance CreateInstance(WebGpuInstanceDescriptor? descriptor = null)
    {
        if (descriptor is null)
        {
            unsafe { return new(new(FFI.Webgpu.wgpuCreateInstance(null))); }
        }

        var features = descriptor.ConvertToFFIRequiredFeature();
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
        return new()
        {
            RequiredFeatures = GetInstanceFeatures(),
            RequiredLimit = GetInstanceLimits()
        };
    }
    public static HashSet<WebGPUInstanceFeatureName> GetInstanceFeatures()
    {
        var output = new HashSet<WebGPUInstanceFeatureName>();
        unsafe
        {
            FFI.WGPUSupportedInstanceFeatures ffiFeatures;
            FFI.Webgpu.wgpuGetInstanceFeatures(&ffiFeatures);
            var names = new ReadOnlySpan<FFI.WGPUInstanceFeatureName>(ffiFeatures.Features, (int)ffiFeatures.FeaturesCount);
            foreach (var name in names)
            {
                output.Add(name.ToW());
            }
            FFI.WGPUSupportedInstanceFeatures.FreeMembers(ref ffiFeatures);
        }
        return output;
    }
    public static WebGPUInstanceLimits GetInstanceLimits()
    {
        var l = new WebGPUInstanceLimits();
        unsafe
        {
            FFI.WGPUInstanceLimits instanceLimits;
            FFI.Webgpu.wgpuGetInstanceLimits(&instanceLimits);
            l.TimedWaitAnyMaxCount = instanceLimits.TimedWaitAnyMaxCount;
        }
        return l;
    }

    public static bool HasInstanceFeature(WebGPUInstanceFeatureName feature)
    {
        return (bool)FFI.Webgpu.wgpuHasInstanceFeature(feature.ToF());
    }

    public static bool HasAllInstanceFeature(HashSet<WebGPUInstanceFeatureName> features)
    {
        return features.All(HasInstanceFeature);
    }

}
