using System.Text.Json.Serialization;

namespace ContractManager.Data.Entities.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderByProcedure
{
    Date,
    ClosingDate
}
