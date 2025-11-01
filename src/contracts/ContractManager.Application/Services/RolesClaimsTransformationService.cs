using ContractManager.Data.Entities;
using ContractManager.Shared.Application.Security.Configuration;
using ContractManager.Shared.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ContractManager.Application.Services;
public class RolesClaimsTransformationService : IClaimsTransformation
{
    private readonly IRepository<UserRole> repository;
    private readonly JwtSettings jwtSettings;

    public RolesClaimsTransformationService(
        IRepository<UserRole> repository,
        JwtSettings jwtSettings)
    {
        this.repository = repository;
        this.jwtSettings = jwtSettings;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (!jwtSettings.UseCognito)
            return principal;

        if (principal.Identities.Any(i => i.AuthenticationType == "ClaimsTransformation"))
            return principal;

        var userIdClaim = principal.FindFirst("id")?.Value
                       ?? principal.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            return principal;

        var userRoles = await repository.TableNoTracking
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .ToListAsync();

        if (!userRoles.Any())
            return principal;

        var extra = new ClaimsIdentity(authenticationType: "ClaimsTransformation");

        foreach (var role in userRoles.Select(ur => ur.Role.Name).Distinct())
            extra.AddClaim(new Claim("role", role));

        var permissions = userRoles
            .SelectMany(ur => ur.Role.RolePermissions.Select(rp => rp.Permission.Name))
            .Distinct();

        foreach (var permission in permissions)
            extra.AddClaim(new Claim("permissions", permission));

        principal.AddIdentity(extra);
        return principal;
    }
}
