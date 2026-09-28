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
    public WebGPUVertexState Vertex = new();
    public WebGPUPrimitiveState Primitive = new();
    public WebGPUDepthStencilState? DepthStencil;
    public WebGPUMultisampleState Multisample = new();
    public WebGPUFragmentState? Fragment;

    internal FFI.WGPURenderPipelineDescriptor ToF(FFIMemoryManager ffiMem)
    {
        unsafe
        {
            var ds = DepthStencil is not null ? DepthStencil.ToF() : default;
            var fs = Fragment is not null ? Fragment.ToF() : default;
            return new()
            {
                Label = ffiMem.AllocateString(Label),
                Layout = Layout is not null ? Layout.Native.GetPtr() : null,
                Vertex = Vertex.ToF(),
                Primitive = Primitive.ToF(),
                DepthStencil = DepthStencil is not null ? &ds : null,
                Multisample = Multisample.ToF(),
                Fragment = Fragment is not null ? &fs : null,
            };
        }
    }
}

[FFINote(typeof(FFI.WGPUFragmentState))]
public class WebGPUFragmentState
{
    //TODO : 
    internal FFI.WGPUFragmentState ToF()
    {
        throw new Exception();
    }
}

[FFINote(typeof(FFI.WGPUMultisampleState))]
public class WebGPUMultisampleState
{
    //TODO : 
    internal FFI.WGPUMultisampleState ToF()
    {
        throw new Exception();
    }
}

[FFINote(typeof(FFI.WGPUDepthStencilState))]
public class WebGPUDepthStencilState
{
    //TODO : 
    internal FFI.WGPUDepthStencilState ToF()
    {
        throw new Exception();
    }
}

[FFINote(typeof(FFI.WGPUPrimitiveState))]
public class WebGPUPrimitiveState
{
    //TODO : 
    internal FFI.WGPUPrimitiveState ToF()
    {
        throw new Exception();
    }
}

[FFINote(typeof(FFI.WGPUVertexState))]
public class WebGPUVertexState
{
    //TODO : 
    internal FFI.WGPUVertexState ToF()
    {
        throw new Exception();
    }
}
