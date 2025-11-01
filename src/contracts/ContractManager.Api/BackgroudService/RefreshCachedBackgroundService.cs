using ContractManager.Application.CachedServices;

namespace ContractManager.Api.BackgroudService;

public class RefreshCachedBackgroundService : BackgroundService
{
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<RefreshCachedBackgroundService> logger;
    public RefreshCachedBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<RefreshCachedBackgroundService> logger)
    {
        this.serviceProvider = serviceProvider;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting background service for refreshing cache");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var cachedService = scope.ServiceProvider.GetRequiredService<ICacheRefresherService>();

                await cachedService.RefreshCacheAsync(stoppingToken);
                logger.LogInformation("Cache successfully refreshed at: {time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error refreshing cache");
            }

            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
        }
    }
}
