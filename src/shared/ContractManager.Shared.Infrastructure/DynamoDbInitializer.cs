using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace ContractManager.Shared.Infrastructure;
public class DynamoDbInitializer
{
    private readonly IAmazonDynamoDB _client;

    public DynamoDbInitializer(IAmazonDynamoDB client)
    {
        _client = client;
    }

    public async Task InitializeAsync()
    {
        await EnsureTableExistsAsync("AuditLogs", "Id");

        await EnsureTableExistsAsync("ContractDocuments", "ContractId");
    }

    private async Task EnsureTableExistsAsync(string tableName, string partitionKey)
    {
        var existingTables = await _client.ListTablesAsync();

        if (existingTables.TableNames.Contains(tableName))
            return;

        var createRequest = new CreateTableRequest
        {
            TableName = tableName,
            AttributeDefinitions = new List<AttributeDefinition>
            {
                new AttributeDefinition
                {
                    AttributeName = partitionKey,
                    AttributeType = "S"
                }
            },
            KeySchema = new List<KeySchemaElement>
            {
                new KeySchemaElement
                {
                    AttributeName = partitionKey,
                    KeyType = "HASH"
                }
            },
            ProvisionedThroughput = new ProvisionedThroughput
            {
                ReadCapacityUnits = 5,
                WriteCapacityUnits = 5
            }
        };

        await _client.CreateTableAsync(createRequest);
    }
}
