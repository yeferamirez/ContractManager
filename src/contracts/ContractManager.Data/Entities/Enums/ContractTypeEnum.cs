using System.Text.Json.Serialization;

namespace ContractManager.Data.Entities.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ContractTypeEnum : byte
{
    Servicio = 1,
    Mantenimiento = 2
}
