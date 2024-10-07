﻿/*
   Copyright 2024 Alexander Stärk

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System.Reflection;

namespace Basilisque.AutoImplementer.CodeAnalysis;

internal static class AutoImplementerGeneratorData
{

    internal const string C_AUTOIMPLEMENTATTRIBUTE_TARGET_NAMESPACE = "Basilisque.AutoImplementer";
    internal const string C_AUTO_IMPLEMENT_INTERFACE_ATTRIBUTE_CLASSNAME = "AutoImplementInterfaceAttribute";
    internal const string C_AUTO_IMPLEMENT_ON_MEMBERS_ATTRIBUTE_CLASSNAME = "AutoImplementAttribute";
    internal const string C_IMPLEMENT_AS_REQUIRED_ATTRIBUTE_CLASSNAME = "IRequiredAttribute";

    internal const string C_AUTO_IMPLEMENT_INTERFACE_ATTRIBUTE_COMPILATIONNAME = $"{C_AUTO_IMPLEMENT_INTERFACE_ATTRIBUTE_CLASSNAME}.g";
    internal const string C_AUTOIMPLEMENTATTRIBUTE_ON_MEMBERS_COMPILATIONNAME = $"{C_AUTO_IMPLEMENT_ON_MEMBERS_ATTRIBUTE_CLASSNAME}.g";
    internal const string C_IMPLEMENT_AS_REQUIRED_ATTRIBUTE_COMPILATIONNAME = $"{C_IMPLEMENT_AS_REQUIRED_ATTRIBUTE_CLASSNAME}.g";
    internal const string C_REQUIRED_DOTNET_6_PATCH_COMPILATIONNAME = "Patch for required attribute in .NET 6 and below";

    internal static readonly AssemblyName ASSEMBLY_NAME = Assembly.GetExecutingAssembly().GetName();

    internal static readonly string GENERATED_FILE_SHARED_HEADER = @$"//------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by a tool.
//   {ASSEMBLY_NAME.Name}, {ASSEMBLY_NAME.Version}
//   
//   Changes to this file may cause incorrect behavior and will be lost if
//   the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------

#nullable enable

using System;
";

    internal static readonly string GENERATED_CLASS_SHARED_ATTRIBUTES = $@"[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""{ASSEMBLY_NAME.Name}"", ""{ASSEMBLY_NAME.Version}"")]
[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]";

    internal static readonly string AUTO_IMPLEMENT_INTERFACE_ATTRIBUTE_SOURCE =
    @$"{GENERATED_FILE_SHARED_HEADER}
namespace {C_AUTOIMPLEMENTATTRIBUTE_TARGET_NAMESPACE};

/// <summary>
/// Marks an interface with all members for automatic implementation
/// </summary>
{GENERATED_CLASS_SHARED_ATTRIBUTES}
[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
internal sealed class {C_AUTO_IMPLEMENT_INTERFACE_ATTRIBUTE_CLASSNAME} : Attribute
{@"{
}"}
";

    internal static readonly string AUTO_IMPLEMENT_ON_MEMBERS_ATTRIBUTE_SOURCE =
    @$"{GENERATED_FILE_SHARED_HEADER}
namespace {C_AUTOIMPLEMENTATTRIBUTE_TARGET_NAMESPACE};

/// <summary>
/// Marks a member of an interface for automatic implementation
/// </summary>
{GENERATED_CLASS_SHARED_ATTRIBUTES}
[AttributeUsage(AttributeTargets.Property /*| AttributeTargets.Method | AttributeTargets.Event*/, AllowMultiple = false, Inherited = false)]
internal sealed class {C_AUTO_IMPLEMENT_ON_MEMBERS_ATTRIBUTE_CLASSNAME} : Attribute
{@"{
    /// <summary>
    /// Determines if the member should be automatically implemented or not
    /// </summary>
    public bool Implement { get; set; } = true;
}"}";


    internal static readonly string IMPLEMENT_AS_REQUIRED_ATTRIBUTE_SOURCE =
    @$"{GENERATED_FILE_SHARED_HEADER}
/// <summary>
/// Adds the ""required"" keyword to the generated property
/// </summary>
{GENERATED_CLASS_SHARED_ATTRIBUTES}
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
internal sealed class {C_IMPLEMENT_AS_REQUIRED_ATTRIBUTE_CLASSNAME} : Attribute
{@"{
}"}";

    internal static readonly string REQUIRED_DOTNET_6_PATCH_SOURCE =
    $@"{GENERATED_FILE_SHARED_HEADER}
{@"#if !NET7_0_OR_GREATER
namespace System.Runtime.CompilerServices {
    internal class RequiredMemberAttribute : Attribute { }
    internal class CompilerFeatureRequiredAttribute : Attribute
    {
        public CompilerFeatureRequiredAttribute(string name) { }
    }
}

namespace System.Diagnostics.CodeAnalysis {

    [System.AttributeUsage(System.AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    internal sealed class SetsRequiredMembersAttribute : Attribute { }

}
#endif
"}";

}