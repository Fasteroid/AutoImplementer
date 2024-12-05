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
public class Implement_1_Interface_With_Exempt_Property : BaseAutoImplementerGeneratorTest
{
    protected override void AddSourcesUnderTest(SourceFileList sources)
    {
        // interface to auto implement
        sources.Add(@"
#nullable enable

namespace AutoImpl.AIG.TestObjects.Implement_1_Interface_With_Exempt_Property;

/// <summary>
/// A class used as parameter type
/// </summary>
public class MyTestClass
{ }

/// <summary>
/// The interface to be implemented
/// </summary>
[Basilisque.AutoImplementer.Annotations.AutoImplementable()]
public interface IMyInterface
{
    /// <summary>
    /// Auto-implemented property
    /// </summary>
    int AutoImplementedInt { get; set; }

    /// <summary>
    /// Manually-implemented property
    /// </summary>
    [Basilisque.AutoImplementer.Annotations.AutoImplementExempt()]
    int ManuallyImplementedInt { get; set; }
}
");

        // class that implements the interface
        sources.Add(@"
namespace AutoImpl.AIG.TestObjects.Implement_1_Interface_With_Exempt_Property;

/// <summary>
/// The class implementing the interface
/// </summary>
public partial class MyImplementation : IMyInterface
{
    /// <inheritdoc />
    public int ManuallyImplementedInt { get; set; }
}
");
    }

    protected override IEnumerable<(string Name, string SourceText)> GetExpectedInterfaceImplementations()
    {
        yield return (
            Name: "AutoImpl.AIG.TestObjects.Implement_1_Interface_With_Exempt_Property.MyImplementation.auto_impl.g.cs",
            SourceText: @$"{CommonGeneratorData.GeneratedFileSharedHeaderWithNullable}
namespace AutoImpl.AIG.TestObjects.Implement_1_Interface_With_Exempt_Property;

{CommonGeneratorData.GeneratedClassSharedAttributes}
public partial class MyImplementation
{{
    /// <inheritdoc />
    public required int AutoImplementedInt {{ get; set; }}
}}

#nullable restore");
    }
}

