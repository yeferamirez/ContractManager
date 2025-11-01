using Amazon.DynamoDBv2.DataModel;

namespace ContractManager.Data.Entities;

[DynamoDBTable("ContractDocuments")]
public class ContractDocument
{
    [DynamoDBHashKey]
    public string ContractId { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string DigitalFileUrl { get; set; } = string.Empty;

    [DynamoDBProperty]
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
