using PolicyGenerator.Tests.Helpers;

namespace PolicyGenerator.Tests.GeneratorTests;
public class PolicyTests
{
	[Theory]
	[InlineData(DriverReferenceAssemblies.Normal)]
	public async Task ShouldGenerateForValidPolicy(DriverReferenceAssemblies assemblies)
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using PolicyGenerator;

			namespace Dummy;

			[Policy]
			public static class DummyPolicy
			{
				public const string Name = "DummyPolicy";
				public static readonly string[] Claims = ["dummy"];
			}
			""",
			assemblies);

		_ = await Verify(result)
			.UseParameters(string.Join("_", assemblies));
	}
}
