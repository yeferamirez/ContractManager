namespace ContractManager.Shared.Core.Context;

public record CurrentEnvironment(string Name);

public static class CurrentEnvironmentExtensions
{
    public static bool IsDevelopment(this CurrentEnvironment environment) => environment.Name == "Development";

    public static bool IsLocal(this CurrentEnvironment environment) => environment.Name == "Local";

    public static bool IsE2E(this CurrentEnvironment environment) => environment.Name == "E2E";

    public static bool IsProduction(this CurrentEnvironment environment) => environment.Name == "Production";

    public static bool IsIntegrationTests(this CurrentEnvironment environment) => environment.Name == "IntegrationTests";
}
