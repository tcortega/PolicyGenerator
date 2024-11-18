using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using PolicyGenerator.Generators;

namespace PolicyGenerator.Tests.GeneratorTests;
public static class GeneratorTestHelper
{
	public static GeneratorDriverRunResult RunGenerator(string source)
	{
		var syntaxTree = CSharpSyntaxTree.ParseText(source);

		var compilation = CSharpCompilation.Create(
			assemblyName: "Tests",
			syntaxTrees: [syntaxTree],
			references:
			[
				.. Basic.Reference.Assemblies.AspNet80.References.All,
				.. Utility.GetMetadataReferences(),
			],
			options: new(
				outputKind: OutputKind.DynamicallyLinkedLibrary
			)
		);

		var generator = new PoliciesGenerator();

		var driver = CSharpGeneratorDriver
			.Create(new PoliciesGenerator())
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
