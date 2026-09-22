// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

namespace Rs64.WebGPU.BindingGenerator;

public static partial class Generator
{
    private static (string delegateName, string FieldName) WriteLibraryFunction(StringBuilder strBuild, WebGPUJson.WebGPUJsonFunction webGpuFunc, string typeNamePrefix = "", string thisArgument = "")
    {
        WriteDocument(strBuild, webGpuFunc.Document);
        WriteReturnDocument(strBuild, webGpuFunc.Returns?.Document);

        WriteNameSpace(strBuild, webGpuFunc.Namespace);

        var functionNameNonPrefix = typeNamePrefix + webGpuFunc.Name.ToPascalCase();
        var functionName = WGPU + functionNameNonPrefix;
        var returnType = "void";
        if (webGpuFunc.Returns is not null)
        {
            WriteOptional(strBuild, webGpuFunc.Returns.Optional, "return");
            WritePassedWithOwnership(strBuild, webGpuFunc.Returns.PassedWithOwnership, "return");

            if (webGpuFunc.Returns.Pointer is not null) WritePointerMutation(strBuild, webGpuFunc.Returns.Pointer.Value, "return");

            var isPointer = webGpuFunc.Returns.Pointer.HasValue;
            isPointer |= IsObject(webGpuFunc.Returns.TypeID);

            returnType = TypeNameTranslate(webGpuFunc.Returns.TypeID) + (isPointer ? "*" : "");
        }

        var argumentsBuilder = new StringBuilder();
        if (string.IsNullOrWhiteSpace(thisArgument) is false)
        {
            argumentsBuilder.AppendLine(thisArgument);
            argumentsBuilder.Append(",");
        }
        foreach (var arg in webGpuFunc.Arguments)
        {
            WriteArgument(argumentsBuilder, arg);
            argumentsBuilder.Append(",");
        }
        if (string.IsNullOrWhiteSpace(webGpuFunc.CallbackTypeID) is false)
        {
            var cb = $"{TypeNameTranslate(webGpuFunc.CallbackTypeID)} callback";
            argumentsBuilder.AppendLine(cb);
            argumentsBuilder.Append(",");
        }
        if (argumentsBuilder.Length is not 0) argumentsBuilder.Remove(argumentsBuilder.Length - 1, 1);

        if (returnType is "void" && string.IsNullOrWhiteSpace(webGpuFunc.CallbackTypeID) is false)
        {
            returnType = WGPU_FUTURE;
        }

        var functionFieldName = FN_WGPU + functionNameNonPrefix;
        // delegate ...
        // strBuild.AppendLine($"public delegate {returnType} {functionName}({argumentsBuilder});");
        // strBuild.AppendLine($"public static {functionName}? {functionFieldName};");

        strBuild.AppendLine($"[LibraryImport(Webgpu.{LIB_NAME_DEF})]");
        strBuild.AppendLine($"public static partial {returnType} {functionName}({argumentsBuilder});");

        strBuild.AppendLine();
        return (functionName, functionFieldName);
    }
    private static string GenerateArguments(WebGPUJson.WebGPUJsonParameterType[] arguments)
    {
        var argumentsBuilder = new StringBuilder();
        foreach (var arg in arguments)
        {
            WriteArgument(argumentsBuilder, arg);

            argumentsBuilder.Append(",");
        }
        if (argumentsBuilder.Length is not 0) argumentsBuilder.Remove(argumentsBuilder.Length - 1, 1);
        return argumentsBuilder.ToString();
    }
    private static void WriteArgument(StringBuilder argumentsBuilder, WebGPUJson.WebGPUJsonParameterType arg)
    {
        argumentsBuilder.AppendLine();

        // argumentsBuilder.AppendLine("// TypeID : " + arg.TypeID);
        if (arg.Default is not null) argumentsBuilder.AppendLine("//Debug Argument Default : " + arg.Name + " = " + arg.Default.ToString());

        WriteDocument(argumentsBuilder, arg.Document);
        WritePassedWithOwnership(argumentsBuilder, arg.PassedWithOwnership);
        WriteOptional(argumentsBuilder, arg.Optional);

        if (IsStringTypeID(arg.TypeID)) { WriteStringType(argumentsBuilder, arg.TypeID); }
        if (IsNullableFloat32(arg.TypeID)) { argumentsBuilder.AppendLine("[WebGPUNullableFloat32]"); }
        if (IsFloat64SuperType(arg.TypeID)) { argumentsBuilder.AppendLine("[WebGPUFloat64SuperType]"); }


        var isPointer = arg.Pointer.HasValue;
        if (isPointer) { WritePointerMutation(argumentsBuilder, arg.Pointer!.Value); }
        isPointer |= IsObject(arg.TypeID);

        argumentsBuilder.AppendLine(TypeNameTranslate(arg.TypeID) + (isPointer ? "*" : "") + " " + arg.Name);
    }
    private static (string filename, string contents) GenerateLibraryNameSelector(CodeGenContext ctx)
    {
        var webGPUJson = ctx.WebGPUJson;
        var strBuild = new StringBuilder();

        var className = webGPUJson.Name.ToPascalCase();
        strBuild.AppendLine(CODE_TEMPLATE);
        strBuild.AppendLine("internal static unsafe partial class " + className);
        strBuild.AppendLine("{");
        strBuild.AppendLine();

        strBuild.AppendLine(
$"""
#if RS64_WEBGPU_BACKEND_WGPU_NATIVE
public const string {LIB_NAME_DEF} = "wgpu_native";
#elif RS64_WEBGPU_BACKEND_DAWN
public const string {LIB_NAME_DEF} = "webgpu_dawn";
#else
// fallback is wgpu-native
public const string {LIB_NAME_DEF} = "wgpu_native";
#endif
"""
);

        strBuild.AppendLine();
        strBuild.AppendLine("}");

        return (className + "LibraryName.cs", strBuild.ToString());
    }
    private static (string filename, string contents) GenerateFunctions(CodeGenContext ctx)
    {
        var webGPUJson = ctx.WebGPUJson;
        var strBuild = new StringBuilder();

        var className = webGPUJson.Name.ToPascalCase();
        strBuild.AppendLine(CODE_TEMPLATE);
        strBuild.AppendLine("internal static unsafe partial class " + className);
        strBuild.AppendLine("{");
        strBuild.AppendLine();

        foreach (var webGpuFunc in webGPUJson.Functions)
        {
            var (dName, fName) = WriteLibraryFunction(strBuild, webGpuFunc);
            ctx.DllExportToList.Add(new(className, dName, fName));
        }

        strBuild.AppendLine();
        strBuild.AppendLine("}");

        return (className + ".cs", strBuild.ToString());
    }
    private static (string filename, string contents) GenerateConstants(CodeGenContext ctx)
    {
        var webGPUJson = ctx.WebGPUJson;
        var strBuild = new StringBuilder();

        var className = webGPUJson.Name.ToPascalCase();
        strBuild.AppendLine(CODE_TEMPLATE);
        strBuild.AppendLine("internal static unsafe partial class " + className);
        strBuild.AppendLine("{");
        strBuild.AppendLine();

        foreach (var constant in webGPUJson.Constants)
        {
            strBuild.AppendLine();
            WriteDocument(strBuild, constant.Document);
            WriteNameSpace(strBuild, constant.Namespace);
            var constName = WGPU_UP + "_" + constant.Name.ToUpper();
            if (constant.Value.HasValue is false)
            {
                strBuild.AppendLine($"public const string {constName} = \"Unknown\";");
            }
            else
            {
                var valueJson = constant.Value.Value;
                switch (valueJson.ValueKind)
                {
                    default:
                        {
                            strBuild.AppendLine($"public const string {constName} = \"Unknown\";");
                            break;
                        }
                    case System.Text.Json.JsonValueKind.String:
                        {
                            var defaultValueID = valueJson.GetString();
                            switch (defaultValueID)
                            {
                                default: break;
                                case "usize_max":
                                    {
                                        strBuild.AppendLine($"public static nuint {constName} => nuint.MaxValue;");
                                        break;
                                    }
                                case "uint32_max":
                                    {
                                        strBuild.AppendLine($"public static uint {constName} => uint.MaxValue;");
                                        break;
                                    }
                                case "uint64_max":
                                    {
                                        strBuild.AppendLine($"public static ulong {constName} => ulong.MaxValue;");
                                        break;
                                    }
                                case "nan":
                                    {
                                        strBuild.AppendLine($"public static float {constName} => float.NaN;");
                                        break;
                                    }
                            }
                            break;
                        }
                    case System.Text.Json.JsonValueKind.Number:
                        {
                            strBuild.AppendLine($"public const ulong {constName} = \"{valueJson.GetUInt64()}\";");
                            break;
                        }
                }
            }
            strBuild.AppendLine();
        }

        strBuild.AppendLine();
        strBuild.AppendLine("}");

        return (className + ".Constants" + ".cs", strBuild.ToString());
    }

    private static (string filename, string contents) GenerateLibraryInitializer(CodeGenContext ctx)
    {
        var webGPUJson = ctx.WebGPUJson;
        var strBuild = new StringBuilder();

        var className = webGPUJson.Name.ToPascalCase();
        strBuild.AppendLine(CODE_TEMPLATE);
        strBuild.AppendLine("internal static unsafe partial class " + className);
        strBuild.AppendLine("{");
        strBuild.AppendLine();
        strBuild.AppendLine();

        strBuild.AppendLine("public static void LoadLibrary(IntPtr lib)");
        strBuild.AppendLine("{");
        strBuild.AppendLine();
        foreach (var export in ctx.DllExportToList)
        {
            var str = "{"
            + $" if(NativeLibrary.TryGetExport(lib, @\"{export.delegateName}\", out var addr))"
            + $" {export.targetTypeName}.{export.filedName} = Marshal.GetDelegateForFunctionPointer<{export.targetTypeName}.{export.delegateName}>(addr);"
            + " }";
            strBuild.AppendLine(str);
        }
        strBuild.AppendLine();
        strBuild.AppendLine("}");

        strBuild.AppendLine();
        strBuild.AppendLine();
        strBuild.AppendLine("}");

        return (className + ".LibraryLoadHelper" + ".cs", strBuild.ToString());
    }

    private static (string filename, string contents) GenerateLibraryImporter(CodeGenContext ctx)
    {
        var webGPUJson = ctx.WebGPUJson;
        var strBuild = new StringBuilder();

        var className = webGPUJson.Name.ToPascalCase();
        strBuild.AppendLine(CODE_TEMPLATE);
        strBuild.AppendLine("internal static unsafe partial class " + className);
        strBuild.AppendLine("{");
        strBuild.AppendLine();
        strBuild.AppendLine();

        strBuild.AppendLine("public static void LoadLibrary(IntPtr lib)");
        strBuild.AppendLine("{");
        strBuild.AppendLine();
        foreach (var export in ctx.DllExportToList)
        {
            var str = "{"
            + $" if(NativeLibrary.TryGetExport(lib, @\"{export.delegateName}\", out var addr))"
            + $" {export.targetTypeName}.{export.filedName} = Marshal.GetDelegateForFunctionPointer<{export.targetTypeName}.{export.delegateName}>(addr);"
            + " }";
            strBuild.AppendLine(str);
        }
        strBuild.AppendLine();
        strBuild.AppendLine("}");

        strBuild.AppendLine();
        strBuild.AppendLine();
        strBuild.AppendLine("}");

        return (className + ".LibraryLoadHelper" + ".cs", strBuild.ToString());
    }
    private static IEnumerable<(string filename, string contents)> GenerateObjects(CodeGenContext ctx)
    {
        var webGPUJson = ctx.WebGPUJson;
        foreach (var objectDef in webGPUJson.Objects)
        {
            var typeNameNoPrefix = objectDef.Name.ToPascalCase();
            var typeName = WGPU_UP + typeNameNoPrefix;
            var strBuild = new StringBuilder(CODE_TEMPLATE + "\n// object\n\n");

            WriteDocument(strBuild, objectDef.Document);
            WriteNameSpace(strBuild, objectDef.Namespace);
            if (objectDef.Extended is not null) { strBuild.AppendLine("// Extended : " + objectDef.Extended.ToString()); }
            strBuild.AppendLine($"internal unsafe partial struct {typeName} : IWGPUObject<{typeName}>");
            strBuild.AppendLine("{");
            strBuild.AppendLine();

            foreach (var method in objectDef.Methods
                .Append(new() { Name = "add_ref" })
                .Append(new() { Name = "release" })
            )
            {
                var (dName, fName) = WriteLibraryFunction(strBuild, method, typeNameNoPrefix, $"{typeName}* thisArgument");
                ctx.DllExportToList.Add(new(typeName, dName, fName));

                strBuild.AppendLine();
                if (dName.EndsWith("AddRef"))
                {
                    strBuild.AppendLine($"public static void AddRef({typeName}* ptr)");
                    strBuild.AppendLine("{");
                    strBuild.AppendLine($"{dName}(ptr);");
                    strBuild.AppendLine("}");
                }
                else if (dName.EndsWith("Release"))
                {
                    strBuild.AppendLine($"public static void Release({typeName}* ptr)");
                    strBuild.AppendLine("{");
                    strBuild.AppendLine($"{dName}(ptr);");
                    strBuild.AppendLine("}");

                }
            }



            strBuild.AppendLine();
            strBuild.AppendLine("}");
            strBuild.AppendLine();

            yield return (typeName + ".cs", strBuild.ToString());
        }
    }

    private static IEnumerable<(string filename, string contents)> GenerateStruct(CodeGenContext ctx)
    {
        var webGPUJson = ctx.WebGPUJson;
        foreach (var structDef in webGPUJson.Structs)
        {
            var typeNameNoPrefix = structDef.Name.ToPascalCase();
            var typeName = WGPU_UP + typeNameNoPrefix;
            var strBuild = new StringBuilder(CODE_TEMPLATE + "\n// struct\n\n");

            WriteDocument(strBuild, structDef.Document);

            strBuild.AppendLine($"[WebGPUStructType(WebGPUStructTypeAttribute.WebGPUStructType.{structDef.StructType})]");
            if (structDef.Extends.Any())
            {
                var typeOfs = structDef.Extends.Select(ExtendTypeNameTranslate).Select(t => $"typeof({t})");
                strBuild.AppendLine($"[WebGPUExtensible([{string.Join(",", typeOfs)}])]");
            }
            var havFree = structDef.FreeMembers ?? false;
            if (havFree) strBuild.AppendLine("[WebGPUHaveFreeMembers]");
            WriteNameSpace(strBuild, structDef.Namespace);
            strBuild.AppendLine("[StructLayout(LayoutKind.Sequential)]");
            strBuild.Append($"internal unsafe ref partial struct {typeName}");
            if (havFree) strBuild.AppendLine($" : IWGPUStructHaveFreeMembers<{typeName}>");
            else strBuild.AppendLine();

            strBuild.AppendLine("{");
            switch (structDef.StructType)
            {
                default:
                case WebGPUJson.WebGPUJsonStruct.WebGPUJsonStructType.Standalone:
                    break;
                case WebGPUJson.WebGPUJsonStruct.WebGPUJsonStructType.Extensible:
                    WriteNextInChain(strBuild, true);
                    break;
                case WebGPUJson.WebGPUJsonStruct.WebGPUJsonStructType.Extensible_Callback_Arg:
                    WriteNextInChain(strBuild, true);
                    break;
                case WebGPUJson.WebGPUJsonStruct.WebGPUJsonStructType.Extension:
                    WriteNextInChain(strBuild, false);
                    break;
            }

            foreach (var member in structDef.Members)
            {
                strBuild.AppendLine();

                var memberName = member.Name.ToPascalCase();

                var isArray = IsArray(member.TypeID);
                if (isArray)
                {
                    strBuild.AppendLine("[WebGPUArrayLength]");
                    strBuild.AppendLine($"public nuint {memberName}Count;");
                    strBuild.AppendLine();
                }

                WriteDocument(strBuild, member.Document);

                // strBuild.AppendLine("// TypeID : " + member.TypeID);
                var defaultWriteString = "";
                if (member.Default is not null)
                {
                    var defaultValue = member.Default.Value;
                    strBuild.AppendLine("// Default : " + defaultValue.ToString());
                    switch (defaultValue.ValueKind)
                    {
                        default: break;
                        case System.Text.Json.JsonValueKind.String:
                            {
                                var defaultStr = defaultValue.GetString()!;
                                if (defaultStr.StartsWith("0x"))
                                {
                                    defaultWriteString = " = " + defaultStr;
                                }
                                else if (defaultStr.StartsWith("constant"))
                                {
                                    defaultWriteString = " = " + TypeNameTranslate(defaultStr);
                                }
                                else if (member.TypeID.StartsWith("enum") || member.TypeID.StartsWith("bitflag"))
                                {
                                    defaultWriteString = " = " + TypeNameTranslate(member.TypeID) + "." + TranslateEnumEntryName(defaultStr);
                                }
                                else if (defaultStr is "zero")
                                {
                                    defaultWriteString = " = default";
                                }
                                else
                                {
                                    defaultWriteString = " = " + "@\"" + defaultStr + "\"";
                                }
                                break;
                            }
                        case System.Text.Json.JsonValueKind.Number: { defaultWriteString = " = " + defaultValue.ToString(); break; }
                        case System.Text.Json.JsonValueKind.True: { defaultWriteString = " = WGPUBool.WGPU_TRUE"; break; }
                        case System.Text.Json.JsonValueKind.False: { defaultWriteString = " = WGPUBool.WGPU_FALSE"; break; }
                    }
                }


                WriteOptional(strBuild, member.Optional);
                WritePassedWithOwnership(strBuild, member.PassedWithOwnership);

                if (IsStringTypeID(member.TypeID)) { WriteStringType(strBuild, member.TypeID); }
                if (IsNullableFloat32(member.TypeID)) { strBuild.AppendLine("[WebGPUNullableFloat32]"); }
                if (IsFloat64SuperType(member.TypeID)) { strBuild.AppendLine("[WebGPUFloat64SuperType]"); }

                var isPointer = member.Pointer.HasValue;
                if (isPointer) { WritePointerMutation(strBuild, member.Pointer!.Value); }
                isPointer |= IsObject(member.TypeID);

                strBuild.AppendLine($"public {TypeNameTranslate(member.TypeID)}{(isPointer ? "*" : "")} {memberName} {defaultWriteString};");

                strBuild.AppendLine();
            }

            if (havFree)
            {
                var method = new WebGPUJson.WebGPUJsonFunction() { Name = structDef.Name + "_free_members" };
                var (dName, fName) = WriteLibraryFunction(strBuild, method, "", $"{typeName} thisArgument");
                ctx.DllExportToList.Add(new(typeName, dName, fName));
                strBuild.AppendLine();
                strBuild.AppendLine($"public static void FreeMembers(ref {typeName} wgpuStruct)");
                strBuild.AppendLine("{");
                strBuild.AppendLine($"{dName}(wgpuStruct);");
                strBuild.AppendLine("}");
                strBuild.AppendLine();
            }

            strBuild.AppendLine($"public {typeName} (){{}}");
            strBuild.AppendLine("}");

            yield return (typeName + ".cs", strBuild.ToString());
        }
    }

    private static IEnumerable<(string filename, string contents)> GenerateEnum(CodeGenContext ctx)
    {
        var webGPUJson = ctx.WebGPUJson;
        foreach (var enumDef in webGPUJson.Enums)
        {
            var typeName = WGPU_UP + enumDef.Name.ToPascalCase();
            var strBuild = new StringBuilder(CODE_TEMPLATE + "\n// enum\n\n");

            WriteDocument(strBuild, enumDef.Document);
            WriteNameSpace(strBuild, enumDef.Namespace);

            if (enumDef.Extended ?? false) { strBuild.AppendLine("[WebGPUEnumExtended]"); }

            strBuild.AppendLine("internal enum " + typeName + " : int");
            strBuild.AppendLine("{");


            var enumEntry = enumDef.Entries;
            for (var i = 0; enumEntry.Length > i; i += 1)
            {
                var entry = enumEntry[i];
                if (entry is null) { continue; }
                var entryName = TranslateEnumEntryName(entry.Name);
                var value = entry.Value ?? i;
                WriteDocument(strBuild, entry.Document);
                WriteNameSpace(strBuild, entry.Namespace);
                strBuild.AppendLine($"{entryName} = {value},");
            }
            strBuild.AppendLine("}");

            yield return (typeName + ".cs", strBuild.ToString());
        }
    }

    private static IEnumerable<(string filename, string contents)> GenerateBitFlag(CodeGenContext ctx)
    {
        var webGPUJson = ctx.WebGPUJson;
        foreach (var bitFlagDef in webGPUJson.BitFlags)
        {
            var typeName = WGPU_UP + bitFlagDef.Name.ToPascalCase();
            var strBuild = new StringBuilder(CODE_TEMPLATE + "\n// bit flag\n\n");

            WriteDocument(strBuild, bitFlagDef.Document);
            WriteNameSpace(strBuild, bitFlagDef.Namespace);

            if (bitFlagDef.Extended ?? false) { strBuild.AppendLine("[WebGPUEnumExtended]"); }

            strBuild.AppendLine("[Flags]");
            strBuild.AppendLine("internal enum " + typeName + " : ulong");
            strBuild.AppendLine("{");


            var flagEntry = bitFlagDef.Entries;
            for (var i = 0; flagEntry.Length > i; i += 1)
            {
                var entry = flagEntry[i];
                if (entry is null) { continue; }
                var entryName = TranslateEnumEntryName(entry.Name);
                var value = entry.Value ?? (ushort)(i is not 0 ? 1 << (i - 1) : 0);
                WriteDocument(strBuild, entry.Document);
                WriteNameSpace(strBuild, entry.Namespace);

                if (entry.ValueCombination is null)
                {
                    strBuild.AppendLine($"{entryName} = {value},");
                }
                else
                {
                    strBuild.AppendLine($"{entryName} = {string.Join(" | ", entry.ValueCombination.Select(TranslateEnumEntryName))},");
                }
            }
            strBuild.AppendLine("}");

            yield return (typeName + ".cs", strBuild.ToString());
        }
    }
    private static IEnumerable<(string filename, string contents)> GenerateCallbacks(CodeGenContext ctx)
    {
        var webGPUJson = ctx.WebGPUJson;
        foreach (var callBack in webGPUJson.CallBacks)
        {
            var typeName = WGPU_UP + callBack.Name.ToPascalCase() + "Callback";
            var strBuild = new StringBuilder(CODE_TEMPLATE + "\n// callback\n\n");

            {
                WriteDocument(strBuild, callBack.Document);
                WriteNameSpace(strBuild, callBack.Namespace);
                var immediateCallBack = callBack.CallBackStyle is WebGPUJson.WebGPUJsonCallbackStyle.Immediate;
                if (immediateCallBack)
                    strBuild.AppendLine("[WebGPUImmediateCallBack]");

                strBuild.AppendLine("[StructLayout(LayoutKind.Sequential)]");
                strBuild.AppendLine("internal unsafe ref partial struct " + typeName + "Info");
                strBuild.AppendLine("{");
                strBuild.AppendLine();
                WriteNextInChain(strBuild, true);
                strBuild.AppendLine();
                if (immediateCallBack is false)
                    strBuild.AppendLine("public WGPUCallbackMode CallBackMode;");
                strBuild.AppendLine();

                var callBackFullArg = callBack.Arguments.Concat(new WebGPUJson.WebGPUJsonParameterType[]{
                    new(){Name = "userData1",TypeID = "c_void",Optional = true, Pointer = (WebGPUJson.WebGPUJsonPointer?)-1},
                    new(){Name = "userData2",TypeID = "c_void",Optional = true, Pointer = (WebGPUJson.WebGPUJsonPointer?)-1},
                }).ToArray();
                var delegateArg = string.Join(", ", callBackFullArg.Select(a =>
                        {
                            var isPointer = a.Pointer.HasValue;
                            isPointer |= IsObject(a.TypeID);

                            return TypeNameTranslate(a.TypeID) + (isPointer ? "*" : "");
                        }
                    )
                );
                var unmanagedDelegateTypeStr = $"delegate* unmanaged<{delegateArg},void>";
                strBuild.AppendLine($"public {unmanagedDelegateTypeStr} {typeName};");

                strBuild.AppendLine();
                strBuild.AppendLine("[WebGPUNullable]");
                strBuild.AppendLine("public void* UserData1;");
                strBuild.AppendLine();
                strBuild.AppendLine("[WebGPUNullable]");
                strBuild.AppendLine("public void* UserData2;");

                strBuild.AppendLine();
                strBuild.AppendLine("}");

                strBuild.AppendLine();

                //}
                //{

                var arguments = GenerateArguments(callBack.Arguments);
                var fullArg = GenerateArguments(callBackFullArg);

                var interfaceName = $"I{typeName}";

                strBuild.AppendLine("//  managed call back interface");
                strBuild.AppendLine($"internal unsafe interface {interfaceName}");
                strBuild.AppendLine("{");
                strBuild.AppendLine($"void CallBack ({arguments});");
                strBuild.AppendLine("}");

                strBuild.AppendLine();
                strBuild.AppendLine();

                strBuild.AppendLine("// managed wrapper ");
                strBuild.AppendLine($"internal static unsafe class {typeName}ManagedWrapper");
                strBuild.AppendLine("{");
                strBuild.AppendLine();
                strBuild.AppendLine($"public static void* CreateUserData({interfaceName} receiverInterface)");
                strBuild.AppendLine("{");
                strBuild.AppendLine("var interfaceHandle = GCHandle.Alloc(receiverInterface);");
                strBuild.AppendLine("var gcHandlePtr = (void*)GCHandle.ToIntPtr(interfaceHandle);");
                strBuild.AppendLine("return gcHandlePtr;");
                strBuild.AppendLine("}");
                strBuild.AppendLine();
                strBuild.AppendLine("[UnmanagedCallersOnly]");
                strBuild.AppendLine($"public static void CallBack ({fullArg})");
                strBuild.AppendLine("{");
                strBuild.AppendLine($"var gcHandle = GCHandle.FromIntPtr((nint)userData1);");
                strBuild.AppendLine("try{");
                strBuild.AppendLine($"var i =({interfaceName})gcHandle.Target!;");
                strBuild.AppendLine($"i.CallBack({string.Join(",", callBack.Arguments.Select(a => a.Name))});");
                strBuild.AppendLine("}");
                strBuild.AppendLine("finally{");
                if (callBack.Name is not "uncaptured_error")
                    strBuild.AppendLine("gcHandle.Free();");
                // else 解放を行わない、端的にいうと何度でも呼び出せる特別扱い。
                strBuild.AppendLine("}");
                strBuild.AppendLine("}");
                strBuild.AppendLine();
                strBuild.AppendLine("}");
            }
            yield return (typeName + ".cs", strBuild.ToString());
        }
    }


    private static IEnumerable<(string filename, string contents)> SafeWrapper(WebGPUJson webGPUJson)
    {
        foreach (var enumDef in webGPUJson.Enums)
        {
            switch (enumDef.Name)
            {
                default: break;
                case "instance_feature_name":
                case "WGSL_language_feature_name":
                case "feature_level":
                case "power_preference":
                case "backend_type":
                case "texture_format":
                case "composite_alpha_mode":
                case "present_mode":
                case "surface_get_current_texture_status":
                case "status":
                    {
                        var strBuild = new StringBuilder();
                        var typeName = "WebGPU" + enumDef.Name.ToPascalCase();
                        var ffiTypeName = "FFI." + WGPU_UP + enumDef.Name.ToPascalCase();
                        strBuild.AppendLine(
"""
// this code is generated 
// do not edit
namespace Rs64.WebGPU;
"""
                        );

                        strBuild.AppendLine("public enum " + typeName);
                        strBuild.AppendLine("{");
                        bool haveUndefined = false;
                        foreach (var e in enumDef.Entries)
                        {
                            if (e is null) { continue; }
                            if (e.Name is "undefined") { haveUndefined = true; continue; }

                            WriteDocument(strBuild, e.Document);
                            strBuild.AppendLine(TranslateEnumEntryName(e.Name) + ",");
                        }
                        strBuild.AppendLine("}");

                        strBuild.AppendLine($"internal static class {typeName}Util");
                        strBuild.AppendLine("{");

                        strBuild.AppendLine($"public static {ffiTypeName} ToF(this {typeName}{(haveUndefined ? "?" : "")} val)");
                        strBuild.AppendLine("{");

                        strBuild.AppendLine("switch (val)");
                        strBuild.AppendLine("{");
                        strBuild.AppendLine("default: throw new InvalidEnumValueException ();");
                        foreach (var e in enumDef.Entries)
                        {
                            if (e is null) { continue; }

                            var enumEntryName = TranslateEnumEntryName(e.Name);
                            var ffiEntryEnumLiteral = ffiTypeName + "." + enumEntryName;
                            var EntryEnumLiteral = typeName + "." + enumEntryName;
                            if (e.Name is "undefined") { EntryEnumLiteral = "null"; }
                            strBuild.AppendLine($"case {EntryEnumLiteral}: return {ffiEntryEnumLiteral};");
                        }
                        strBuild.AppendLine("}");

                        strBuild.AppendLine("}");

                        strBuild.AppendLine($"public static {typeName}{(haveUndefined ? "?" : "")} ToW(this {ffiTypeName} val)");
                        strBuild.AppendLine("{");

                        strBuild.AppendLine("switch (val)");
                        strBuild.AppendLine("{");
                        strBuild.AppendLine("default: throw new InvalidEnumValueException ();");
                        foreach (var e in enumDef.Entries)
                        {
                            if (e is null) { continue; }

                            var enumEntryName = TranslateEnumEntryName(e.Name);
                            var ffiEntryEnumLiteral = ffiTypeName + "." + enumEntryName;
                            var EntryEnumLiteral = typeName + "." + enumEntryName;
                            if (e.Name is "undefined") { EntryEnumLiteral = "null"; }
                            strBuild.AppendLine($"case {ffiEntryEnumLiteral}: return {EntryEnumLiteral};");
                        }
                        strBuild.AppendLine("}");

                        strBuild.AppendLine("}");

                        strBuild.AppendLine("}");

                        yield return (typeName + ".cs", strBuild.ToString());
                        break;
                    }
            }
        }

        foreach (var bitFlagDef in webGPUJson.BitFlags)
        {
            switch (bitFlagDef.Name)
            {
                default: break;
                case "texture_usage":
                    {
                        var strBuild = new StringBuilder();

                        var typeName = "WebGPU" + bitFlagDef.Name.ToPascalCase();
                        var ffiTypeName = "FFI." + WGPU_UP + bitFlagDef.Name.ToPascalCase();
                        strBuild.AppendLine(
"""
// this code is generated 
// do not edit
namespace Rs64.WebGPU;
using System;
"""
                        );

                        strBuild.AppendLine("[Flags]");
                        strBuild.AppendLine("public enum " + typeName);
                        strBuild.AppendLine("{");
                        foreach (var e in bitFlagDef.Entries)
                        {
                            if (e is null) { continue; }

                            WriteDocument(strBuild, e.Document);
                            strBuild.AppendLine(TranslateEnumEntryName(e.Name) + ",");
                        }
                        strBuild.AppendLine("}");

                        strBuild.AppendLine($"internal static class {typeName}Util");
                        strBuild.AppendLine("{");

                        strBuild.AppendLine($"public static {ffiTypeName} ToF(this {typeName} val)");
                        strBuild.AppendLine("{");
                        strBuild.AppendLine($"var ffiVal = default({ffiTypeName});");

                        foreach (var e in bitFlagDef.Entries)
                        {
                            if (e is null) { continue; }

                            var enumEntryName = TranslateEnumEntryName(e.Name);
                            var ffiEntryEnumLiteral = ffiTypeName + "." + enumEntryName;
                            var EntryEnumLiteral = typeName + "." + enumEntryName;
                            strBuild.AppendLine($"if (val.HasFlag({EntryEnumLiteral}))");
                            strBuild.AppendLine("{");
                            strBuild.AppendLine($"ffiVal |= {ffiEntryEnumLiteral};");
                            strBuild.AppendLine("}");
                        }

                        strBuild.AppendLine($"return ffiVal;");
                        strBuild.AppendLine("}");

                        strBuild.AppendLine($"public static {typeName} ToW(this {ffiTypeName} val)");
                        strBuild.AppendLine("{");

                        strBuild.AppendLine($"var wVal = default({typeName});");

                        foreach (var e in bitFlagDef.Entries)
                        {
                            if (e is null) { continue; }

                            var enumEntryName = TranslateEnumEntryName(e.Name);
                            var ffiEntryEnumLiteral = ffiTypeName + "." + enumEntryName;
                            var EntryEnumLiteral = typeName + "." + enumEntryName;
                            strBuild.AppendLine($"if (val.HasFlag({ffiEntryEnumLiteral}))");
                            strBuild.AppendLine("{");
                            strBuild.AppendLine($"wVal |= {EntryEnumLiteral};");
                            strBuild.AppendLine("}");
                        }

                        strBuild.AppendLine($"return wVal;");
                        strBuild.AppendLine("}");

                        strBuild.AppendLine("}");

                        yield return (typeName + ".cs", strBuild.ToString());
                        break;
                    }
            }
        }
    }

}
