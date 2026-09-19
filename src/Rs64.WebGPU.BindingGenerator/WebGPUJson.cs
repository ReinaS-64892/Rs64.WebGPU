// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rs64.WebGPU.BindingGenerator;

// reference to lib/webgpu-headers/schema.json
public class WebGPUJson
{
    /// <summary>
    /// The license string to include at the top of the generated header
    /// </summary>
    [JsonPropertyName("copyright")]
    public string Copyright { get; set; } = "";

    /// <summary>
    /// The name/namespace of the specification
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    /// <summary>
    /// The dedicated enum prefix for the implementation specific header to avoid collisions
    /// </summary>
    [JsonPropertyName("enum_prefix")]
    public ushort EnumPrefix { get; set; }


    [JsonPropertyName("doc")]
    public string Document { get; set; } = "";



    [JsonPropertyName("__copyright")]
    public string _Copyright { get; set; } = "";

    [JsonPropertyName("_comment")]
    public string _Comment { get; set; } = "";


    // ... ? 
    // [JsonPropertyName("typedefs")]
    // public WebGPUJsonTypedef[] Typedefs { get; set; }

    [JsonPropertyName("constants")]
    public WebGPUJsonConstant[] Constants { get; set; } = [];
    public class WebGPUJsonConstant
    {
        /// <summary>
        /// Name of the constant variable/define
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        /// <summary>
        /// Optional property, specifying the namespace where this constant is defined
        /// </summary>
        [JsonPropertyName("namespace")]
        public string? Namespace { get; set; }

        /// <summary>
        /// An enum of predefined max constants or a 64-bit unsigned integer, or float NaN value.
        /// </summary>
        [JsonPropertyName("value")]
        public JsonElement? Value { get; set; } = null;


        [JsonPropertyName("doc")]
        public string Document { get; set; } = "";
    }

    [JsonPropertyName("enums")]
    public WebGPUJsonEnum[] Enums { get; set; } = [];
    public class WebGPUJsonEnum
    {
        /// <summary>
        /// Name of the enum
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        /// <summary>
        /// Optional property, specifying the namespace where this enum is defined
        /// </summary>
        [JsonPropertyName("namespace")]
        public string? Namespace { get; set; }

        [JsonPropertyName("doc")]
        public string Document { get; set; } = "";

        /// <summary>
        /// Optional property, an indicator that this enum is an extension of an already present enum
        /// </summary>
        [JsonPropertyName("extended")]
        public bool? Extended { get; set; }

        [JsonPropertyName("entries")]
        public WebGPUJsonEnumEntry?[] Entries { get; set; } = [];
        public class WebGPUJsonEnumEntry
        {
            /// <summary>
            /// Name of the enum entry
            /// </summary>
            [JsonPropertyName("name")]
            public string Name { get; set; } = "";

            /// <summary>
            /// Optional property, specifying the namespace where this enum entry is defined, applying both its name prefix and its enum block
            /// </summary>
            [JsonPropertyName("namespace")]
            public string? Namespace { get; set; }

            [JsonPropertyName("doc")]
            public string Document { get; set; } = "";

            /// <summary>
            /// If specified, overrides the default value for the enum entry (which is its index in the list)
            /// </summary>
            [JsonPropertyName("value")]
            public ushort? Value { get; set; }
        }

    }

    [JsonPropertyName("bitflags")]
    public WebGPUJsonBitFlag[] BitFlags { get; set; } = [];
    public class WebGPUJsonBitFlag
    {
        /// <summary>
        /// Name of the bitflag
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        /// <summary>
        /// Optional property, specifying the namespace where this struct is defined
        /// </summary>
        [JsonPropertyName("namespace")]
        public string? Namespace { get; set; }

        [JsonPropertyName("doc")]
        public string Document { get; set; } = "";

        /// <summary>
        /// Optional property, an indicator that this bitflag is an extension of an already present bitflag
        /// </summary>
        [JsonPropertyName("extended")]
        public bool? Extended { get; set; }

        [JsonPropertyName("entries")]
        public WebGPUJsonBigFlagEntry?[] Entries { get; set; } = [];
        public class WebGPUJsonBigFlagEntry
        {
            /// <summary>
            /// Name of the bitflag entry
            /// </summary>
            [JsonPropertyName("name")]
            public string Name { get; set; } = "";
            /// <summary>
            /// Optional property, specifying the namespace where this bitmask entry is defined
            /// </summary>
            [JsonPropertyName("namespace")]
            public string? Namespace { get; set; }

            [JsonPropertyName("doc")]
            public string Document { get; set; } = "";

            /// <summary>
            /// Optional property, a 64-bit unsigned integer
            /// </summary>
            [JsonPropertyName("value")]
            public ulong? Value { get; set; }

            /// <summary>
            /// Optional property, an array listing the names of bitflag entries to be OR-ed
            /// </summary>
            [JsonPropertyName("value_combination")]
            public string[]? ValueCombination { get; set; }
        }
    }
    [JsonPropertyName("structs")]
    public WebGPUJsonStruct[] Structs { get; set; } = [];
    public class WebGPUJsonStruct
    {
        /// <summary>
        /// Name of the structure
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        /// <summary>
        /// Optional property, specifying the namespace where this struct is defined
        /// </summary>
        [JsonPropertyName("namespace")]
        public string? Namespace { get; set; }

        [JsonPropertyName("doc")]
        public string Document { get; set; } = "";

        /// <summary>
        /// Type of the structure
        /// </summary>
        [JsonPropertyName("type")]
        public WebGPUJsonStructType StructType { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter<WebGPUJsonStructType>))]
        public enum WebGPUJsonStructType
        {
            Extensible,
            Extensible_Callback_Arg,
            Extension,
            Standalone,
        }

        /// <summary>
        /// Optional property, list of names of the structs that this extension structure extends
        /// </summary>
        [JsonPropertyName("extends")]
        public string[] Extends { get; set; } = [];

        /// <summary>
        /// Optional property, to indicate if a free members function be emitted for the struct
        /// </summary>
        [JsonPropertyName("free_members")]
        public bool? FreeMembers { get; set; }


        /// <summary>
        /// Optional property, list of struct members
        /// </summary>
        [JsonPropertyName("members")]
        public WebGPUJsonParameterType[] Members { get; set; } = [];
    }


    [JsonPropertyName("callbacks")]
    public WebGPUJsonCallBack[] CallBacks { get; set; } = [];

    [JsonPropertyName("functions")]
    public WebGPUJsonFunction[] Functions { get; set; } = [];

    [JsonPropertyName("objects")]
    public WebGPUJsonObject[] Objects { get; set; } = [];
    public class WebGPUJsonObject
    {
        /// <summary>
        /// Name of the object
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        /// <summary>
        /// Optional property, specifying the namespace where this object is defined
        /// </summary>
        [JsonPropertyName("namespace")]
        public string? Namespace { get; set; }

        [JsonPropertyName("doc")]
        public string Document { get; set; } = "";

        /// <summary>
        /// Optional property, an indicator that this object is an extension of an already present object
        /// </summary>
        [JsonPropertyName("extended")]
        public bool? Extended { get; set; }

        [JsonPropertyName("methods")]
        public WebGPUJsonFunction[] Methods { get; set; } = [];
    }

    public class WebGPUJsonParameterType
    {
        /// <summary>
        /// Parameter name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("doc")]
        public string Document { get; set; } = "";

        /// <summary>
        /// Parameter type
        /// </summary>
        [JsonPropertyName("type")]
        public string TypeID { get; set; } = "";

        /// <summary>
        /// Whether the value is passed with ownership or without ownership
        /// </summary>
        [JsonPropertyName("passed_with_ownership")]
        public bool? PassedWithOwnership { get; set; }

        /// <summary>
        /// Whether the value is passed with ownership or without ownership
        /// </summary>
        [JsonPropertyName("pointer")]
        public WebGPUJsonPointer? Pointer { get; set; }

        /// <summary>
        /// Optional property, to indicate if a parameter is optional
        /// </summary>
        [JsonPropertyName("optional")]
        public bool? Optional { get; set; }

        /// <summary>
        /// Default value assigned to this parameter when using initializer macro. Special context-dependent values include constant names (`constant.*`), bitflag names (unprefixed), and `zero` for struct-zero-init (where zero-init is known to have the desired result).
        /// it may contain a string, a number, or a boolean value ... 
        /// </summary>
        [JsonPropertyName("default")]
        public JsonElement? Default { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter<WebGPUJsonPointer>))]
    public enum WebGPUJsonPointer
    {
        Immutable,
        Mutable,
    }
    public class WebGPUJsonCallBack
    {
        /// <summary>
        /// Callback name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        /// <summary>
        /// Optional property, specifying the namespace where this callback is defined
        /// </summary>
        [JsonPropertyName("namespace")]
        public string? Namespace { get; set; }

        [JsonPropertyName("doc")]
        public string Document { get; set; } = "";

        /// <summary>
        /// Callback style
        /// </summary>
        [JsonPropertyName("style")]
        public WebGPUJsonCallbackStyle CallBackStyle { get; set; }


        /// <summary>
        /// Optional property, list of callback arguments
        /// </summary>
        [JsonPropertyName("args")]
        public WebGPUJsonFunctionParameterType[] Arguments { get; set; } = [];
    }
    [JsonConverter(typeof(JsonStringEnumConverter<WebGPUJsonCallbackStyle>))]
    public enum WebGPUJsonCallbackStyle
    {
        Callback_Mode,
        Immediate,
    }
    public class WebGPUJsonFunctionParameterType : WebGPUJsonParameterType { }

    public class WebGPUJsonFunction
    {
        /// <summary>
        /// Name of the function
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        /// <summary>
        /// Optional property, specifying the namespace where this function is defined
        /// </summary>
        [JsonPropertyName("namespace")]
        public string? Namespace { get; set; }

        [JsonPropertyName("doc")]
        public string Document { get; set; } = "";

        /// <summary>
        /// Optional property, return type of the function
        /// </summary>
        [JsonPropertyName("returns")]
        public WebGPUJsonReturns? Returns { get; set; }
        public class WebGPUJsonReturns
        {
            [JsonPropertyName("doc")]
            public string Document { get; set; } = "";
            /// <summary>
            /// Return type of the function
            /// </summary>
            [JsonPropertyName("type")]
            public string TypeID { get; set; } = "";

            /// <summary>
            /// Indicates if the return type is optional/nullable
            /// </summary>
            [JsonPropertyName("optional")]
            public bool? Optional { get; set; }

            /// <summary>
            /// Whether the value is passed with ownership or without ownership
            /// </summary>
            [JsonPropertyName("passed_with_ownership")]
            public bool? PassedWithOwnership { get; set; }

            /// <summary>
            /// Optional property, specifies if a method return type is a pointer
            /// </summary>
            [JsonPropertyName("pointer")]
            public WebGPUJsonPointer? Pointer { get; set; }
        }

        /// <summary>
        /// Optional property, callback type for async functon
        /// </summary>
        [JsonPropertyName("callback")]
        public string? CallbackTypeID { get; set; }

        /// <summary>
        /// Optional property, list of function arguments
        /// </summary>
        [JsonPropertyName("args")]
        public WebGPUJsonFunctionParameterType[] Arguments { get; set; } = [];
    }
}
