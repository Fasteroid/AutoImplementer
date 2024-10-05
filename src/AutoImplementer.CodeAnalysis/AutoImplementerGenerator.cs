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

using Basilisque.CodeAnalysis.Syntax;

namespace Basilisque.AutoImplementer.CodeAnalysis;

/// <summary>
/// A source generator that generates code to automatically implement interfaces
/// </summary>
[Generator]
public class AutoImplementerGenerator : IIncrementalGenerator
{
    ///<inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //get providers
        var classesToGenerateProvider = AutoImplementerGeneratorSelectors.GetClassesToGenerate(context);

        //write output
        context.RegisterPostInitializationOutput(AutoImplementerGeneratorOutput.OutputAttributes);
        context.RegisterCompilationInfoOutput(classesToGenerateProvider, AutoImplementerGeneratorOutput.OutputImplementations);
    }
}