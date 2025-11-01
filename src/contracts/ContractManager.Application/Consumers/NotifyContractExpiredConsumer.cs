using ContractManager.Application.UseCases.NotifyContractExpired;
using ContractManager.Messaging;
using ContractManager.Shared.Application.Messaging;
using MassTransit;
using MediatR;

namespace ContractManager.Application.Consumers;

public class NotifyContractExpiredConsumer : IConsumer<ContractExpired>
{
    public static string EndpointName => $"{MessagingEndpoints.CONTRACTS_CONTRACT_EXPIRED}-{nameof(NotifyContractExpiredConsumer)}";

    private readonly ISender sender;

    public NotifyContractExpiredConsumer(ISender sender)
    {
        this.sender = sender;
    }

    public async Task Consume(ConsumeContext<ContractExpired> context)
    {
        var notifyCommand = new NotifyContractExpiredCommand
        {
            ContractId = context.Message.ContractId
        };

        await this.sender.Send(notifyCommand);
    }
}
