using ContractManager.Application.Extensions;
using ContractManager.Application.Models;
using ContractManager.Application.Repositories;
using ContractManager.Shared.Application.Errors;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContractManager.Application.UseCases.GetContractById;
public class GetContractByIdQuery : IRequest<Result<GetByIdContractDto>>
{
    public Guid ContractId { get; set; }
}

public class GetContractByIdQueryHandler(
    IContractRepository contractRepository,
    ILogger<GetContractByIdQueryHandler> logger) : IRequestHandler<GetContractByIdQuery, Result<GetByIdContractDto>>
{
    public async Task<Result<GetByIdContractDto>> Handle(GetContractByIdQuery request, CancellationToken cancellationToken)
    {
        var contract = await contractRepository.GetByIdAsync(request.ContractId);

        if (contract == null)
        {
            logger.LogInformation("Contract {ProcedureId} not found", request.ContractId);
            return Result.Fail(new NotFoundError());
        }

        var contractDto = contract.ToGetByIdDto();

        return contractDto;
    }
}
