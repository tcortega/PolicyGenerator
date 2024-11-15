using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using PolicyGenerator.Generators;
using PolicyGenerator.Tests.Helpers;

namespace PolicyGenerator.Tests.GeneratorTests;

public static class GeneratorTestHelper
{
	public static GeneratorDriverRunResult RunGenerator(
		[StringSyntax("c#-test")] string source,
		DriverReferenceAssemblies assemblies
	)
	{
		var syntaxTree = CSharpSyntaxTree.ParseText(source);

		var compilation = CSharpCompilation.Create(
			assemblyName: "Tests",
			syntaxTrees: [syntaxTree],
			references:
			[
				.. Basic.Reference.Assemblies.Net80.References.All,
				.. assemblies.GetAdditionalReferences(),
			],
			options: new(
				outputKind: OutputKind.DynamicallyLinkedLibrary
			)
		);

		var generator = new PoliciesGenerator();

		var driver = CSharpGeneratorDriver
			.Create(generator)
			.RunGeneratorsAndUpdateCompilation(
				compilation,
				out var outputCompilation,
				out var diagnostics
			);

		Assert.Empty(
			outputCompilation
				.GetDiagnostics()
				.Where(d => d.Severity is DiagnosticSeverity.Error or DiagnosticSeverity.Warning)
		);

		Assert.Empty(diagnostics);
		return driver.GetRunResult();
	}
}
