using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContractManager.Shared.Api.Attributes;
public class NonEmptyModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ActionArguments.Count == 0 || context.ActionArguments.ContainsKey("model") && context.ActionArguments["model"] == null)
        {
            context.Result = new BadRequestObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Model cannot be null",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            });
        }
        else
        {
            base.OnActionExecuting(context);
        }
    }
}
