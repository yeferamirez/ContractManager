namespace ContractManager.Shared.Application.Exceptions;
public class ContractException : Exception
{
    public ContractException(string error) : base(error)
    {
    }

    public ContractException(ContractExceptionCodes code) : base(EnumHelpers.GetDescription(code))
    {
        this.Code = code;
    }

    public ContractException(ContractExceptionCodes code, string error) : base(error)
    {
        this.Code = code;
    }

    public ContractException(string target, ContractExceptionCodes code) : base(EnumHelpers.GetDescription(code))
    {
        this.Target = target;
        this.Code = code;
    }

    public ContractException(ContractExceptionCodes code, string error, string target) : base(error)
    {
        this.Target = target;
        this.Code = code;
    }

    public ContractExceptionCodes Code { get; set; }

    public string? Target { get; set; }
}
