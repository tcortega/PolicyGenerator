using Microsoft.CodeAnalysis;

namespace PolicyGenerator.Tests.Helpers;

public static class ReferenceAssemblyHelpers
{
	public static IEnumerable<MetadataReference> GetAdditionalReferences(this DriverReferenceAssemblies assemblies)
	{
		if (assemblies is DriverReferenceAssemblies.Normal)
			return [];

		if (assemblies is DriverReferenceAssemblies.Msdi)
		{
			return
			[
				MetadataReference.CreateFromFile("./Microsoft.Extensions.DependencyInjection.dll"),
				MetadataReference.CreateFromFile("./Microsoft.Extensions.DependencyInjection.Abstractions.dll"),
			];
		}

		// to be done with other renderers
		throw new NotImplementedException();
	}
}

public enum DriverReferenceAssemblies
{
	Normal,
	Msdi,
}
