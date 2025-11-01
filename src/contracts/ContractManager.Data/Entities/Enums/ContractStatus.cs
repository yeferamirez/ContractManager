using System.Text.Json.Serialization;

namespace ContractManager.Data.Entities.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ContractStatus : byte
{
    Active = 1,
    Expired = 2,
    Terminated = 3
}
