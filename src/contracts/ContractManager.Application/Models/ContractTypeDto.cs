using ContractManager.Data.Entities.Enums;

namespace ContractManager.Application.Models;
public class ContractTypeDto
{
    public required ContractTypeEnum Type { get; set; }
    public required string Name { get; set; } = default!;
}
