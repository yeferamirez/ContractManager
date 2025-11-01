namespace ContractManager.Shared.Application.Security.Extensions;

public static class UserAuthenticatedExtensions
{
    public static bool HasPermission(this AuthenticatedUser? user, params PermissionType[] permissions)
    {
        return user?.Permissions.Overlaps(permissions) ?? false;
    }
}
