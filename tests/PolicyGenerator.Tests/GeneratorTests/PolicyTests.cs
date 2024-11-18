namespace PolicyGenerator.Tests.GeneratorTests;

public sealed class PolicyTests
{
	[Test]
	public async Task ShouldGenerateForValidPolicies()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using PolicyGenerator;

			namespace Foo;

			[Policy]
			public static class FooPolicy
			{
				public const string Name = "FooPolicy";
				public static readonly string[] Claims = ["Foo"];
				public static readonly string[] Roles = ["admin"];
				public static readonly string[] AuthenticationSchemes = ["hello"];
			}

			[Policy]
			public static class BarPolicy
			{
				public const string Name = "BarPolicy";
				public static readonly string[] Claims = ["Bar"];
			}
			""");

		Assert.Equal(
			[
				@"PolicyGenerator.Generators/PolicyGenerator.Generators.PoliciesGenerator/PolicyAttribute.g.cs",
				@"PolicyGenerator.Generators/PolicyGenerator.Generators.PoliciesGenerator/Authorization.Policies.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result);
	}
}
