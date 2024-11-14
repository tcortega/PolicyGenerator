namespace PolicyGenerator.Generators;

public sealed partial class PoliciesGenerator
{
	private sealed record PolicyDescriptor
	{
		public required string Name { get; init; }
		public string? Claims { get; init; }
		public string[]? Roles { get; init; }
		public string[]? AuthenticationSchemes { get; init; }
		public required string ClassPath { get; init; }
	}
}
