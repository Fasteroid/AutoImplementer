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

using Basilisque.AutoImplementer.CodeAnalysis.Generators;
using Microsoft.CodeAnalysis.Testing;

namespace Basilisque.AutoImplementer.CodeAnalysis.Tests.Generators.AutoImplementerGenerator;

[TestClass]
[TestCategory(AutoImplementerGeneratorCategory)]
public class Implement_1_Interface_With_Required_Properties : BaseAutoImplementerGeneratorTest
{
    protected override void AddSourcesUnderTest(SourceFileList sources)
    {
        // interface to auto implement
        sources.Add(@"
#nullable enable

using Basilisque.AutoImplementer.Annotations;

namespace AutoImpl.AIG.TestObjects.Implement_1_Interface_With_Required_Properties;

/// <summary>
/// The interface to be implemented
/// </summary>
[Basilisque.AutoImplementer.Annotations.AutoImplementInterface()]
public interface IMyInterface
{
    /// <summary>
    /// int not required
    /// </summary>
    int IntNotRequired { get; set; }

    /// <summary>
    /// int required
    /// </summary>
    [Required] int IntRequired { get; set; }

    /// <summary>
    /// string not required
    /// </summary>
    string StringNotRequired { get; set; }

    /// <summary>
    /// string required
    /// </summary>
    [Required] string StringRequired { get; set; }

    /// <summary>
    /// string required by AutoImplementAttribute
    /// </summary>
    [Basilisque.AutoImplementer.Annotations.AutoImplement(AsRequired = true)]
    int IntRequiredByAutoImplementAttribute { get; set; }

    /// <summary>
    /// string not required by AutoImplementAttribute
    /// </summary>
    [Basilisque.AutoImplementer.Annotations.AutoImplement(AsRequired = false)]
    int IntNotRequiredByAutoImplementAttribute { get; set; }
}
");

        // class that implements the interface
        sources.Add(@"
namespace AutoImpl.AIG.TestObjects.Implement_1_Interface_With_Required_Properties;

/// <summary>
/// The class implementing the interface
/// </summary>
[Basilisque.AutoImplementer.Annotations.AutoImplementInterfaces()]
public partial class MyImplementation : IMyInterface
{ }
");
    }

    protected override IEnumerable<(string Name, string SourceText)> GetExpectedInterfaceImplementations()
    {
        yield return (
            Name: "AutoImpl.AIG.TestObjects.Implement_1_Interface_With_Required_Properties.MyImplementation.auto_impl.g.cs",
            SourceText: @$"{CommonGeneratorData.GeneratedFileSharedHeaderWithNullable}
namespace AutoImpl.AIG.TestObjects.Implement_1_Interface_With_Required_Properties;

{CommonGeneratorData.GeneratedClassSharedAttributesNotIndented}
public partial class MyImplementation
{{
    /// <inheritdoc />
    public int IntNotRequired {{ get; set; }}
    
    /// <inheritdoc />
    public required int IntRequired {{ get; set; }}
    
    /// <inheritdoc />
    public string StringNotRequired {{ get; set; }}
    
    /// <inheritdoc />
    public required string StringRequired {{ get; set; }}
    
    /// <inheritdoc />
    public required int IntRequiredByAutoImplementAttribute {{ get; set; }}
    
    /// <inheritdoc />
    public int IntNotRequiredByAutoImplementAttribute {{ get; set; }}
}}

#nullable restore");
    }

    protected override IEnumerable<DiagnosticResult> GetExpectedDiagnostics()
    {
        // There should be exactly one warning CS8618 stating that 'StringNotRequired' must contain a non-null value when exiting constructor.
        // The 'IntNotRequired' property doesn't raise this warning and the other two properties are marked as required.
        yield return DiagnosticResult.CompilerWarning("CS8618")
            .WithSpan(System.IO.Path.Combine("Basilisque.AutoImplementer.CodeAnalysis", "Basilisque.AutoImplementer.CodeAnalysis.AutoImplementerGenerator", "AutoImpl.AIG.TestObjects.Implement_1_Interface_With_Required_Properties.MyImplementation.auto_impl.g.cs"), 27, 19, 27, 36)
            .WithSpan(System.IO.Path.Combine("Basilisque.AutoImplementer.CodeAnalysis", "Basilisque.AutoImplementer.CodeAnalysis.AutoImplementerGenerator", "AutoImpl.AIG.TestObjects.Implement_1_Interface_With_Required_Properties.MyImplementation.auto_impl.g.cs"), 27, 19, 27, 36);
    }
}

