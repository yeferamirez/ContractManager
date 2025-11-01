using ContractManager.Data.Entities.Enums;
using ContractManager.Shared.Application.Models;

namespace ContractManager.Api.Models.Contract;

public class ContractFilterModel : BaseFilterModel
{
    public DateTime? EndDate { get; set; }
    public ContractStatus? Status { get; set; }
    public OrderByProcedure? OrderByEnum { get; set; }
    public SortDirection SortDirection { get; set; } = SortDirection.Desc;
    public ContractTypeEnum? ContractType { get; set; }
}
