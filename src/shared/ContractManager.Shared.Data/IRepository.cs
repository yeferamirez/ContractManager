using ContractManager.Shared.Core;

namespace ContractManager.Shared.Data;

public partial interface IRepository<T> where T : class, IEntity
{
    IQueryable<T> Table { get; }

    IQueryable<T> TableNoTracking { get; }

    Task<int> DeleteAsync(T entity);

    Task InsertAsync(T entity);

    Task InsertAsync(IEnumerable<T> entities);

    Task<int> UpdateAsync(T entity);

    Task UpdateAsync(ICollection<T> entities);

    Task<T> GetByIdAsync(Guid id);

    Task<IReadOnlyList<T>> GetAllAsync();
}
