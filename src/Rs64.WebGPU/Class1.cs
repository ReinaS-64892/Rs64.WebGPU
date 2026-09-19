// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FFI = Rs64.WebGPU.FFI;

namespace Rs64.WebGPU;

public class Class1
{
    internal static void Run()
    {
        var wgpuPath = "../../lib/wgpu-native/target/debug/libwgpu_native.so";
        var lib = NativeLibrary.Load(wgpuPath);
        FFI.Webgpu.LoadLibrary(lib);
        unsafe
        {
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

            var instanceDesc = new FFI.WGPUInstanceDescriptor();

            var instance = FFI.Webgpu.FnWgpuCreateInstance!(&instanceDesc);
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

            FFI.WGPUInstance.FnWgpuInstanceRequestAdapter!(instance, null, callBackInfo);
            Console.WriteLine("call FnWgpuInstanceRequestAdapter");
            while (CallbackReceiver.s_adapter == null)
            {

                FFI.WGPUInstance.FnWgpuInstanceProcessEvents!(instance);
                Console.WriteLine("call FnWgpuInstanceProcessEvents");
            }

            var adapter = CallbackReceiver.s_adapter;
            CallbackReceiver.s_adapter = null;

            FFI.WGPUAdapterInfo wGPUAdapterInfo = default;
            var adapterInfoGetResult = FFI.WGPUAdapter.FnWgpuAdapterGetInfo!(adapter, &wGPUAdapterInfo);
                Console.WriteLine("call FnWgpuAdapterGetInfo");
            if (adapterInfoGetResult is FFI.WGPUStatus.Success)
            {
                Console.WriteLine("-- read wGPUAdapterInfo --");
                Console.WriteLine(ReadString(wGPUAdapterInfo.Vendor));
                Console.WriteLine(ReadString(wGPUAdapterInfo.Architecture));
                Console.WriteLine(ReadString(wGPUAdapterInfo.Device));
                Console.WriteLine(ReadString(wGPUAdapterInfo.Description));
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
            
            FFI.WGPUAdapterInfo.FnWgpuAdapterInfoFreeMembers!(wGPUAdapterInfo);
            Console.WriteLine("call FnWgpuAdapterInfoFreeMembers");
            FFI.WGPUAdapter.FnWgpuAdapterRelease!(adapter);
            Console.WriteLine("call FnWgpuAdapterRelease");
            FFI.WGPUInstance.FnWgpuInstanceRelease!(instance);
            Console.WriteLine("call FnWgpuInstanceRelease");

            Console.WriteLine("end ffi call");
        }
    }
    private unsafe static string? ReadString(FFI.WGPUStringView message)
    {
        if (message.StringData is not null && message.Length is not 0)
        {
            if (message.Length == FFI.Webgpu.WGPU_STRLEN)
            {
                return Marshal.PtrToStringUTF8((nint)message.StringData);
            }
            else
            {
                return Marshal.PtrToStringUTF8((nint)message.StringData, (int)message.Length);
            }
        }
        return null;
    }
    unsafe class CallbackReceiver
    {
        // すごいアンセーフかもしれない()
        public static FFI.WGPUAdapter* s_adapter;
        [UnmanagedCallersOnly]
        public static unsafe void CallBack(FFI.WGPURequestAdapterStatus status, [FFI.WebGPUPassedWithOwnership(true)] FFI.WGPUAdapter* adapter, [FFI.WebGPUOutString, FFI.WebGPUPassedWithOwnership(false)] FFI.WGPUStringView message)
        {
            Console.WriteLine(status);
            ReadString(message);
            if (status is not FFI.WGPURequestAdapterStatus.Success) { return; }
            s_adapter = adapter;
        }

    }
}
