using PolicyGenerator;

namespace Api.Polices;

[Policy]
public static class AuthenticationSchemesPolicy
{
	public const string Name = "AuthenticationSchemes";
	public static readonly string[] AuthenticationSchemes = [Constants.IdentityAuthScheme, Constants.ApiKeyAuthScheme];
}
