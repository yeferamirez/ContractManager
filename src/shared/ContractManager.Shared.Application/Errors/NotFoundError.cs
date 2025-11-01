namespace ContractManager.Shared.Application.Errors;
public class NotFoundError : ContractError
{


    public NotFoundError(string message = "Not found") : base("NotFound", message)
    {
    }
}
