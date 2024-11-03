﻿using OpenHarmony.Sdk.Native;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Entry;

public class napi_init
{
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)], EntryPoint = "RegisterEntryModule")]
    public unsafe static void RegisterEntryModule()
    {
        var moduleName = "avalonianative";
        var moduleNamePtr = Marshal.StringToHGlobalAnsi(moduleName);
        napi_module demoModule = new napi_module
        {
            nm_version = 1,
            nm_flags = 0,
            nm_filename = null,
            nm_modname = (sbyte*)moduleNamePtr,
            nm_priv = null,
            napi_addon_register_func = &Init,
            reserved_0 = null,
            reserved_1 = null,
            reserved_2 = null,
            reserved_3 = null,
        };

        ace_napi.napi_module_register(&demoModule);
    }


    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public unsafe static napi_value* Init(napi_env* env, napi_value* exports)
    {
        napi_value* exportInstance = null;
        OH_NativeXComponent* nativeXComponent = null;
        int ret = default;
        var xcomponentName = "__NATIVE_XCOMPONENT_OBJ__";
        var xcomponentNamePtr = Marshal.StringToHGlobalAnsi(xcomponentName);
        if (ace_napi.napi_get_named_property(env, exports, (sbyte*)xcomponentNamePtr, &exportInstance) == napi_status.napi_ok)
        {
            if (ace_napi.napi_unwrap(env, exportInstance, (void**)&nativeXComponent) == napi_status.napi_ok)
            {
                var p = Marshal.AllocHGlobal(sizeof(OH_NativeXComponent_Callback));
                ref var g_ComponentCallback = ref Unsafe.AsRef<OH_NativeXComponent_Callback>((void*)p);
                g_ComponentCallback.OnSurfaceCreated = &XComponentEntry.OnSurfaceCreated;
                g_ComponentCallback.OnSurfaceChanged = &XComponentEntry.OnSurfaceChanged;
                g_ComponentCallback.OnSurfaceDestroyed = &XComponentEntry.OnSurfaceDestroyed;
                g_ComponentCallback.DispatchTouchEvent = &XComponentEntry.DispatchTouchEvent;
                ace_ndk.OH_NativeXComponent_RegisterCallback(nativeXComponent, (OH_NativeXComponent_Callback*)p);
            }
        }
        Marshal.FreeHGlobal(xcomponentNamePtr);
        return exports;
    }
}