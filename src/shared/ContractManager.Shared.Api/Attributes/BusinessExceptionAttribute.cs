using ContractManager.Shared.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Concurrent;

namespace ContractManager.Shared.Api.Attributes;
public class BusinessExceptionAttribute : ExceptionFilterAttribute
{
    private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

    // list of error codes that should be mapped as Internal Server Error
    private readonly HashSet<ExceptionCodes> _internalServerErrorCodes = new HashSet<ExceptionCodes>()
    {
        ExceptionCodes.DatabaseError
    };

    public BusinessExceptionAttribute()
    {
        _exceptionHandlers = new ConcurrentDictionary<Type, Action<ExceptionContext>>()
        {
            [typeof(ActionValidationException)] = HandleValidationException,
        };
    }

    public override void OnException(ExceptionContext context)
    {
        HandleException(context);

        base.OnException(context);
    }

    private void HandleException(ExceptionContext context)
    {
        Type type = context.Exception.GetType();

        if (_exceptionHandlers.TryGetValue(type, out var value))
        {
            value.Invoke(context);
        }
    }

    private static void HandleValidationException(ExceptionContext context)
    {
        var exception = (ActionValidationException)context.Exception;

        var details = new ValidationProblemDetails(exception.Errors)
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        };

        context.Result = new BadRequestObjectResult(details);

        context.ExceptionHandled = true;
    }
}
