using ContractManager.Data.Entities.Enums;
using ContractManager.Shared.Core;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContractManager.Data.Entities;
public class Contract : IEntity
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public byte ContractTypeId { get; set; }
    public ContractStatus Status { get; set; }
    public double Value { get; set; }
    public string DigitalFileUrl { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string TrackingId { get; set; } = default!;

    [NotMapped]
    public ContractTypeEnum ContractTypeEnum
    {
        get => (ContractTypeEnum)ContractTypeId;
        set => ContractTypeId = (byte)value;
    }

    public virtual ContractType ContractType { get; set; } = null!;
}
