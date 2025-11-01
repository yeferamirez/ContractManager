namespace ContractManager.Application.CachedServices;

public interface ICacheRefresherService
{
    Task RefreshCacheAsync(CancellationToken cancellationToken = default);
}
