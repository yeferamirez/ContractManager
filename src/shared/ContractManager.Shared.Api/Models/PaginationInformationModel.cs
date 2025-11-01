namespace ContractManager.Shared.Api.Models;

public class PaginationInformationModel
{
    public int Count { get; set; }

    public bool HasNextPage { get; set; }

    public int TotalCount { get; set; }
}
