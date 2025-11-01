using ContractManager.Shared.Api.Extensions;
using ContractManager.Shared.Application.Security;
using ContractManager.Shared.Application.Security.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContractManager.Shared.Api.Attributes;

public class AuthorizationRoleAttribute : Attribute, IAuthorizationFilter
{
    private readonly PermissionType[] permissions;

    public AuthorizationRoleAttribute(params PermissionType[] permissions)
    {
        this.permissions = permissions;
    }

    public void OnAuthorization(AuthorizationFilterContext actionContext)
    {
        if (actionContext.HttpContext?.User?.Identity is { IsAuthenticated: true } &&
            !actionContext.HttpContext.User.Identity.GetUserFromClaims().HasPermission(this.permissions))
        {
            actionContext.Result = new ForbidResult();
        }
    }
}
