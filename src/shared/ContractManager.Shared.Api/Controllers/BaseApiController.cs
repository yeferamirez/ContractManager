using ContractManager.Shared.Api.Extensions;
using ContractManager.Shared.Api.Models;
using ContractManager.Shared.Application.Security;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ContractManager.Shared.Api.Controllers;
public class BaseApiController : ControllerBase
{
    private AuthenticatedUser? currentUser = null;

    protected int CurrentUserId
    {
        get
        {
            if (User?.Identity is not { IsAuthenticated: true })
                return 0;

            var claimsIdentity = User.Identity as ClaimsIdentity;
            var idValue = claimsIdentity?.FindFirst("id")?.Value
                          ?? claimsIdentity?.FindFirst("sub")?.Value;

            return int.TryParse(idValue, out var result) ? result : 0;
        }
    }

    protected AuthenticatedUser? CurrentUser
    {
        get
        {
            if (currentUser != null)
                return currentUser;

            currentUser = User.Identity?.GetUserFromClaims();
            return currentUser;
        }
    }

    [NonAction]
    protected bool IsAuthenticated()
    {
        return this.User.Identity?.IsAuthenticated ?? false;
    }

    protected IActionResult BadRequest(List<IError> error)
    {
        var errorResponse = new ProblemDetails()
        {
            Status = 400,
            Title = error.First().Message,
            Detail = error.First().Message,
        };

        return this.BadRequest(errorResponse);
    }

    protected IActionResult NotFound(List<IError> error)
    {
        var details = new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Detail = error.First().Message,
            Title = error.First().Message,
            Instance = HttpContext?.Request?.Path
        };

        return this.NotFound(details);
    }

    protected IActionResult Conflict(List<IError> error)
    {
        var details = new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Detail = error.First().Message,
            Title = error.First().Message,
            Instance = HttpContext?.Request?.Path
        };

        return this.Conflict(details);
    }

    protected IActionResult Ok<T>(T[] list, bool hasNextPage, int totalCount)
        where T : class
    {
        var model = new PaginationResponseModel<T>()
        {
            Meta = new PaginationInformationModel
            {
                Count = list.Length,
                HasNextPage = hasNextPage,
                TotalCount = totalCount
            },
            Results = list
        };

        return this.Ok(model);
    }
}
