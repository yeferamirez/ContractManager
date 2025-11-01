namespace ContractManager.Shared.Application.Models;
public abstract class BaseFilterModel
{
    public string? OrderBy { get; set; }

    public int Page { get; set; } = 0;

    public int PageSize { get; set; } = 10;
}
