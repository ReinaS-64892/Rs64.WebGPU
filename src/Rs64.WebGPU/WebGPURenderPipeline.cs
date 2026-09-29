// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;

namespace Rs64.WebGPU;

[FFINote(typeof(FFI.WGPURenderPipeline))]
public class WebGPURenderPipeline : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPURenderPipeline> Native { get; }
    internal WebGPURenderPipeline(WGPUObjectHolder<FFI.WGPURenderPipeline> holder) { Native = holder; }
    public void Dispose() { Native.Dispose(); }

}

[FFINote(typeof(FFI.WGPURenderPipelineDescriptor))]
public class WebGPURenderPipelineDescriptor
{
    public string Label = "";
    public WebGPUPipelineLayout? Layout;
    public required WebGPUVertexState Vertex;
    public WebGPUPrimitiveState Primitive = new();
    public WebGPUDepthStencilState? DepthStencil;
    public WebGPUMultisampleState Multisample = new();
    public WebGPUFragmentState? Fragment;

    internal FFI.WGPURenderPipelineDescriptor ToF(ref FFIStackMemory ffiMem)
    {
        unsafe
        {
            var ds = DepthStencil is not null ? ffiMem.CopyToAllocate(DepthStencil.ToF()) : default;
            var fs = Fragment is not null ? ffiMem.CopyToAllocate(Fragment.ToF(ref ffiMem)) : default;
            return new()
            {
                Label = ffiMem.AllocateString(Label),
                Layout = Layout is not null ? Layout.Native.GetPtr() : null,
                Vertex = Vertex.ToF(ref ffiMem),
                Primitive = Primitive.ToF(),
                DepthStencil = ds,
                Multisample = Multisample.ToF(),
                Fragment = fs,
            };
        }
    }
}

[FFINote(typeof(FFI.WGPUFragmentState))]
public class WebGPUFragmentState
{
    public required WebGPUShaderModule Module;
    public string? EntryPoint;
    public WebGPUConstantEntry[] Constants = [];
    public WebGPUColorTargetState[] Targets = [];
    internal unsafe FFI.WGPUFragmentState ToF(ref FFIStackMemory ffiMem)
    {
        var constants = ffiMem.AllocateArea<FFI.WGPUConstantEntry>(Constants.Length);
        for (var i = 0; Constants.Length > i; i += 1) { constants[i] = Constants[i].ToF(ref ffiMem); }
        var targets = ffiMem.AllocateArea<FFI.WGPUColorTargetState>(Targets.Length);
        for (var i = 0; Targets.Length > i; i += 1) { targets[i] = Targets[i].ToF(ref ffiMem); }
        return new()
        {
            Module = Module.Native.GetPtr(),
            EntryPoint = ffiMem.AllocateString(EntryPoint),
            ConstantsCount = (nuint)Constants.Length,
            Constants = constants,
            TargetsCount = (nuint)Targets.Length,
            Targets = targets,
        };
    }
}

[FFINote(typeof(FFI.WGPUColorTargetState))]
public class WebGPUColorTargetState
{
    public WebGPUTextureFormat? Format;
    public WebGPUBlendState? Blend;
    public WebGPUColorWriteMask WriteMask = WebGPUColorWriteMask.All;

    internal unsafe FFI.WGPUColorTargetState ToF(scoped ref FFIStackMemory ffiMem)
    {
        var blend = Blend.HasValue ? ffiMem.CopyToAllocate(Blend.Value.ToF()) : null;
        return new()
        {
            Format = Format.ToF(),
            Blend = blend,
            WriteMask = WriteMask.ToF(),
        };
    }
}

[FFINote(typeof(FFI.WGPUBlendState))]
public struct WebGPUBlendState
{
    public WebGPUBlendComponent Color;
    public WebGPUBlendComponent Alpha;
    internal FFI.WGPUBlendState ToF()
    {
        return new()
        {
            Color = Color.ToF(),
            Alpha = Alpha.ToF(),
        };
    }
}

[FFINote(typeof(FFI.WGPUBlendComponent))]
public struct WebGPUBlendComponent
{
    public WebGPUBlendOperation? Operation;
    public WebGPUBlendFactor? SrcFactor;
    public WebGPUBlendFactor? DstFactor;
    internal FFI.WGPUBlendComponent ToF()
    {
        return new()
        {
            Operation = Operation.ToF(),
            SrcFactor = SrcFactor.ToF(),
            DstFactor = DstFactor.ToF(),
        };
    }
}

[FFINote(typeof(FFI.WGPUMultisampleState))]
public class WebGPUMultisampleState
{
    public uint Count = 1;
    public uint Mask = 0xFFFFFFFF;
    public bool AlphaToCoverageEnabled = false;
    internal FFI.WGPUMultisampleState ToF()
    {
        return new()
        {
            Count = Count,
            Mask = Mask,
            AlphaToCoverageEnabled = (FFI.WGPUBool)AlphaToCoverageEnabled,
        };
    }
}

[FFINote(typeof(FFI.WGPUDepthStencilState))]
public class WebGPUDepthStencilState
{
    public WebGPUTextureFormat? Format;
    public bool? DepthWriteEnabled;
    public WebGPUCompareFunction? DepthCompare;
    public WebGPUStencilFaceState StencilFront;
    public WebGPUStencilFaceState StencilBack;
    public uint StencilReadMask = 0xFFFFFFFF;
    public uint StencilWriteMask = 0xFFFFFFFF;
    public int DepthBias = 0;
    public float DepthBiasSlopeScale = 0;
    public float DepthBiasClamp = 0;

    internal FFI.WGPUDepthStencilState ToF()
    {
        return new()
        {
            Format = Format.ToF(),
            DepthWriteEnabled = DepthWriteEnabled is not null ? (DepthWriteEnabled.Value ? FFI.WGPUOptionalBool.True : FFI.WGPUOptionalBool.False) : FFI.WGPUOptionalBool.Undefined,
            DepthCompare = DepthCompare.ToF(),
            StencilFront = StencilFront.ToF(),
            StencilBack = StencilBack.ToF(),
            StencilReadMask = StencilReadMask,
            StencilWriteMask = StencilWriteMask,
            DepthBias = DepthBias,
            DepthBiasSlopeScale = DepthBiasSlopeScale,
            DepthBiasClamp = DepthBiasClamp,
        };
    }
}

[FFINote(typeof(FFI.WGPUStencilFaceState))]
public struct WebGPUStencilFaceState
{
    public WebGPUCompareFunction? Compare;
    public WebGPUStencilOperation? FailOp;
    public WebGPUStencilOperation? DepthFailOp;
    public WebGPUStencilOperation? PassOp;

    internal FFI.WGPUStencilFaceState ToF()
    {
        return new()
        {
            Compare = Compare.ToF(),
            FailOp = FailOp.ToF(),
            DepthFailOp = DepthFailOp.ToF(),
            PassOp = PassOp.ToF(),
        };
    }
}

[FFINote(typeof(FFI.WGPUPrimitiveState))]
public class WebGPUPrimitiveState
{
    public WebGPUPrimitiveTopology? Topology;
    public WebGPUIndexFormat? StripIndexFormat;
    public WebGPUFrontFace? FrontFace;
    public WebGPUCullMode? CullMode;
    public bool UnclippedDepth = false;

    internal FFI.WGPUPrimitiveState ToF()
    {
        return new()
        {
            Topology = Topology.ToF(),
            StripIndexFormat = StripIndexFormat.ToF(),
            FrontFace = FrontFace.ToF(),
            CullMode = CullMode.ToF(),
        };
    }
}

[FFINote(typeof(FFI.WGPUVertexState))]
public class WebGPUVertexState
{
    public required WebGPUShaderModule Module;
    public string? EntryPoint;

    public WebGPUConstantEntry[] Constants = [];
    public WebGPUVertexBufferLayout[] Buffers = [];

    internal unsafe FFI.WGPUVertexState ToF(ref FFIStackMemory ffiMem)
    {
        var constants = ffiMem.AllocateArea<FFI.WGPUConstantEntry>(Constants.Length);
        for (var i = 0; Constants.Length > i; i += 1) { constants[i] = Constants[i].ToF(ref ffiMem); }
        var buffers = ffiMem.AllocateArea<FFI.WGPUVertexBufferLayout>(Buffers.Length);
        for (var i = 0; Buffers.Length > i; i += 1) { buffers[i] = Buffers[i].ToF(ref ffiMem); }
        return new()
        {
            Module = Module.Native.GetPtr(),
            EntryPoint = ffiMem.AllocateString(EntryPoint),
            ConstantsCount = (nuint)Constants.Length,
            Constants = constants,
            BuffersCount = (nuint)Buffers.Length,
            Buffers = buffers,
        };
    }
}

[FFINote(typeof(FFI.WGPUVertexBufferLayout))]
public class WebGPUVertexBufferLayout
{
    public WebGPUVertexStepMode? StepMode;
    public ulong ArrayStride;
    public WebGPUVertexAttribute[] Attributes = [];
    internal unsafe FFI.WGPUVertexBufferLayout ToF(scoped ref FFIStackMemory ffiMem)
    {
        var attributes = ffiMem.AllocateArea<FFI.WGPUVertexAttribute>(Attributes.Length);
        for (var i = 0; Attributes.Length > i; i += 1) { attributes[i] = Attributes[i].ToF(); }
        return new()
        {
            StepMode = StepMode.ToF(),
            ArrayStride = ArrayStride,
            AttributesCount = (nuint)Attributes.Length,
            Attributes = attributes,
        };
    }
}

[FFINote(typeof(FFI.WGPUVertexAttribute))]
public class WebGPUVertexAttribute
{

    public WebGPUVertexFormat Format;
    public ulong Offset;
    public uint ShaderLocation;
    internal FFI.WGPUVertexAttribute ToF()
    {
        return new()
        {
            Format = Format.ToF(),
            Offset = Offset,
            ShaderLocation = ShaderLocation,
        };
    }
}
