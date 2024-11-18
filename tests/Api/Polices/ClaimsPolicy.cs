using PolicyGenerator;

namespace Api.Polices;

[Policy]
internal static class ClaimsPolicy
{
	public const string Name = "ClaimsPolicy";
	public static readonly string[] Claims = ["claims:read"];
}
