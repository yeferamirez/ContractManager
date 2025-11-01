using ContractManager.Shared.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContractManager.Api.Filters;

public class AuthorizationAttribute : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext actionContext)
    {
        var user = actionContext.HttpContext.User.Identity?.GetUserFromClaims();

        if (user == null)
        {
            actionContext.Result = new UnauthorizedResult();
            return;
        }
    }
}
