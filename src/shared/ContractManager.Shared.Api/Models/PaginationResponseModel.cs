namespace ContractManager.Shared.Api.Models;
public class PaginationResponseModel<T> where T : class
{
    public PaginationInformationModel Meta { get; set; } = new PaginationInformationModel();

    public T[] Results { get; set; } = [];
}
