using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Scriban;

namespace PolicyGenerator.Generators;

public sealed partial class Generator
{
	private static void RenderPolicy(SourceProductionContext context, PolicyDescriptor policy, string assemblyName,
		Template template)
	{
		var ct = context.CancellationToken;
		ct.ThrowIfCancellationRequested();

		var source = template.Render(new { Assembly = assemblyName, Policy = policy });

		ct.ThrowIfCancellationRequested();
		context.AddSource($"Authorization.Policies.{policy.Name}Policy.g.cs", source);
	}

	private static void RenderPolicies(SourceProductionContext context, ImmutableArray<PolicyDescriptor> policies,
		string assemblyName, Template template)
	{
		if (policies.Length == 0)
			return;

		var ct = context.CancellationToken;
		ct.ThrowIfCancellationRequested();

		var source = template.Render(new { Assembly = assemblyName, Policies = policies });

		ct.ThrowIfCancellationRequested();
		context.AddSource("Authorization.Policies.g.cs", source);
	}
}
