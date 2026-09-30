// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System.Numerics;
using System.Runtime.InteropServices;
using Rs64.WebGPU;

const string WGSL_VERT =
"""
struct Vertex {
    @location(0) position: vec3f,
};
struct VertexOut {
    @builtin(position) position: vec4f,
};
@vertex fn vs(vert: Vertex) -> VertexOut {
    var vOut: VertexOut;
    vOut.position = vec4f(vert.position,1.0);
    return vOut;
}
""";

const string WGSL_FRAG =
"""
@fragment fn fs() -> @location(0) vec4f
{
    return vec4f(0.0,0.0,0.0,1.0);
}
""";


Console.WriteLine("nun !!!");


using var instance = WebGpu.CreateInstance();

using var adapter = await instance.RequestAdapter();
using var device = await adapter.RequestRequestDevice();


var targetRenderTextureFormat = WebGPUTextureFormat.Rgba8UnormSrgb;
using var renderTexture = device.CreateTexture(new()
{
    Format = targetRenderTextureFormat,
    MipLevelCount = 1,
    SampleCount = 1,
    Size = new()
    {
        Height = 512,
        Width = 512,
        DepthOrArrayLayers = 1,
    },
    Dimension = WebGPUTextureDimension.W_2d,
    Usage = WebGPUTextureUsage.RenderAttachment | WebGPUTextureUsage.CopySrc,
});
using var rTextureView = renderTexture.CreateView(new() { });

using var vertexBufferMapped = device.CreateMappedBuffer(new()
{
    Label = "vertex",
    Usage = WebGPUBufferUsage.Vertex,
    Size = 4 * 3 * 3
})!.Value;
using (var mapBuffer = vertexBufferMapped.Mapped)
{
    Span<Vector3> vertex = [
        new(-0.5f, -0.5f, 0.0f),
        new(0.5f, -0.5f, 0.0f),
        new(0.0f, 0.5f, 0.0f),
    ];
    
    mapBuffer.WriteMappedRange(MemoryMarshal.Cast<Vector3, byte>(vertex));
}
var vertexBuffer = vertexBufferMapped.Buffer;
var outputBufferSize = 512u * 512 * 4;
using var outputBuffer = device.CreateBuffer(new()
{
    Size = outputBufferSize,
    Usage = WebGPUBufferUsage.CopyDst | WebGPUBufferUsage.MapRead
}) ?? throw new Exception("バッファーが作れなかったよ"); ;


using var vertexShader = device.CreateShaderModule(new WebGPUWgslShaderModuleDescriptor()
{
    Label = "vertex shader ht",
    Code = WGSL_VERT,
});
using var fragmentShader = device.CreateShaderModule(new WebGPUWgslShaderModuleDescriptor()
{
    Label = "fragment shader ht",
    Code = WGSL_FRAG,
});

using var pipeline = await device.CreateRenderPipelineAsync(new()
{
    Label = "render pipe line",
    Vertex = new()
    {
        Module = vertexShader,
        EntryPoint = "vs",
        Buffers = [
            new(){
                StepMode = WebGPUVertexStepMode.Vertex,
                ArrayStride = 12,
                Attributes = [
                    new(){
                        ShaderLocation = 0,
                        Offset = 0,
                        Format = WebGPUVertexFormat.Float32x3,
                    }
                ]
            }
        ],
    },
    Primitive = new()
    {
        Topology = WebGPUPrimitiveTopology.TriangleList,
        CullMode = WebGPUCullMode.None,
    },
    Fragment = new()
    {
        Module = fragmentShader,
        EntryPoint = "fs",
        Targets = [
            new(){
                Format = targetRenderTextureFormat,
                Blend = new(){
                    Color = new(){
                        SrcFactor = WebGPUBlendFactor.SrcAlpha,
                        DstFactor = WebGPUBlendFactor.OneMinusSrcAlpha,
                        Operation = WebGPUBlendOperation.Add,
                    },
                    Alpha = new(){
                        SrcFactor = WebGPUBlendFactor.SrcAlpha,
                        DstFactor = WebGPUBlendFactor.OneMinusSrcAlpha,
                        Operation = WebGPUBlendOperation.Add,
                    }
                },
                WriteMask = WebGPUColorWriteMask.All,
            }
        ]
    }
});


using var queue = device.GetQueue();

{
    using var encoder = device.CreateCommandEncoder();
    using (var pss = encoder.BeginRenderPass(new()
    {
        ColorAttachments = [new(){
            View = rTextureView,
            LoadOp = WebGPULoadOp.Clear,
            StoreOp = WebGPUStoreOp.Store,
            ClearValue = new(0.0,1.0,1.0,1),
        }]
    }))
    {
        pss.SetPipeline(pipeline);
        pss.SetVertexBuffer(0, vertexBuffer, 0, 36);
        pss.Draw(3, 1, 0, 0);
    }

    using var commandBuffer = encoder.Finish();
    queue.Submit([commandBuffer]);

}
{
    using var encoder = device.CreateCommandEncoder();
    encoder.CopyTextureToBuffer(
        new() { Texture = renderTexture, },
        new()
        {
            Buffer = outputBuffer,
            Layout = new()
            {
                BytesPerRow = 512 * 4,
            }
        },
        new()
        {
            Width = 512,
            Height = 512,
            DepthOrArrayLayers = 1,
        }
    );
    using var commandBuffer = encoder.Finish();
    queue.Submit([commandBuffer]);
    await queue.OnSubmittedWorkDone();
}
Console.WriteLine("readback ... ");
var resultArray = await CopyToRam(outputBuffer, outputBufferSize);
Console.WriteLine("readback success !");
static async Task<byte[]> CopyToRam(WebGPUBuffer outputBuffer, uint outputBufferSize)
{
    using var mapped = await outputBuffer.MapAsync(WebGPUMapMode.Read, 0, outputBufferSize);
    var mappedRange = mapped.GetConstMappedRange();
    return mappedRange.ToArray();
}

Console.WriteLine("output run !");
{
    var outPath = "output.png";
    if (File.Exists(outPath)) { File.Delete(outPath); }
    using var outStream = File.Open(outPath, FileMode.Create);

    new StbImageWriteSharp.ImageWriter().WritePng(resultArray, 512, 512, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, outStream);
}
Console.WriteLine("output exit !");
