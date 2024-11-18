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

		var mergedClaims = policies.Where(x => x.Claims != null)
			.SelectMany(x => x.Claims);
		var allClaims = string.Join(", ", mergedClaims);

		ct.ThrowIfCancellationRequested();

		var mergedRoles = policies.Where(x => x.Roles != null)
			.SelectMany(x => x.Roles);
		var allRoles = string.Join(", ", mergedRoles);

		ct.ThrowIfCancellationRequested();

		var source = template.Render(new { Assembly = assemblyName, Policies = policies, AllClaims = allClaims, AllRoles = allRoles });

		ct.ThrowIfCancellationRequested();
		context.AddSource("Authorization.Policies.g.cs", source);
	}
}
