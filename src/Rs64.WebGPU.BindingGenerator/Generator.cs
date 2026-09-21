// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0
namespace Rs64.WebGPU.BindingGenerator;

public static partial class Generator
{
    const string WEBGPU_H_PATH = "../../lib/webgpu-headers/webgpu.json";
    const string OUTPUT_PATH = "../Rs64.WebGPU.FFI/GeneratedCode";
    const string WGPU = "wgpu";
    const string WGPU_UP = "WGPU";
    const string FN_WGPU = "FnWgpu";
    const string LIB_NAME_DEF = "RS64_WEBGPU_LIB";
    public static async Task Run()
    {

        var json = File.OpenRead(WEBGPU_H_PATH);
        var webGPUJson = await System.Text.Json.JsonSerializer.DeserializeAsync<WebGPUJson>(json);
        if (webGPUJson is null) { return; }

        Console.WriteLine(webGPUJson.Name!);
        Console.WriteLine(webGPUJson.Copyright!);

        if (Directory.Exists(OUTPUT_PATH)) { Directory.Delete(OUTPUT_PATH, true); }
        Directory.CreateDirectory(OUTPUT_PATH);
        await Task.Delay(1000);


        File.WriteAllText(Path.Combine(OUTPUT_PATH, "Copyright.cs"),
            CODE_TEMPLATE +
            "// this codes is generate from webgpu.json \n// \n" +
            "// " + string.Join("\n// ", webGPUJson.Copyright.Split("\n"))
        );
        File.WriteAllText(Path.Combine(OUTPUT_PATH, "InternalVisibleTo.cs"),
            CODE_TEMPLATE_INTERNAL_VISIBLE
        );

        var ctx = new CodeGenContext(webGPUJson, new());

        foreach (var dec in
            GenerateStruct(ctx)
            .Concat(GenerateObjects(ctx))
            .Concat(GenerateEnum(ctx))
            .Concat(GenerateBitFlag(ctx))
            .Concat(GenerateCallbacks(ctx))
            .Append(GenerateFunctions(ctx))
            .Append(GenerateConstants(ctx))
            .ToArray() // ここで ToArray することでこれまでのやつを全部その場で評価させる
            // .Append(GenerateLibraryInitializer(ctx))
            .Append(GenerateLibraryNameSelector(ctx))
            // .Append(GenerateLibraryImporter(ctx))
        )
        {
            File.WriteAllText(Path.Combine(OUTPUT_PATH, dec.filename), dec.contents);
        }
    }


    enum Backend
    {
        Wgpu,
        Dawn,
    }

    class CodeGenContext
    {
        public WebGPUJson WebGPUJson;
        public List<DllExportTo> DllExportToList;

        public CodeGenContext(WebGPUJson webGPUJson, List<DllExportTo> dllExportToList)
        {
            WebGPUJson = webGPUJson;
            DllExportToList = dllExportToList;
        }

        public record DllExportTo(string targetTypeName, string delegateName, string filedName);
    }
}
