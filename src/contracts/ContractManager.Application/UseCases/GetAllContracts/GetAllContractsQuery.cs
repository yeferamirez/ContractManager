using ContractManager.Application.Models;
using ContractManager.Application.Repositories;
using ContractManager.Data.Entities;
using ContractManager.Data.Entities.Enums;
using ContractManager.Shared.Application.Models;
using ContractManager.Shared.Data;
using FluentResults;
using MediatR;

namespace ContractManager.Application.UseCases.GetAllContracts;
public class GetAllContractsQuery : BaseFilterModel, IRequest<Result<IPagedList<ContractListDto>>>
{
    public DateTime? EndDate { get; set; }
    public ContractStatus? Status { get; set; }
    public ContractTypeEnum? ContractType { get; set; }
    public OrderByProcedure? OrderBy { get; set; } = OrderByProcedure.Date;
    public SortDirection SortDirection { get; set; } = SortDirection.Desc;
}

public class GetAllContractsQueryHandler : IRequestHandler<GetAllContractsQuery, Result<IPagedList<ContractListDto>>>
{
    private readonly IContractRepository contractRepository;

    public GetAllContractsQueryHandler(
        IContractRepository contractRepository)
    {
        this.contractRepository = contractRepository;
    }

    public async Task<Result<IPagedList<ContractListDto>>> Handle(GetAllContractsQuery request, CancellationToken cancellationToken)
    {
        var endDate = request.EndDate.HasValue
          ? request.EndDate.Value.AddHours(24)
          : request.EndDate;

        var pagedContracts = await contractRepository.GetAllContractsAsync(
            request.Page,
            request.PageSize,
            endDate,
            request.Status,
            request.ContractType,
            request.OrderBy,
            request.SortDirection);

        IPagedList<ContractListDto> pagedDtos = new PagedList<ContractListDto>(
            pagedContracts.Select(MapToDto).ToList(),
            pagedContracts.PageIndex,
            pagedContracts.PageSize,
            pagedContracts.TotalCount
        );

        return Result.Ok(pagedDtos);
    }

    private static ContractListDto MapToDto(Contract contract)
    {
        return new ContractListDto
        {
            ContractId = contract.Id,
            ClientId = contract.ClientId,
            TrackingId = contract.TrackingId,
            EndDate = contract.EndDate,
            ContractTypeId = contract.ContractTypeEnum,
            StartDate = contract.StartDate,
            Value = contract.Value,
            DigitalFileUrl = contract.DigitalFileUrl,
            CreatedAt = contract.CreatedAt,
            UpdatedAt = contract.UpdatedAt,
            Status = contract.Status,
            ContractType = new ContractTypeDto
            {
                Type = contract.ContractType.ContractTypeEnum,
                Name = contract.ContractType.Name
            }
        };
    }
}
