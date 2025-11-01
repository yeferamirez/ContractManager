using ContractManager.Application.Repositories;
using ContractManager.Data.Entities.Enums;
using ContractManager.Messaging;
using MassTransit;

namespace ContractManager.Api.BackgroudService;

public class ContractExpirationCheckerService : BackgroundService
{
    private readonly ILogger<ContractExpirationCheckerService> logger;
    private readonly IServiceProvider serviceProvider;
    private readonly IHostApplicationLifetime applicationLifetime;

    private static int delay = 5 * 60 * 1000; // 5 minutes

    public ContractExpirationCheckerService(
        ILogger<ContractExpirationCheckerService> logger,
        IServiceProvider serviceProvider,
        IHostApplicationLifetime applicationLifetime)
    {
        this.logger = logger;
        this.serviceProvider = serviceProvider;
        this.applicationLifetime = applicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await WaitForApplicationStartAsync(stoppingToken);

        logger.LogInformation("Application started and ready to find expired contracts");

        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckExpiredContractsAsync(stoppingToken);

            await Task.Delay(delay, stoppingToken);
        }
    }

    private async Task WaitForApplicationStartAsync(CancellationToken stoppingToken)
    {
        while (!applicationLifetime.ApplicationStarted.IsCancellationRequested)
        {
            if (stoppingToken.IsCancellationRequested)
                return;

            logger.LogInformation("Waiting for application start");
            await Task.Delay(500, stoppingToken);
        }
    }

    private async Task CheckExpiredContractsAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var contractRepository = scope.ServiceProvider.GetRequiredService<IContractRepository>();
            var publisher = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
            var timeProvider = scope.ServiceProvider.GetRequiredService<TimeProvider>();

            var expiredContracts = await contractRepository.GetContractExpiredAsync(timeProvider.GetUtcNow().DateTime);

            foreach (var contract in expiredContracts!)
            {
                contract.Status = ContractStatus.Expired;
                contract.UpdatedAt = timeProvider.GetUtcNow().DateTime;
                await contractRepository.UpdateAsync(contract);

                await publisher.Publish(new ContractExpired(contract.Id));

                logger.LogInformation("published expired contract: {ContractId}", contract.Id);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while rewriting");
            throw;
        }
    }
}
