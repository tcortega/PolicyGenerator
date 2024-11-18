using PolicyGenerator;

namespace Api.Polices;

[Policy]
public static class RolesPolicy
{
	public const string Name = "Roles";
	public static readonly string[] Roles = ["Admin", "Moderator"];
}
