namespace ContractManager.Shared.Application.Security;

public record class AuthenticatedUser(int Id, string Email, string Name, HashSet<PermissionType> Permissions, HashSet<RoleType> Roles);
