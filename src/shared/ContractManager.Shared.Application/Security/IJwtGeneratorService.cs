using ContractManager.Shared.Application.Security.Configuration;
using System.Security.Claims;

namespace ContractManager.Shared.Application.Security;
public interface IJwtGeneratorService
{
    public string BuildToken(
        JwtSettings jwtSettings,
        Claim[] claims);
}
