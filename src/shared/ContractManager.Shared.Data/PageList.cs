using Microsoft.EntityFrameworkCore;

namespace ContractManager.Shared.Data;
public class PagedList<T> : List<T>, IPagedList<T>
{
    public PagedList(IQueryable<T> query, int page, int pageSize)
    {
        this.PageIndex = page;
        this.PageSize = pageSize;

        ////counts all rows
        this.TotalCount = query.Count();
        this.TotalPages = this.TotalCount / this.PageSize;

        if (this.TotalCount % this.PageSize > 0)
        {
            this.TotalPages++;
        }

        this.AddRange(query.Skip(page * pageSize).Take(pageSize).ToList());
    }

    public PagedList(IList<T> results, int page, int pageSize, int totalCount)
    {
        this.TotalCount = totalCount;

        var list = results;
        this.AddRange(list);

        this.CalculateValues(page, pageSize);
    }

    public PagedList(IEnumerable<T> results, int page, int pageSize, int totalCount)
    {
        this.TotalCount = totalCount;

        var list = results;
        this.AddRange(list);

        this.CalculateValues(page, pageSize);
    }

    public PagedList()
    {
    }

    public bool HasNextPage
    {
        get
        {
            return this.TotalPages > this.PageIndex + 1;
        }
    }

    public bool HasPreviousPage
    {
        get
        {
            return this.PageIndex > 0;
        }
    }

    public int PageIndex
    {
        get; private set;
    }

    public int PageSize
    {
        get; private set;
    }

    public int TotalCount
    {
        get; private set;
    }

    public int TotalPages
    {
        get; private set;
    }

    public async Task<PagedList<T>> Async(IQueryable<T> query, int page, int pageSize)
    {
        ////counts all rows
        this.TotalCount = await query.CountAsync().ConfigureAwait(false);

        var list = await query.Skip(page * pageSize).Take(pageSize).ToListAsync().ConfigureAwait(false);
        this.AddRange(list);

        this.CalculateValues(page, pageSize);

        return this;
    }

    private void CalculateValues(int page, int pageSize)
    {
        pageSize = pageSize == 0 ? 1 : pageSize;
        this.PageIndex = page;
        this.PageSize = pageSize;
        this.TotalPages = this.TotalCount / this.PageSize;

        if (this.TotalCount % this.PageSize > 0)
        {
            this.TotalPages++;
        }
    }
}
