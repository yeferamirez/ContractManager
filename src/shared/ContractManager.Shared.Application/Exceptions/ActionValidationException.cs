using FluentValidation.Results;

namespace ContractManager.Shared.Application.Exceptions;
public class ActionValidationException : Exception
{
    public ActionValidationException()
        : base("One or more validation errors.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ActionValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    public IDictionary<string, string[]> Errors { get; }
}
