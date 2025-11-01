namespace ContractManager.Application.Storage;
public interface IDynamoRepository<T>
{
    Task<T?> GetByIdAsync(string id);
    Task SaveAsync(T entity);
    Task DeleteAsync(string id);
    Task<IEnumerable<T>> ScanAsync();
}
