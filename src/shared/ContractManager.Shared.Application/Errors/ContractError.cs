using FluentResults;

namespace ContractManager.Shared.Application.Errors;
public class ContractError : Error
{
    public ContractError(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }

    public ContractError(string errorCode) : base(errorCode)
    {
        ErrorCode = errorCode;
    }

    public string ErrorCode { get; }
}
