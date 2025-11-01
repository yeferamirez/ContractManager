using ContractManager.Application.Repositories;
using ContractManager.Data.Entities;
using ContractManager.Data.Entities.Enums;
using ContractManager.Shared.Data;
using Microsoft.EntityFrameworkCore;

namespace ContractManager.Data.EF.Repositories;
public class ContractRepository(ContractsDbContext context) : EFRepository<Contract>(context), IContractRepository
{
    public async Task<List<Contract>?> GetContractExpiredAsync(DateTime date)
    {
        IQueryable<Contract>? query = context.Contracts
            .Include(t => t.ContractType)
            .AsNoTracking()
            .Where(c => c.EndDate <= date);

        return await query.ToListAsync();
    }

    public async Task<IPagedList<Contract>> GetAllContractsAsync(
        int page,
        int pageSize,
        DateTime? endDate,
        ContractStatus? status,
        ContractTypeEnum? contractType,
        OrderByProcedure? orderBy,
        SortDirection? sortDirection)
    {
        var query = context.Contracts
            .Include(t => t.ContractType)
            .AsNoTracking();

        if (endDate.HasValue)
        {
            query = query.Where(t => t.EndDate <= endDate);
        }

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        if (contractType.HasValue)
        {
            var contractTypeId = (byte)contractType.Value;
            query = query.Where(t => t.ContractTypeId == contractTypeId);
        }

        switch (orderBy)
        {
            case OrderByProcedure.Date:
            default:
                query = sortDirection.HasValue && sortDirection.Value == SortDirection.Asc
                    ? query.OrderBy(c => c.StartDate)
                    : query.OrderByDescending(c => c.StartDate);
                break;
            case OrderByProcedure.ClosingDate:
                query = sortDirection.HasValue && sortDirection.Value == SortDirection.Asc
                    ? query.OrderBy(c => c.EndDate)
                    : query.OrderByDescending(c => c.EndDate);
                break;
        }

        return await new PagedList<Contract>().Async(query, page, pageSize);
    }
}
