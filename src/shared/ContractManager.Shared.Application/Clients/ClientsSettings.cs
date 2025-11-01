namespace ContractManager.Shared.Application.Clients;

public record ClientsSettings(DefaultUserSettings DefaultAdmin, DefaultUserSettings DefaultAnalyst);

public record DefaultUserSettings(int Id, string[] Permissions);
