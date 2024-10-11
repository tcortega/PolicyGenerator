using System.Runtime.CompilerServices;

namespace PolicyGenerator.Tests;

public static class ModuleInitializer
{
	[ModuleInitializer]
	public static void Init()
	{
		VerifierSettings.AutoVerify(includeBuildServer: false);
		VerifySourceGenerators.Initialize();
	}
}
