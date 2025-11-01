using ContractManager.Data.Entities.Enums;

namespace ContractManager.Application.Models;
public class ContractListDto
{
    public Guid ContractId { get; set; }
    public Guid ClientId { get; set; }
    public ContractStatus Status { get; set; }
    public ContractTypeEnum ContractTypeId { get; set; }
    public double Value { get; set; }
    public string DigitalFileUrl { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string TrackingId { get; set; } = default!;
    public ContractTypeDto ContractType { get; set; } = default!;
}
