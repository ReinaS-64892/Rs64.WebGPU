// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Linq;
using System.Threading.Tasks;

namespace Rs64.WebGPU;

[FFINote(typeof(FFI.WGPUAdapter))]
public partial class WebGPUAdapter : IDisposable
{
    internal WGPUObjectHolder<FFI.WGPUAdapter> Native { get; }
    internal WebGPUAdapter(WGPUObjectHolder<FFI.WGPUAdapter> holder) { Native = holder; }
    public void Dispose() { Native.Dispose(); }

    public WebGPULimits GetLimits()
    {
        WebGPULimits limits;
        unsafe
        {
            FFI.WGPULimits ffiLimits = default;
            FFI.WGPUAdapter.wgpuAdapterGetLimits(Native.GetPtr(), &ffiLimits);
            limits = WebGPULimits.ToW(ffiLimits);
        }
        return limits;
    }

    public bool HasFeature(WebGPUFeatureName featureName)
    {
        unsafe { return (bool)FFI.WGPUAdapter.wgpuAdapterHasFeature(Native.GetPtr(), featureName.ToF()); }
    }
    public bool HasAllFeature(WebGPUFeatureName[] featureNames) { return featureNames.All(HasFeature); }

    public WebGPUFeatureName[] GetFeatures()
    {
        unsafe
        {
            FFI.WGPUSupportedFeatures supportedFeatures = default;
            FFI.WGPUAdapter.wgpuAdapterGetFeatures(Native.GetPtr(), &supportedFeatures);

            var ffiFeatureNames = new Span<FFI.WGPUFeatureName>(supportedFeatures.Features, (int)supportedFeatures.FeaturesCount);
            var featureNames = ffiFeatureNames.ToArray().Select(WebGPUFeatureNameUtil.ToW).ToArray();

            FFI.WGPUSupportedFeatures.FreeMembers(ref supportedFeatures);
            return featureNames;
        }
    }
    public WebGPUAdapterInfo? GetInfo()
    {
        unsafe
        {
            FFI.WGPUAdapterInfo adapterInfoFfi = default;
            var status = FFI.WGPUAdapter.wgpuAdapterGetInfo(Native.GetPtr(), &adapterInfoFfi);
            try
            {
                if (status is FFI.WGPUStatus.Success)
                {
                    return WebGPUAdapterInfo.ToW(adapterInfoFfi);
                }
                else { return null; }
            }
            finally
            {
                FFI.WGPUAdapterInfo.FreeMembers(ref adapterInfoFfi);
            }
        }
    }


    public Task<WebGPUDevice> RequestRequestDevice(WebGPUDeviceDescriptor? requestDeviceDescriptor = null)
    {
        requestDeviceDescriptor ??= new();
        var callBack = new RequestDeviceCallBack(new TaskCompletionSource<WebGPUDevice>());
        unsafe
        {
            var ffiCallBack = new FFI.WGPURequestDeviceCallbackInfo
            {
                CallBackMode = FFI.WGPUCallbackMode.AllowSpontaneous,
                WGPURequestDeviceCallback = &FFI.WGPURequestDeviceCallbackManagedWrapper.CallBack,
                UserData1 = FFI.WGPURequestDeviceCallbackManagedWrapper.CreateUserData(callBack)
            };
            var uncapturedErrorCallback = new UncapturedErrorCallback();
            var ffiUncapturedErrorCallback = new FFI.WGPUUncapturedErrorCallbackInfo()
            {
                WGPUUncapturedErrorCallback = &FFI.WGPUUncapturedErrorCallbackManagedWrapper.CallBack,
                UserData1 = FFI.WGPUUncapturedErrorCallbackManagedWrapper.CreateUserData(uncapturedErrorCallback)
            };
            var uncapturedErrorCallbackUserData = ffiUncapturedErrorCallback.UserData1;
            var deviceLostCallback = new DeviceLostCallback(() =>
            {
                var gcHandle = System.Runtime.InteropServices.GCHandle.FromIntPtr((nint)uncapturedErrorCallbackUserData);
                gcHandle.Free();
            });
            var ffiDeviceLostCallBack = new FFI.WGPUDeviceLostCallbackInfo()
            {
                CallBackMode = FFI.WGPUCallbackMode.AllowSpontaneous,
                WGPUDeviceLostCallback = &FFI.WGPUDeviceLostCallbackManagedWrapper.CallBack,
                UserData1 = FFI.WGPUDeviceLostCallbackManagedWrapper.CreateUserData(deviceLostCallback)
            };
            FFI.WGPUDeviceDescriptor desc = default;
            var ffiReqFeatures = requestDeviceDescriptor.RequiredFeatures.Select(WebGPUFeatureNameUtil.ToF).ToArray();
            FFI.WGPULimits ffiLimits = requestDeviceDescriptor.RequiredLimits is not null ? WebGPULimits.ToF(requestDeviceDescriptor.RequiredLimits) : default;

            fixed (byte* deviceLabelPtr = FFI.WGPUStringView.ConvertWGPUStringParts(requestDeviceDescriptor.Label, out var dLength))
            fixed (byte* queueLabelPtr = FFI.WGPUStringView.ConvertWGPUStringParts(requestDeviceDescriptor.DefaultQueue.Label, out var qLength))
            fixed (FFI.WGPUFeatureName* reqFeatureNames = ffiReqFeatures)
            {
                desc.Label = new FFI.WGPUStringView(deviceLabelPtr, dLength);
                desc.RequiredFeaturesCount = (nuint)ffiReqFeatures.Length;
                desc.RequiredFeatures = reqFeatureNames;
                desc.RequiredLimits = requestDeviceDescriptor.RequiredLimits is not null ? &ffiLimits : null;
                desc.DefaultQueue = new FFI.WGPUQueueDescriptor() { Label = new FFI.WGPUStringView(queueLabelPtr, qLength) };
                desc.DeviceLostCallbackInfo = ffiDeviceLostCallBack;
                desc.UncapturedErrorCallbackInfo = ffiUncapturedErrorCallback;

                _ = FFI.WGPUAdapter.wgpuAdapterRequestDevice(Native.GetPtr(), &desc, ffiCallBack);
            }
        }
        return callBack.Task.Task;
    }
}
[FFINote(typeof(FFI.WGPUAdapterInfo))]
public class WebGPUAdapterInfo
{
    public string? Vendor { get; internal set; }
    public string? Architecture { get; internal set; }
    public string? Device { get; internal set; }
    public string? Description { get; internal set; }
    public WebGPUBackendType? BackendType { get; internal set; } = null;
    public WebGPUAdapterType AdapterType { get; internal set; } = WebGPUAdapterType.Unknown;
    public uint VendorId { get; internal set; }
    public uint DeviceId { get; internal set; }
    public uint SubgroupMinSize { get; internal set; }
    public uint SubgroupMaxSize { get; internal set; }

    internal static WebGPUAdapterInfo ToW(FFI.WGPUAdapterInfo adapterInfoFfi)
    {
        return new WebGPUAdapterInfo
        {
            Vendor = adapterInfoFfi.Vendor.ReadStringView(),
            Architecture = adapterInfoFfi.Architecture.ReadStringView(),
            Device = adapterInfoFfi.Device.ReadStringView(),
            Description = adapterInfoFfi.Description.ReadStringView(),
            BackendType = adapterInfoFfi.BackendType.ToW(),
            AdapterType = adapterInfoFfi.AdapterType.ToW(),
            VendorId = adapterInfoFfi.VendorId,
            DeviceId = adapterInfoFfi.DeviceId,
            SubgroupMinSize = adapterInfoFfi.SubgroupMinSize,
            SubgroupMaxSize = adapterInfoFfi.SubgroupMaxSize
        };
    }
}

[FFINote(typeof(FFI.WGPUDeviceDescriptor))]
public class WebGPUDeviceDescriptor
{
    public string Label = "";
    public WebGPUFeatureName[] RequiredFeatures = [];
    public WebGPULimits? RequiredLimits;
    public WebGPUQueueDescriptor DefaultQueue = new();
}
[FFINote(typeof(FFI.WGPUQueueDescriptor))]
public class WebGPUQueueDescriptor
{
    public string Label = "";
}
