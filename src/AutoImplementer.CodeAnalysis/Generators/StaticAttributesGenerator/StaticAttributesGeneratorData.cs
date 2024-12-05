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
    internal const string AutoImplementableAttribute = "AutoImplementableAttribute";
        internal const string AutoImplementable_Strict   = "Strict";

    internal const string ExemptionAttribute = "AutoImplementExemptAttribute";

    internal const string AutoImplementableFull  = $"{AnnotationsNamespace}.{AutoImplementableAttribute}";
    internal const string ExemptionAttributeFull = $"{AnnotationsNamespace}.{ExemptionAttribute}";

    private const string AutoImplementableSourceFile  = $"{AutoImplementableFull}.g";
    private const string ExemptionAttributeSourceFile = $"{ExemptionAttributeFull}.g";

    private static readonly string _autoImplementableAttributeSourceCode =
    @$"{GeneratedFileSharedHeaderWithUsings}
namespace {AnnotationsNamespace}
{{
    /// <summary>
    /// Marks an interface with all members for automatic implementation
    /// </summary>
    {GeneratedClassSharedAttributes}
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    internal sealed class {AutoImplementableAttribute} : Attribute
    {{
        /// <summary>
        /// Should non-nullable properties be marked required?
        /// </summary>
        public bool {AutoImplementable_Strict} {{ get; set; }} = true;
    }}
}}";

    private static readonly string _exemptionAttributeSourceCode =
    @$"{GeneratedFileSharedHeaderWithUsings}
namespace {AnnotationsNamespace}
{{
    /// <summary>
    /// Exempts a property from being automatically implemented
    /// </summary>
    {GeneratedClassSharedAttributes}
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class {ExemptionAttribute} : Attribute
    {{
    }}
}}";

    /// <summary>
    /// The list of currently supported attributes
    /// </summary>
    public static readonly IReadOnlyDictionary<string, (string CompilationName, string Source)> SupportedAttributes = new Dictionary<string, (string CompilationName, string Source)>()
    {
        { AutoImplementableFull, (AutoImplementableSourceFile, _autoImplementableAttributeSourceCode) },
        { ExemptionAttributeFull, (ExemptionAttributeSourceFile, _exemptionAttributeSourceCode) },
    };
}
