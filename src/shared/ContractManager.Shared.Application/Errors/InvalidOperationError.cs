using ContractManager.Shared.Application.Exceptions;

namespace ContractManager.Shared.Application.Errors;
public class InvalidOperationError : ContractError
{
    public InvalidOperationError(ContractErrorCodes code) : base("InvalidOperation", code.ToString())
    {
    }
}
