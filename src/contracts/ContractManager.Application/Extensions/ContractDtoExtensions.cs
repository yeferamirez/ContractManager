using ContractManager.Application.Models;
using ContractManager.Data.Entities;
using ContractManager.Data.Entities.Enums;

namespace ContractManager.Application.Extensions;
public static class ContractDtoExtensions
{
    public static GetByIdContractDto ToGetByIdDto(
        this Contract contract)
    {
        return new GetByIdContractDto
        {
            ClientId = contract.ClientId,
            ContractType = (ContractTypeEnum) contract.ContractTypeId,
            CreatedAt = contract.CreatedAt,
            DigitalFileUrl = contract.DigitalFileUrl,
            EndDate = contract.EndDate, 
            StartDate = contract.StartDate,
            Status = (ContractStatus)contract.Status,
            TrackingId = contract.TrackingId,
            UpdatedAt = contract.UpdatedAt,
            Value = contract.Value
        };
    }
}
