namespace ContractManager.Application.Configuration;

public record class CachedSettings(string Host, int Port, int Database, int DefaultTtlMinutes);
