namespace ContractManager.Shared.Application.Security.Configuration;
public record JwtSettings(string Key, string Issuer, string Audience, int ExpirationTimeInMinutes, bool UseCognito = false);
