using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using ContractManager.Application.Storage;

namespace ContractManager.Shared.Infrastructure.Storage;
public class DynamoRepository<T> : IDynamoRepository<T>
{
    private readonly IDynamoDBContext context;

    public DynamoRepository(IAmazonDynamoDB client)
    {
        context = new DynamoDBContext(client);
    }

    public async Task<T?> GetByIdAsync(string id)
        => await context.LoadAsync<T>(id);

    public async Task SaveAsync(T entity)
        => await context.SaveAsync(entity);

    public async Task DeleteAsync(string id)
        => await context.DeleteAsync<T>(id);

    public async Task<IEnumerable<T>> ScanAsync()
    {
        var conditions = new List<ScanCondition>();
        return await context.ScanAsync<T>(conditions).GetRemainingAsync();
    }
}
