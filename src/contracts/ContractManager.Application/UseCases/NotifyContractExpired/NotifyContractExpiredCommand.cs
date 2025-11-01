using FluentResults;
using MediatR;

namespace ContractManager.Application.UseCases.NotifyContractExpired;
public class NotifyContractExpiredCommand : IRequest<Result<Unit>>
{
    public Guid ContractId { get; set; }
}

public class NotifyScheduleAppointmentCommandHandler : IRequestHandler<NotifyContractExpiredCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(NotifyContractExpiredCommand request, CancellationToken cancellationToken)
    {
        return Unit.Value;
    }
}
