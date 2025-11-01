using ContractManager.Data.Entities.Enums;
using ContractManager.Shared.Core;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContractManager.Data.Entities;
public class ContractType : IEntity
{
    public byte Id { get; set; }
    public string Name { get; set; } = null!;

    [NotMapped]
    public ContractTypeEnum ContractTypeEnum => (ContractTypeEnum)Id;
}
