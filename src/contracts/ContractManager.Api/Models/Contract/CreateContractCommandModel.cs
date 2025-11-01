using ContractManager.Data.Entities.Enums;
using System.Text.Json.Serialization;

namespace ContractManager.Api.Models.Contract;

public class CreateContractCommandModel
{
    public Guid ClientId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ContractTypeEnum ContractTypeId { get; set; }
    public double Value { get; set; }
    public string DigitalFileUrl { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
