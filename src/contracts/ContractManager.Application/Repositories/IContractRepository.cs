using ContractManager.Data.Entities;
using ContractManager.Data.Entities.Enums;
using ContractManager.Shared.Data;

namespace ContractManager.Application.Repositories;
public interface IContractRepository : IRepository<Contract>
{
    Task<List<Contract>?> GetContractExpiredAsync(DateTime date);

    Task<IPagedList<Contract>> GetAllContractsAsync(int page,
        int pageSize,
        DateTime? endDate,
        ContractStatus? status,
        ContractTypeEnum? contractType,
        OrderByProcedure? orderBy,
        SortDirection? sortDirection);
}
