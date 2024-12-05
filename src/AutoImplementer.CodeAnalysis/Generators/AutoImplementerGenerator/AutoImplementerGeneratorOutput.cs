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

using Basilisque.AutoImplementer.CodeAnalysis.Generators.StaticAttributesGenerator;
using Basilisque.CodeAnalysis.Syntax;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using MemberTypes = System.Reflection.MemberTypes;

namespace Basilisque.AutoImplementer.CodeAnalysis.Generators.AutoImplementerGenerator;

internal static class AutoImplementerGeneratorOutput
{
    internal static void OutputImplementations(SourceProductionContext context, AutoImplementerGeneratorInfo generationInfo, RegistrationOptions registrationOptions)
    {
        if (!checkPreconditions(registrationOptions))
            return;

        if (generationInfo.HasDiagnostics)
        {
            foreach (var diagnostic in generationInfo.Diagnostics)
                context.ReportDiagnostic(diagnostic);
        }

        if (!generationInfo.HasInterfaces)
            return;

        var syntaxNodesToImplement = getSyntaxNodesToImplement(context, generationInfo.Interfaces);

        // skip if nothing to implement
        if (!syntaxNodesToImplement.Any() && !generationInfo.Interfaces.Any(inf => !inf.Value.IsInBaseList))
            return;

        var className = generationInfo.ClassName;
        var namespaceName = generationInfo.ContainingNamespace;

        var compilationName = namespaceName is null ? $"{className}.auto_impl" : $"{namespaceName}.{className}.auto_impl";

        var ci = registrationOptions.CreateCompilationInfo(compilationName, namespaceName);
        ci.EnableNullableContext = true;

        ci.AddNewClassInfo(className, generationInfo.AccessModifier, cl =>
        {
            cl.IsPartial = true;

            cl.BaseClass = getBaseInterfaces(generationInfo.Interfaces);

            foreach (var node in syntaxNodesToImplement)
            {
                switch (node)
                {
                    case Basilisque.CodeAnalysis.Syntax.PropertyInfo pi:
                        cl.Properties.Add(pi);
                        break;
                    case Basilisque.CodeAnalysis.Syntax.MethodInfo mi:
                        cl.Methods.Add(mi);
                        break;
                }
            }
        }).AddToSourceProductionContext();
    }

    private static bool checkPreconditions(RegistrationOptions registrationOptions)
    {
        if (registrationOptions.Language != Language.CSharp)
            throw new System.NotSupportedException($"The language '{registrationOptions.Language}' is currently not supported by this generator.");

        return true;
    }

    private static IEnumerable<Basilisque.CodeAnalysis.Syntax.SyntaxNode> getSyntaxNodesToImplement(SourceProductionContext context, Dictionary<INamedTypeSymbol, AutoImplementerGeneratorInterfaceInfo> interfaces)
    {
        foreach (var i in interfaces)
        {
            var members = i.Key.GetMembers(); // TODO: inheritance?

            foreach (var member in members)
            {
                if( getAutoImplementExemptAttribute(member) != null)
                    continue;

                Basilisque.CodeAnalysis.Syntax.SyntaxNode? node;

                switch (member)
                {
                    case IPropertySymbol propertySymbol:
                        node = implementProperty(context, propertySymbol, i.Value);
                        break;
                    //case IMethodSymbol methodSymbol:
                    //    yield return implementMethod(methodSymbol);
                    //    break;
                    default:
                        node = null;
                        break;
                }

                if (node is null)
                    continue;

                yield return node;
            }
        }
    }

    private static string? getBaseInterfaces(Dictionary<INamedTypeSymbol, AutoImplementerGeneratorInterfaceInfo> interfaces)
    {
        var baseInterfaces = interfaces.Where(kvp => !kvp.Value.IsInBaseList).Select(kvp => kvp.Key.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)).ToList();

        if (!baseInterfaces.Any())
            return null;

        return string.Join(", ", baseInterfaces);
    }

    private static PropertyInfo? implementProperty(SourceProductionContext context, IPropertySymbol propertySymbol, AutoImplementerGeneratorInterfaceInfo info)
    {
        // get the full qualified type name of the property
        var fqtn = propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        bool nullable = false;

        if (string.IsNullOrWhiteSpace(fqtn))
            return null;

        // check if the property is nullable
        if (propertySymbol.NullableAnnotation == NullableAnnotation.Annotated)
        {
            nullable = true;
            // check if the type is a value type and not already a nullable type
            if (!fqtn.EndsWith("?") && !fqtn.StartsWith("global::System.Nullable<"))
                fqtn += "?";
        }

        var pi = new PropertyInfo(fqtn, propertySymbol.Name);

        copyAttributes(propertySymbol, pi);

        if (info.Strict && !nullable)
            pi.IsRequired = true;

        pi.InheritXmlDoc = true;
        pi.AccessModifier = propertySymbol.DeclaredAccessibility.ToAccessModifier();

        return pi;
    }

    private static AttributeData? getAutoImplementableAttribute(ISymbol memberSymbol)
    {
        return memberSymbol.GetAttributes().SingleOrDefault(a =>
                    a.AttributeClass?.Name == StaticAttributesGeneratorData.AutoImplementableAttribute
                    && a.AttributeClass.ContainingNamespace.ToDisplayString() == CommonGeneratorData.AnnotationsNamespace);
    }

    private static AttributeData? getAutoImplementExemptAttribute(ISymbol propertySymbol)
    {
        return propertySymbol.GetAttributes().SingleOrDefault(a =>
                    a.AttributeClass?.Name == StaticAttributesGeneratorData.ExemptionAttribute
                    && a.AttributeClass.ContainingNamespace.ToDisplayString() == CommonGeneratorData.AnnotationsNamespace);
    }

    private static void copyAttributes(IPropertySymbol propertySymbol, PropertyInfo pi)
    {

        var attributes = propertySymbol.GetAttributes();

        foreach (var attribute in attributes)
        {
            if (attribute.AttributeClass?.ContainingNamespace.ToDisplayString() == CommonGeneratorData.AnnotationsNamespace)
            {
                if (attribute.AttributeClass.Name == StaticAttributesGeneratorData.ExemptionAttribute)
                {
                    continue; // don't copy internal attributes
                }

            }

            var att = attribute.ToString();
            pi.Attributes.Add(new AttributeInfo(att));
        }
    }
}
