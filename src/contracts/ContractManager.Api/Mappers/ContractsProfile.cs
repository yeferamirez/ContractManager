using AutoMapper;
using ContractManager.Api.Models.Contract;
using ContractManager.Application.UseCases.CreateContract;

namespace ContractManager.Api.Mappers;

public class ContractsProfile : Profile
{
    public ContractsProfile()
    {
        CreateMap<CreateContractCommandModel, CreateContractCommand>();
    }
}
