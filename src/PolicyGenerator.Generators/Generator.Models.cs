namespace PolicyGenerator.Generators;

public sealed partial class Generator
{
	private sealed record PolicyDescriptor
	{
		public required string Name { get; init; }
		public required string Claims { get; init; }
		public required string ClassPath { get; init; }
	}
}
