// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System.Numerics;
using System.Runtime.InteropServices;
using NWayland.Protocols.Wayland;
using NWayland.Protocols.XdgShell;
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


Console.WriteLine("neko ~~~ !");


var display = WlDisplay.Connect();
var compositor = default(WlCompositor);
var xdgWmBase = default(XdgWmBase);
var registry = display.GetRegistry(new WlRegistry.Listener.Relay()
{
    OnGlobal = (sender, name, iface, version) =>
    {
        // Console.WriteLine($"interface {iface}-{version}");
        switch (iface)
        {
            case "wl_seat":
                {
                    WlSeat.Bind(sender, name, Math.Min(version, 7), new WlSeat.Listener.Relay()
                    {
                        OnName = (eventSender, s) =>
                        {
                            Console.WriteLine("seat name " + s);
                        }
                    });
                    break;
                }
            case "wl_compositor":
                {
                    compositor = WlCompositor.Bind(sender, name, Math.Min(version, 6));
                    break;
                }
            case "xdg_wm_base":
                {
                    xdgWmBase = XdgWmBase.Bind(sender, name, Math.Min(version, 6), new XdgWmBase.Listener.Relay()
                    {
                        OnPing = (sender, serial) =>
                        {
                            sender.Pong(serial);
                        }
                    });
                    break;
                }
        }
    }
});

display.Roundtrip();

var surface = compositor!.CreateSurface();
var task = new TaskCompletionSource();
var xdgSurface = xdgWmBase!.GetXdgSurface(surface, new XdgSurface.Listener.Relay()
{
    OnConfigure = (sender, serial) =>
    {
        sender.AckConfigure(serial);
        task.TrySetResult();
    }
});
var xdgTopLevel = xdgSurface.GetToplevel();
xdgTopLevel.SetAppId("net.rs64.WebGPU.TestApp");

surface.Commit();

while (task.Task.IsCompleted is false)
{
    display.Dispatch();
}

using var instance = WebGpu.CreateInstance();

var webGPUSurface = CreateFromWaylandSurface(instance, display, surface);

using var adapter = await instance.RequestAdapter(new() { CompatibleSurface = webGPUSurface, });
using var device = await adapter.RequestRequestDevice();


var surfaceCompatibility = webGPUSurface.GetCapabilities(adapter);
if (surfaceCompatibility.Formats.Any(f => f is WebGPUTextureFormat.Rgba8UnormSrgb) is false) { throw new Exception("unsupported surface"); }
var surfaceFormat = WebGPUTextureFormat.Rgba8UnormSrgb;
Console.WriteLine(surfaceFormat);
webGPUSurface.Configure(new()
{
    Device = device,
    Format = surfaceFormat,
    Width = 512,
    Height = 512
});


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
                Format = surfaceFormat,
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

var wlTask = Task.Run(() => { while (true) { display.Dispatch(); } });
var v = 0.0;
var u = true;
while (wlTask.IsCompleted is false)
{
    {
        Console.WriteLine(v);
        using var currentSurface = webGPUSurface.GetCurrentTexture();
        Console.WriteLine(currentSurface.Status);
        if (currentSurface.Status is WebGPUSurfaceGetCurrentTextureStatus.Timeout or WebGPUSurfaceGetCurrentTextureStatus.Outdated or WebGPUSurfaceGetCurrentTextureStatus.Lost or WebGPUSurfaceGetCurrentTextureStatus.Error)
        {
            break;
        }
        using var surfaceTex = currentSurface.Texture;
        using var texView = surfaceTex.CreateView(new());

        using var encoder = device.CreateCommandEncoder();
        using (var pss = encoder.BeginRenderPass(new()
        {
            ColorAttachments = [new(){
                View = texView,
                LoadOp = WebGPULoadOp.Clear,
                StoreOp = WebGPUStoreOp.Store,
                ClearValue = new(v,1.0,1.0,1),
            }]
        }))
        {
            pss.SetPipeline(pipeline);
            pss.SetVertexBuffer(0, vertexBuffer, 0, 36);
            pss.Draw(3, 1, 0, 0);
        }

        using var commandBuffer = encoder.Finish();
        queue.Submit([commandBuffer]);

        webGPUSurface.Present();
    }

    if (u)
    {
        v += 0.1;
        if (v > 1.0) { u = false; }
    }
    else
    {
        v -= 0.1;
        if (v < 0.0) { u = true; }
    }

    await Task.Delay(100);
}



static WebGPUSurface CreateFromWaylandSurface(WebGPUInstance instance, WlDisplay display, WlSurface surface)
{
    unsafe
    {
        return instance.CreateSurface(new WebGPUWaylandSurfaceDescriptor()
        {
            Label = "wayland surface",
            Display = (void*)display.Handle,
            Surface = (void*)surface.Handle
        });
    }
}
