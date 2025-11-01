using ContractManager.Application.Repositories;
using ContractManager.Infrastructure.Caching;
using Microsoft.Extensions.Logging;

namespace ContractManager.Application.CachedServices;

public class CachedContractsService : ICacheRefresherService
{
    private readonly IContractRepository contractRepository;
    private readonly ICacheService cacheService;
    private readonly ILogger<CachedContractsService> logger;

    public CachedContractsService(
        IContractRepository contractRepository,
        ICacheService cacheService,
        ILogger<CachedContractsService> logger)
    {
        this.contractRepository = contractRepository;
        this.cacheService = cacheService;
        this.logger = logger;
    }

    public async Task RefreshCacheAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting contracts cache refresh...");

        var contracts = await contractRepository.GetAllAsync();

        foreach (var contract in contracts)
        {
            var key = $"contract:{contract.Id}";
            var value = System.Text.Json.JsonSerializer.Serialize(contract);

            await cacheService.Set(key, value, TimeSpan.FromHours(1));
        }

        logger.LogInformation("Contracts cache successfully refreshed with {Count} items.", contracts.Count);
    }
}
