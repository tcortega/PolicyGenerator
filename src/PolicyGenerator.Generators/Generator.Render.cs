using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Scriban;

namespace PolicyGenerator.Generators;

public sealed partial class PoliciesGenerator
{
	private static void RenderPolicies(SourceProductionContext context, ImmutableArray<PolicyDescriptor> policies,
		string assemblyName, Template template)
	{
		if (policies.Length == 0)
			return;

		var ct = context.CancellationToken;
		ct.ThrowIfCancellationRequested();

		var allClaims = policies.SelectMany(x => x.Claims)
			.Where(x => !string.IsNullOrWhiteSpace(x));

		var source = template.Render(new { Assembly = assemblyName, Policies = policies, AllClaims = allClaims });

		ct.ThrowIfCancellationRequested();
		context.AddSource("Authorization.Policies.g.cs", source);
	}
}
