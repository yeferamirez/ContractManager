using Amazon.DynamoDBv2.DataModel;

namespace ContractManager.Data.Entities;

[DynamoDBTable("AuditLogs")]
public class AuditLog
{
    [DynamoDBHashKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [DynamoDBProperty]
    public string Action { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string Entity { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string EntityId { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string User { get; set; } = string.Empty;

    [DynamoDBProperty]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
