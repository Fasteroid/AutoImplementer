/*
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

using static Basilisque.AutoImplementer.CodeAnalysis.Generators.CommonGeneratorData;

namespace Basilisque.AutoImplementer.CodeAnalysis.Generators.StaticAttributesGenerator;

/// <summary>
/// Provides data necessary for generating auto implementations
/// </summary>
public static class StaticAttributesGeneratorData
{
    internal const string AutoImplementInterfaceAttributeClassName = "AutoImplementInterfaceAttribute";
        internal const string AutoImplementInterfaceAttribute_Strict = "Strict";

    internal const string ExemptionAttributeClassName = "AutoImplementExemptAttribute";

    internal const string AutoImplementInterfaceAttributeFullName = $"{AutoImplementedAttributesTargetNamespace}.{AutoImplementInterfaceAttributeClassName}";
    internal const string ExemptionAttributeFullName              = $"{AutoImplementedAttributesTargetNamespace}.{ExemptionAttributeClassName}";

    private const string AutoImplementInterfaceAttributeCompilationName = $"{AutoImplementInterfaceAttributeFullName}.g";
    private const string ExemptionAttributeCompilationName              = $"{ExemptionAttributeFullName}.g";

    private static readonly string _autoImplementInterfaceAttributeSource =
    @$"{GeneratedFileSharedHeaderWithUsings}
namespace {AutoImplementedAttributesTargetNamespace}
{{
    /// <summary>
    /// Marks an interface with all members for automatic implementation
    /// </summary>
    {GeneratedClassSharedAttributes}
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    internal sealed class {AutoImplementInterfaceAttributeClassName} : Attribute
    {{
        /// <summary>
        /// Should non-nullable properties be marked required?
        /// </summary>
        public bool {AutoImplementInterfaceAttribute_Strict} {{ get; set; }} = true;
    }}
}}";

    private static readonly string _exemptionAttributeSource =
    @$"{GeneratedFileSharedHeaderWithUsings}
namespace {AutoImplementedAttributesTargetNamespace}
{{
    /// <summary>
    /// Exempts a property from being automatically implemented
    /// </summary>
    {GeneratedClassSharedAttributes}
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class {ExemptionAttributeClassName} : Attribute
    {{
    }}
}}";

    /// <summary>
    /// The list of currently supported attributes
    /// </summary>
    public static readonly IReadOnlyDictionary<string, (string CompilationName, string Source)> SupportedAttributes = new Dictionary<string, (string CompilationName, string Source)>()
    {
        { AutoImplementInterfaceAttributeFullName, (AutoImplementInterfaceAttributeCompilationName, _autoImplementInterfaceAttributeSource) },
        { ExemptionAttributeFullName, (ExemptionAttributeCompilationName, _exemptionAttributeSource) },
    };
}
