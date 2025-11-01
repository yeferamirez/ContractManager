namespace ContractManager.Shared.Data;
public interface IPagedList<T> : IList<T>
{
    bool HasNextPage { get; }

    bool HasPreviousPage { get; }

    int PageIndex { get; }

    int PageSize { get; }

    int TotalCount { get; }

    int TotalPages { get; }
}
