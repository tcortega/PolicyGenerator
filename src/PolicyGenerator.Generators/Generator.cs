using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace PolicyGenerator.Generators;

[Generator]
public sealed partial class PoliciesGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var policyAttribute = Utility.GetTemplate("PolicyAttribute");
		context.RegisterPostInitializationOutput(i => i.AddSource("PolicyAttribute.g.cs", SourceText.From(policyAttribute.Render(), Encoding.UTF8)));

		var policies = context.SyntaxProvider
			.ForAttributeWithMetadataName(
				"PolicyGenerator.PolicyAttribute",
				(_, _) => true,
				TransformPolicy
			)
			.Where(c => c != null);

		var assemblyName = context.CompilationProvider
			.Select((cp, _) => cp.AssemblyName!
				.Replace(".", string.Empty)
				.Replace(" ", string.Empty)
				.Trim()
			);

		var allPoliciesTemplate = Utility.GetTemplate("Policies");
		context.RegisterSourceOutput(
			policies.Collect().Combine(assemblyName),
			(spc, p) => RenderPolicies(spc, p.Left!, p.Right, allPoliciesTemplate)
		);
	}
}
