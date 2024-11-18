using Microsoft.CodeAnalysis;

namespace PolicyGenerator.Tests;

public static class Utility
{
	public static MetadataReference[] GetMetadataReferences() =>
	[
		MetadataReference.CreateFromFile("./Microsoft.Extensions.DependencyInjection.dll"),
		MetadataReference.CreateFromFile("./Microsoft.Extensions.DependencyInjection.Abstractions.dll"),
		MetadataReference.CreateFromFile("./Microsoft.AspNetCore.Authorization.dll"),
	];
}
