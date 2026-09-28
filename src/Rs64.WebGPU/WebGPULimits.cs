// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

namespace Rs64.WebGPU;

[FFINote(typeof(FFI.WGPULimits))]
public class WebGPULimits
{
    internal WebGPULimits() { }

    public uint? MaxTextureDimension1d { get; internal set; } = null;
    public uint? MaxTextureDimension2d { get; internal set; } = null;
    public uint? MaxTextureDimension3d { get; internal set; } = null;
    public uint? MaxTextureArrayLayers { get; internal set; } = null;
    public uint? MaxBindGroups { get; internal set; } = null;
    public uint? MaxBindGroupsPlusVertexBuffers { get; internal set; } = null;
    public uint? MaxBindingsPerBindGroup { get; internal set; } = null;
    public uint? MaxDynamicUniformBuffersPerPipelineLayout { get; internal set; } = null;
    public uint? MaxDynamicStorageBuffersPerPipelineLayout { get; internal set; } = null;
    public uint? MaxSampledTexturesPerShaderStage { get; internal set; } = null;
    public uint? MaxSamplersPerShaderStage { get; internal set; } = null;
    public uint? MaxStorageBuffersPerShaderStage { get; internal set; } = null;
    public uint? MaxStorageTexturesPerShaderStage { get; internal set; } = null;
    public uint? MaxUniformBuffersPerShaderStage { get; internal set; } = null;
    public ulong? MaxUniformBufferBindingSize { get; internal set; } = null;
    public ulong? MaxStorageBufferBindingSize { get; internal set; } = null;
    public uint? MinUniformBufferOffsetAlignment { get; internal set; } = null;
    public uint? MinStorageBufferOffsetAlignment { get; internal set; } = null;
    public uint? MaxVertexBuffers { get; internal set; } = null;
    public ulong? MaxBufferSize { get; internal set; } = null;
    public uint? MaxVertexAttributes { get; internal set; } = null;
    public uint? MaxVertexBufferArrayStride { get; internal set; } = null;
    public uint? MaxInterStageShaderVariables { get; internal set; } = null;
    public uint? MaxColorAttachments { get; internal set; } = null;
    public uint? MaxColorAttachmentBytesPerSample { get; internal set; } = null;
    public uint? MaxComputeWorkgroupStorageSize { get; internal set; } = null;
    public uint? MaxComputeInvocationsPerWorkgroup { get; internal set; } = null;
    public uint? MaxComputeWorkgroupSizeX { get; internal set; } = null;
    public uint? MaxComputeWorkgroupSizeY { get; internal set; } = null;
    public uint? MaxComputeWorkgroupSizeZ { get; internal set; } = null;
    public uint? MaxComputeWorkgroupsPerDimension { get; internal set; } = null;
    public uint? MaxImmediateSize { get; internal set; } = null;



    internal static WebGPULimits ToW(FFI.WGPULimits ffiLimits)
    {
        WebGPULimits limits = new()
        {
            MaxTextureDimension1d = ffiLimits.MaxTextureDimension1d == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxTextureDimension1d,
            MaxTextureDimension2d = ffiLimits.MaxTextureDimension2d == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxTextureDimension2d,
            MaxTextureDimension3d = ffiLimits.MaxTextureDimension3d == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxTextureDimension3d,
            MaxTextureArrayLayers = ffiLimits.MaxTextureArrayLayers == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxTextureArrayLayers,
            MaxBindGroups = ffiLimits.MaxBindGroups == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxBindGroups,
            MaxBindGroupsPlusVertexBuffers = ffiLimits.MaxBindGroupsPlusVertexBuffers == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxBindGroupsPlusVertexBuffers,
            MaxBindingsPerBindGroup = ffiLimits.MaxBindingsPerBindGroup == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxBindingsPerBindGroup,
            MaxDynamicUniformBuffersPerPipelineLayout = ffiLimits.MaxDynamicUniformBuffersPerPipelineLayout == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxDynamicUniformBuffersPerPipelineLayout,
            MaxDynamicStorageBuffersPerPipelineLayout = ffiLimits.MaxDynamicStorageBuffersPerPipelineLayout == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxDynamicStorageBuffersPerPipelineLayout,
            MaxSampledTexturesPerShaderStage = ffiLimits.MaxSampledTexturesPerShaderStage == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxSampledTexturesPerShaderStage,
            MaxSamplersPerShaderStage = ffiLimits.MaxSamplersPerShaderStage == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxSamplersPerShaderStage,
            MaxStorageBuffersPerShaderStage = ffiLimits.MaxStorageBuffersPerShaderStage == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxStorageBuffersPerShaderStage,
            MaxStorageTexturesPerShaderStage = ffiLimits.MaxStorageTexturesPerShaderStage == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxStorageTexturesPerShaderStage,
            MaxUniformBuffersPerShaderStage = ffiLimits.MaxUniformBuffersPerShaderStage == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxUniformBuffersPerShaderStage,
            MaxUniformBufferBindingSize = ffiLimits.MaxUniformBufferBindingSize == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxUniformBufferBindingSize,
            MaxStorageBufferBindingSize = ffiLimits.MaxStorageBufferBindingSize == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxStorageBufferBindingSize,
            MinUniformBufferOffsetAlignment = ffiLimits.MinUniformBufferOffsetAlignment == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MinUniformBufferOffsetAlignment,
            MinStorageBufferOffsetAlignment = ffiLimits.MinStorageBufferOffsetAlignment == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MinStorageBufferOffsetAlignment,
            MaxVertexBuffers = ffiLimits.MaxVertexBuffers == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxVertexBuffers,
            MaxBufferSize = ffiLimits.MaxBufferSize == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxBufferSize,
            MaxVertexAttributes = ffiLimits.MaxVertexAttributes == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxVertexAttributes,
            MaxVertexBufferArrayStride = ffiLimits.MaxVertexBufferArrayStride == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxVertexBufferArrayStride,
            MaxInterStageShaderVariables = ffiLimits.MaxInterStageShaderVariables == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxInterStageShaderVariables,
            MaxColorAttachments = ffiLimits.MaxColorAttachments == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxColorAttachments,
            MaxColorAttachmentBytesPerSample = ffiLimits.MaxColorAttachmentBytesPerSample == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxColorAttachmentBytesPerSample,
            MaxComputeWorkgroupStorageSize = ffiLimits.MaxComputeWorkgroupStorageSize == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxComputeWorkgroupStorageSize,
            MaxComputeInvocationsPerWorkgroup = ffiLimits.MaxComputeInvocationsPerWorkgroup == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxComputeInvocationsPerWorkgroup,
            MaxComputeWorkgroupSizeX = ffiLimits.MaxComputeWorkgroupSizeX == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxComputeWorkgroupSizeX,
            MaxComputeWorkgroupSizeY = ffiLimits.MaxComputeWorkgroupSizeY == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxComputeWorkgroupSizeY,
            MaxComputeWorkgroupSizeZ = ffiLimits.MaxComputeWorkgroupSizeZ == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxComputeWorkgroupSizeZ,
            MaxComputeWorkgroupsPerDimension = ffiLimits.MaxComputeWorkgroupsPerDimension == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxComputeWorkgroupsPerDimension,
            MaxImmediateSize = ffiLimits.MaxImmediateSize == FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED ? null : ffiLimits.MaxImmediateSize
        };

        unsafe
        {
            var exPtr = ffiLimits.NextInChain;
            while (exPtr is not null)
            {
                var currentPtr = exPtr;
                exPtr = exPtr->Next;
                switch (currentPtr->StructType)
                {
                    default: break;
                    case FFI.WGPUSType.CompatibilityModeLimits:
                        {
                            //TODO : 実装 !!! ログの仕組み!!!
                            System.Console.WriteLine("have CompatibilityModeLimits !");
                            break;
                        }
                }
            }
        }
        return limits;
    }
    internal static FFI.WGPULimits ToF(WebGPULimits limits)
    {
        FFI.WGPULimits ffiLimits = new()
        {
            MaxTextureDimension1d = limits.MaxTextureDimension1d.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxTextureDimension1d.Value,
            MaxTextureDimension2d = limits.MaxTextureDimension2d.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxTextureDimension2d.Value,
            MaxTextureDimension3d = limits.MaxTextureDimension3d.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxTextureDimension3d.Value,
            MaxTextureArrayLayers = limits.MaxTextureArrayLayers.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxTextureArrayLayers.Value,
            MaxBindGroups = limits.MaxBindGroups.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxBindGroups.Value,
            MaxBindGroupsPlusVertexBuffers = limits.MaxBindGroupsPlusVertexBuffers.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxBindGroupsPlusVertexBuffers.Value,
            MaxBindingsPerBindGroup = limits.MaxBindingsPerBindGroup.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxBindingsPerBindGroup.Value,
            MaxDynamicUniformBuffersPerPipelineLayout = limits.MaxDynamicUniformBuffersPerPipelineLayout.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxDynamicUniformBuffersPerPipelineLayout.Value,
            MaxDynamicStorageBuffersPerPipelineLayout = limits.MaxDynamicStorageBuffersPerPipelineLayout.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxDynamicStorageBuffersPerPipelineLayout.Value,
            MaxSampledTexturesPerShaderStage = limits.MaxSampledTexturesPerShaderStage.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxSampledTexturesPerShaderStage.Value,
            MaxSamplersPerShaderStage = limits.MaxSamplersPerShaderStage.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxSamplersPerShaderStage.Value,
            MaxStorageBuffersPerShaderStage = limits.MaxStorageBuffersPerShaderStage.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxStorageBuffersPerShaderStage.Value,
            MaxStorageTexturesPerShaderStage = limits.MaxStorageTexturesPerShaderStage.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxStorageTexturesPerShaderStage.Value,
            MaxUniformBuffersPerShaderStage = limits.MaxUniformBuffersPerShaderStage.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxUniformBuffersPerShaderStage.Value,
            MaxUniformBufferBindingSize = limits.MaxUniformBufferBindingSize.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxUniformBufferBindingSize.Value,
            MaxStorageBufferBindingSize = limits.MaxStorageBufferBindingSize.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxStorageBufferBindingSize.Value,
            MinUniformBufferOffsetAlignment = limits.MinUniformBufferOffsetAlignment.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MinUniformBufferOffsetAlignment.Value,
            MinStorageBufferOffsetAlignment = limits.MinStorageBufferOffsetAlignment.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MinStorageBufferOffsetAlignment.Value,
            MaxVertexBuffers = limits.MaxVertexBuffers.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxVertexBuffers.Value,
            MaxBufferSize = limits.MaxBufferSize.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxBufferSize.Value,
            MaxVertexAttributes = limits.MaxVertexAttributes.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxVertexAttributes.Value,
            MaxVertexBufferArrayStride = limits.MaxVertexBufferArrayStride.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxVertexBufferArrayStride.Value,
            MaxInterStageShaderVariables = limits.MaxInterStageShaderVariables.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxInterStageShaderVariables.Value,
            MaxColorAttachments = limits.MaxColorAttachments.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxColorAttachments.Value,
            MaxColorAttachmentBytesPerSample = limits.MaxColorAttachmentBytesPerSample.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxColorAttachmentBytesPerSample.Value,
            MaxComputeWorkgroupStorageSize = limits.MaxComputeWorkgroupStorageSize.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxComputeWorkgroupStorageSize.Value,
            MaxComputeInvocationsPerWorkgroup = limits.MaxComputeInvocationsPerWorkgroup.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxComputeInvocationsPerWorkgroup.Value,
            MaxComputeWorkgroupSizeX = limits.MaxComputeWorkgroupSizeX.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxComputeWorkgroupSizeX.Value,
            MaxComputeWorkgroupSizeY = limits.MaxComputeWorkgroupSizeY.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxComputeWorkgroupSizeY.Value,
            MaxComputeWorkgroupSizeZ = limits.MaxComputeWorkgroupSizeZ.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxComputeWorkgroupSizeZ.Value,
            MaxComputeWorkgroupsPerDimension = limits.MaxComputeWorkgroupsPerDimension.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxComputeWorkgroupsPerDimension.Value,
            MaxImmediateSize = limits.MaxImmediateSize.HasValue is false ? FFI.Webgpu.WGPU_LIMIT_U32_UNDEFINED : limits.MaxImmediateSize.Value
        };

        return ffiLimits;
    }
}
