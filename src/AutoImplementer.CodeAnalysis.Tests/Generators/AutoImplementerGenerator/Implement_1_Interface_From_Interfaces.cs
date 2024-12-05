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

using Basilisque.AutoImplementer.CodeAnalysis.Generators;
using Microsoft.CodeAnalysis.Testing;

namespace Basilisque.AutoImplementer.CodeAnalysis.Tests.Generators.AutoImplementerGenerator;

[TestClass]
[TestCategory("AutoImplementerGenerator")]
public class Implement_1_Interface_From_Interfaces : BaseAutoImplementerGeneratorTest
{
    protected override void AddSourcesUnderTest(SourceFileList sources)
    {

sources.Add(@"
#nullable enable

namespace AutoImpl.AIG.TestObjects.Implement_1_Interface_From_Interfaces;

/// <summary>
/// A
/// </summary>
public interface IA
{
    /// <summary>
    /// Auto-implemented property
    /// </summary>
    int A { get; set; }
}

/// <summary>
/// B
/// </summary>
public interface IB
{
    /// <summary>
    /// Auto-implemented property
    /// </summary>
    int B { get; set; }
}
");

sources.Add(@"
#nullable enable

namespace AutoImpl.AIG.TestObjects.Implement_1_Interface_From_Interfaces;

/// <summary>
/// A and B
/// </summary>
[Basilisque.AutoImplementer.Annotations.AutoImplementable()]
public interface IAB : IA, IB
{ }
");

sources.Add(@"
#nullable enable

namespace AutoImpl.AIG.TestObjects.Implement_1_Interface_From_Interfaces;

/// <summary>
/// Implements A and B
/// </summary>
public class IABImpl
{ }
");
    }

    protected override IEnumerable<(string Name, string SourceText)> GetExpectedInterfaceImplementations()
    {
        yield return (
            Name: "AutoImpl.AIG.TestObjects.Implement_1_Interface_From_Interfaces.IABImpl.auto_impl.g.cs",
            SourceText: @$"{CommonGeneratorData.GeneratedFileSharedHeaderWithNullable}
namespace AutoImpl.AIG.TestObjects.Implement_1_Interface_From_Interfaces;

{CommonGeneratorData.GeneratedClassSharedAttributes}
public partial class IABImpl
{{
    /// <inheritdoc />
    public required int A {{ get; set; }}
    /// <inheritdoc />
    public required int B {{ get; set; }}
}}

#nullable restore");

    }
}


