// SPDX-FileCopyrightText: 2026 Reina_Sakiria
// SPDX-License-Identifier: MPL-2.0

namespace Rs64.WebGPU;

// type とかを飛べる形で書いておくためのノート
[System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = true)]
internal sealed class FFINoteAttribute(object obj) : System.Attribute
{
    public object Obj { get; } = obj;
}
