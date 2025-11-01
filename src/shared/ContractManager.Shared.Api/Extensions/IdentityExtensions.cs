using ContractManager.Shared.Application.Exceptions;
using ContractManager.Shared.Application.Security;
using System.Data;
using System.Security.Claims;
using System.Security.Principal;

namespace ContractManager.Shared.Api.Extensions;

public static class IdentityExtensions
{
    public static AuthenticatedUser? GetUserFromClaims(this IIdentity identity)
    {
        if (identity is not ClaimsIdentity claimsIdentity || identity is not { IsAuthenticated: true })
        {
            return null;
        }

        var id = claimsIdentity?.FindFirst("id")?.Value
                  ?? claimsIdentity?.FindFirst("sub")?.Value
                  ?? throw new ContractException("Invalid token claims. Id not found.");

        var email = claimsIdentity.FindFirst(ClaimTypes.Email)?.Value
                    ?? claimsIdentity.FindFirst("email")?.Value
                    ?? throw new ContractException("Invalid token claims. Email not found.");

        var name = claimsIdentity.FindFirst("name")?.Value
                    ?? claimsIdentity.FindFirst("cognito:username")?.Value
                    ?? "Unknown";

        var permissions = claimsIdentity.FindAll("permissions")
           .Select(c => Enum.TryParse<PermissionType>(c.Value, true, out var p)
               ? (PermissionType?)p
               : null)
           .Where(p => p.HasValue)
           .Select(p => p!.Value)
           .ToHashSet();

        var roles = claimsIdentity.FindAll("role")
             .Select(c => Enum.TryParse<RoleType>(c.Value, true, out var r)
                 ? (RoleType?)r
                 : null)
             .Where(r => r.HasValue)
             .Select(r => r!.Value)
             .ToHashSet();

        _ = int.TryParse(id, out var userId);

        return new AuthenticatedUser(
           userId,
           email,
           name,
           permissions,
           roles);
    }
}
