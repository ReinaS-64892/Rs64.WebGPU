// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0
namespace Rs64.WebGPU.BindingGenerator;

public static partial class Generator
{
    const string CODE_TEMPLATE_INTERNAL_VISIBLE =
"""
// this code is generated code !
// do not modify
using System.Runtime.CompilerServices;


[assembly:InternalsVisibleTo("Rs64.WebGPU")]
""";

    const string CODE_TEMPLATE =
"""
// this code is generated code !
// do not modify
using System;
using System.Runtime.InteropServices;

namespace Rs64.WebGPU.FFI;

""";
    const string DOC_COMMENT = "/// ";
    const string DOC_BEGIN = DOC_COMMENT + "<summary>";
    const string DOC_END = DOC_COMMENT + "</summary>";
    const string DOC_RET_BEGIN = "<returns>";
    const string DOC_RET_END = "</returns>";


    const string WP_BOOL = "bool";

    const string WP_NULLABLE_STRING = "nullable_string";
    const string WP_STRING_WITH_DEFAULT_EMPTY = "string_with_default_empty";
    const string WP_OUT_STRING = "out_string";

    const string WP_UINT16 = "uint16";
    const string WP_UINT32 = "uint32";
    const string WP_UINT64 = "uint64";
    const string WP_USIZE = "usize";
    const string WP_INT16 = "int16";
    const string WP_INT32 = "int32";
    const string WP_FLOAT32 = "float32";

    const string WP_NULLABLE_FLOAT32 = "nullable_float32";
    const string WP_FLOAT64 = "float64";
    const string WP_FLOAT64_SUPERTYPE = "float64_supertype";


    const string WGPU_STRING_VIEW = "WGPUStringView";

    const string WP_C_VOID_ = "c_void";
    const string WP_C_VOID_DATA_PTR = "c_void_data_ptr";
    const string WP_C_VOID_MAPPED_RANGE_PTR = "c_void_mapped_range_ptr";
    const string WP_C_VOID_A_NATIVE_WINDOW = "c_void_a_native_window";
    const string WP_C_VOID_CA_METAL_LAYER = "c_void_ca_metal_layer";
    const string WP_C_VOID_H_INSTANCE = "c_void_h_instance";
    const string WP_C_VOID_H_WND = "c_void_h_wnd";
    const string WP_C_VOID_WL_DISPLAY = "c_void_wl_display";
    const string WP_C_VOID_WL_SURFACE = "c_void_wl_surface";
    const string WP_C_VOID_X11_DISPLAY = "c_void_x11_display";
    const string WP_C_VOID_XCB_CONNECTION = "c_void_xcb_connection";
    const string WGPU_FUTURE = "WGPUFuture";
    static string TypeNameTranslate(string typeID)
    {
        if (IsArray(typeID))
        {
            return TypeNameTranslate(typeID.TrimStart(ARRAY_BEGIN).TrimEnd(ARRAY_END).ToString());
        }

        var nameSpace = typeID.Split('.');
        if (nameSpace.Length is 1)
        {
            switch (nameSpace[0])
            {
                case WP_BOOL: return WGPU_UP + WP_BOOL.ToPascalCase();

                case WP_NULLABLE_STRING:
                case WP_STRING_WITH_DEFAULT_EMPTY:
                case WP_OUT_STRING:
                    return WGPU_STRING_VIEW;

                case WP_C_VOID_:
                case WP_C_VOID_DATA_PTR:
                case WP_C_VOID_MAPPED_RANGE_PTR:
                case WP_C_VOID_A_NATIVE_WINDOW:
                case WP_C_VOID_CA_METAL_LAYER:
                case WP_C_VOID_H_INSTANCE:
                case WP_C_VOID_H_WND:
                case WP_C_VOID_WL_DISPLAY:
                case WP_C_VOID_WL_SURFACE:
                case WP_C_VOID_X11_DISPLAY:
                case WP_C_VOID_XCB_CONNECTION:
                    return "void";

                case WP_UINT16: return "ushort";
                case WP_UINT32: return "uint";
                case WP_UINT64: return "ulong";
                case WP_USIZE: return "nuint";
                case WP_INT16: return "short";
                case WP_INT32: return "int";

                case WP_FLOAT32: return "float";
                case WP_NULLABLE_FLOAT32: return "float";

                case WP_FLOAT64: return "double";
                case WP_FLOAT64_SUPERTYPE: return "double";
            }
        }
        else
        {
            switch (nameSpace[0])
            {
                case "object":
                case "struct":
                case "enum":
                case "bitflag":
                    {
                        return WGPU_UP + nameSpace[1].ToPascalCase();
                    }
                case "callback":
                    {
                        return WGPU_UP + nameSpace[1].ToPascalCase() + "CallbackInfo";
                    }
                case "constant":
                    {
                        return "Webgpu." + WGPU_UP + "_" + nameSpace[1].ToUpper();
                    }
            }
        }
        return "ERROR";
    }
    static bool IsStringTypeID(string typeID)
    {
        switch (typeID)
        {
            default:
                return false;

            case WP_NULLABLE_STRING:
            case WP_STRING_WITH_DEFAULT_EMPTY:
            case WP_OUT_STRING:
                return true;
        }
    }
    static string ExtendTypeNameTranslate(string typeName)
    {
        return TypeNameTranslate("struct." + typeName);
    }
    const string ARRAY_BEGIN = "array<";
    const string ARRAY_END = ">";
    private static bool IsArray(string typeID)
    {
        return typeID.StartsWith(ARRAY_BEGIN) && typeID.EndsWith(ARRAY_END);
    }
    private static bool IsObject(string typeID)
    {
        return typeID.StartsWith("object");
    }
    private static bool IsNullableFloat32(string typeID)
    {
        return typeID is WP_NULLABLE_FLOAT32;
    }
    private static bool IsFloat64SuperType(string typeID)
    {
        return typeID is WP_FLOAT64_SUPERTYPE;
    }


    private static void WriteDocument(StringBuilder strBuild, string? document)
    {
        if (string.IsNullOrWhiteSpace(document) is true
            || document is "TODO\n"
            || document is "TODO"
        ) { return; }
        strBuild.AppendLine(DOC_BEGIN);
        strBuild.AppendLine(DOC_COMMENT + string.Join("\n" + DOC_COMMENT, document.TrimEnd().Split("\n")));
        strBuild.AppendLine(DOC_END);
    }

    private static void WriteReturnDocument(StringBuilder strBuild, string? returnDocument)
    {
        if (string.IsNullOrWhiteSpace(returnDocument) is true || returnDocument is "TODO\n") { return; }
        strBuild.AppendLine(DOC_COMMENT + DOC_RET_BEGIN);
        strBuild.AppendLine(DOC_COMMENT + string.Join("\n" + DOC_COMMENT, returnDocument.TrimEnd().Split("\n")));
        strBuild.AppendLine(DOC_COMMENT + DOC_RET_END);
    }


    private static void WriteOptional(StringBuilder strBuild, bool? optional, string? spAttribute = null)
    {
        if (optional ?? false)
        {
            if (string.IsNullOrWhiteSpace(spAttribute)) { strBuild.AppendLine("[WebGPUNullable]"); }
            else { strBuild.AppendLine($"[{spAttribute}: WebGPUNullable]"); }
        }
    }

    private static void WritePassedWithOwnership(StringBuilder strBuild, bool? passedWithOwnership, string? spAttribute = null)
    {
        if (passedWithOwnership is not null)
        {
            var val = passedWithOwnership.Value ? "true" : "false";
            if (string.IsNullOrWhiteSpace(spAttribute)) { strBuild.AppendLine($"[WebGPUPassedWithOwnership({val})]"); }
            else { strBuild.AppendLine($"[{spAttribute}: WebGPUPassedWithOwnership({val})]"); }
        }
    }
    private static string TranslateEnumEntryName(string name)
    {
        if (name.Length is 0) { return name; }
        if (char.IsDigit(name[0]))
        {
            return "W_" + name.ToPascalCase();
        }
        else
        {
            return name.ToPascalCase();
        }
    }

    private static void WriteNameSpace(StringBuilder strBuild, string? nameSpace)
    {
        if (string.IsNullOrWhiteSpace(nameSpace)) { return; }
        strBuild.AppendLine($"[WebGPUNameSpace(\"{nameSpace}\")]");
    }

    private static void WritePointerMutation(StringBuilder strBuild, WebGPUJson.WebGPUJsonPointer pointer, string? spAttribute = null)
    {
        switch (pointer)
        {
            case WebGPUJson.WebGPUJsonPointer.Immutable:
                {
                    if (string.IsNullOrWhiteSpace(spAttribute))
                    { strBuild.AppendLine("[WebGPUImmutablePointer]"); }
                    else { strBuild.AppendLine($"[{spAttribute}: WebGPUImmutablePointer]"); }
                    break;
                }
            case WebGPUJson.WebGPUJsonPointer.Mutable:
                {
                    if (string.IsNullOrWhiteSpace(spAttribute))
                    { strBuild.AppendLine("[WebGPUMutablePointer]"); }
                    else { strBuild.AppendLine($"[{spAttribute}: WebGPUMutablePointer]"); }
                    break;
                }
        }
    }

    private static void WriteStringType(StringBuilder strBuild, string typeID)
    {
        switch (typeID)
        {
            case WP_OUT_STRING:
                {
                    strBuild.AppendLine("[WebGPUOutString]");
                    break;
                }
            case WP_STRING_WITH_DEFAULT_EMPTY:
                {
                    strBuild.AppendLine("[WebGPUStringWithDefaultEmpty]");
                    break;
                }
            case WP_NULLABLE_STRING:
                {
                    strBuild.AppendLine("[WebGPUNullableString]");
                    break;
                }
        }
    }

    private static void WriteNextInChain(StringBuilder strBuild)
    {
        strBuild.AppendLine("public WGPUChainedStruct* NextInChain;");
    }

}

internal static class StringUtil
{
    internal static string ToPascalCase(this string snakeCase)
    {
        return string.Join(
            "",
            snakeCase.Split("_").Select(s =>
                {
                    if (s.Length is 0) { return s; }
                    return char.ToUpper(s[0]) + s.Substring(1).ToLower();
                }
            )
        );
    }
}
