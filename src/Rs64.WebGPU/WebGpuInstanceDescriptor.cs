// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System.Collections.Generic;
using System.Linq;

namespace Rs64.WebGPU;

public class WebGpuInstanceDescriptor
{
    public HashSet<WebGPUInstanceFeatureName> RequiredFeatures = [];
    public WebGPUInstanceLimits RequiredLimit = new();
    internal FFI.WGPUInstanceFeatureName[] ConvertToFFIRequiredFeature() { return ConvertToFFIRequiredFeature(RequiredFeatures); }
    internal static FFI.WGPUInstanceFeatureName[] ConvertToFFIRequiredFeature(HashSet<WebGPUInstanceFeatureName> featureNames)
    {
        return featureNames.Select(WebGPUInstanceFeatureNameUtil.ToF).ToArray();
    }
}

public class WebGPUInstanceLimits
{
    public nuint TimedWaitAnyMaxCount = 0;

    internal FFI.WGPUInstanceLimits CreateLimit()
    {
        var limit = new FFI.WGPUInstanceLimits();
        limit.TimedWaitAnyMaxCount = TimedWaitAnyMaxCount;
        return limit;
    }
}

