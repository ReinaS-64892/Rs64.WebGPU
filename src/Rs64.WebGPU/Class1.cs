// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FFI = Rs64.WebGPU.FFI;

namespace Rs64.WebGPU;

public class Class1
{
    internal static async Task Run()
    {
        var wgpuPath = "../../lib/wgpu-native/target/debug/libwgpu_native.so";
        // var lib = NativeLibrary.Load(wgpuPath);
        // FFI.Webgpu.LoadLibrary(lib);
        NativeLibrary.SetDllImportResolver(
            typeof(FFI.Webgpu).Assembly,
            (p, _, _) => { if (p is not "wgpu_native") { return IntPtr.Zero; } return NativeLibrary.Load(wgpuPath); }
        );
        unsafe
        {
#pragma warning disable CS9123 // The '&' operator should not be used on parameters or local variables in async methods.
            // FFI.WGPUSupportedInstanceFeatures* features = null;
            // FFI.Webgpu.FnWgpuGetInstanceFeatures!(features);

            // var fc = (ulong)features->FeaturesCount;
            // Console.WriteLine(fc);
            // for (ulong i = 0; fc > i; i += 1)
            // {
            //     var f = features->Features[i];
            //     Console.WriteLine(f);
            // }
            Console.WriteLine("start ffi call");

            // var instanceDesc = new FFI.WGPUInstanceDescriptor();

            var instance = FFI.Webgpu.wgpuCreateInstance(null);
            Console.WriteLine("call FnWgpuCreateInstance");
            if (instance is null) { Console.WriteLine("instance creation failed!"); }


            var option = new FFI.WGPURequestAdapterOptions();
            var callBackInfo = new FFI.WGPURequestAdapterCallbackInfo()
            {
                NextInChain = default,
                CallBackMode = FFI.WGPUCallbackMode.AllowProcessEvents,
                WGPURequestAdapterCallback = &CallbackReceiver.CallBack,
                UserData1 = null,
                UserData2 = null,
            };

            FFI.WGPUInstance.wgpuInstanceRequestAdapter(instance, null, callBackInfo);
            Console.WriteLine("call FnWgpuInstanceRequestAdapter");
            while (CallbackReceiver.s_adapter == null)
            {

                FFI.WGPUInstance.wgpuInstanceProcessEvents(instance);
                Console.WriteLine("call FnWgpuInstanceProcessEvents");
            }

            var adapter = CallbackReceiver.s_adapter;
            CallbackReceiver.s_adapter = null;

            FFI.WGPUAdapterInfo wGPUAdapterInfo = default;
            var adapterInfoGetResult = FFI.WGPUAdapter.wgpuAdapterGetInfo(adapter, &wGPUAdapterInfo);
            Console.WriteLine("call FnWgpuAdapterGetInfo");
            if (adapterInfoGetResult is FFI.WGPUStatus.Success)
            {
                Console.WriteLine("-- read wGPUAdapterInfo --");
                Console.WriteLine(wGPUAdapterInfo.Vendor.ReadStringView());
                Console.WriteLine(wGPUAdapterInfo.Architecture.ReadStringView());
                Console.WriteLine(wGPUAdapterInfo.Device.ReadStringView());
                Console.WriteLine(wGPUAdapterInfo.Description.ReadStringView());
                Console.WriteLine(wGPUAdapterInfo.BackendType);
                Console.WriteLine(wGPUAdapterInfo.AdapterType);
                Console.WriteLine(wGPUAdapterInfo.VendorId);
                Console.WriteLine(wGPUAdapterInfo.DeviceId);
                Console.WriteLine("-- end --");
            }
            else
            {
                Console.WriteLine("Error!!!");
            }

            FFI.WGPUAdapterInfo.wgpuAdapterInfoFreeMembers(wGPUAdapterInfo);
            Console.WriteLine("call FnWgpuAdapterInfoFreeMembers");
            FFI.WGPUAdapter.wgpuAdapterRelease(adapter);
            Console.WriteLine("call FnWgpuAdapterRelease");
            FFI.WGPUInstance.wgpuInstanceRelease(instance);
            Console.WriteLine("call FnWgpuInstanceRelease");

            Console.WriteLine("end ffi call");
#pragma warning restore CS9123 // The '&' operator should not be used on parameters or local variables in async methods.
        }
        Console.WriteLine("--- --- ---");
        {

            Console.WriteLine("start safe ffi call");

            // var instanceDesc = new FFI.WGPUInstanceDescriptor();

            // WebGpu.GetInstanceDescriptor();
            // WebGpu.HasAllInstanceFeature(new WebGpuInstanceDescriptor.Feature() { ShaderSourceSpirv = true, });

            using var instance = WebGpu.CreateInstance();
            Console.WriteLine("call WebGpu.CreateInstance");


            // var option = new FFI.WGPURequestAdapterOptions();

            using var adapter = await instance.RequestAdapter();
            Console.WriteLine("call instance.RequestAdapter");
            // instance.CreateSurface(new WebGPUWaylandSurfaceDescriptor() { Label = "ねこ" });


            // FFI.WGPUAdapterInfo wGPUAdapterInfo = default;
            // var adapterInfoGetResult = FFI.WGPUAdapter.wgpuAdapterGetInfo(adapter, &wGPUAdapterInfo);
            // Console.WriteLine("call FnWgpuAdapterGetInfo");
            // if (adapterInfoGetResult is FFI.WGPUStatus.Success)
            // {
            //     Console.WriteLine("-- read wGPUAdapterInfo --");
            //     Console.WriteLine(ReadString(wGPUAdapterInfo.Vendor));
            //     Console.WriteLine(ReadString(wGPUAdapterInfo.Architecture));
            //     Console.WriteLine(ReadString(wGPUAdapterInfo.Device));
            //     Console.WriteLine(ReadString(wGPUAdapterInfo.Description));
            //     Console.WriteLine(wGPUAdapterInfo.BackendType);
            //     Console.WriteLine(wGPUAdapterInfo.AdapterType);
            //     Console.WriteLine(wGPUAdapterInfo.VendorId);
            //     Console.WriteLine(wGPUAdapterInfo.DeviceId);
            //     Console.WriteLine("-- end --");
            // }
            // else
            // {
            //     Console.WriteLine("Error!!!");
            // }

            // FFI.WGPUAdapterInfo.wgpuAdapterInfoFreeMembers(wGPUAdapterInfo);
            // Console.WriteLine("call FnWgpuAdapterInfoFreeMembers");
            // FFI.WGPUAdapter.wgpuAdapterRelease(adapter);
            // Console.WriteLine("call FnWgpuAdapterRelease");

            Console.WriteLine("end ffi call");
        }
    }
    unsafe class CallbackReceiver
    {
        // すごいアンセーフかもしれない()
        public static FFI.WGPUAdapter* s_adapter;
        [UnmanagedCallersOnly]
        public static unsafe void CallBack(FFI.WGPURequestAdapterStatus status, [FFI.WebGPUPassedWithOwnership(true)] FFI.WGPUAdapter* adapter, [FFI.WebGPUOutString, FFI.WebGPUPassedWithOwnership(false)] FFI.WGPUStringView message, void* ud1, void* ud2)
        {
            Console.WriteLine(status);
            Console.WriteLine(message.ReadStringView());
            if (status is not FFI.WGPURequestAdapterStatus.Success) { return; }
            s_adapter = adapter;
        }

    }
}
