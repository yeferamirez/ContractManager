using ContractManager.Shared.Application.Security.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ContractManager.Shared.Application.Security;

public class JwtGeneratorService : IJwtGeneratorService
{
    private readonly TimeProvider timeProvider;

    public JwtGeneratorService(TimeProvider timeProvider)
    {
        this.timeProvider = timeProvider;
    }

    public string BuildToken(
        JwtSettings jwtSettings,
        Claim[] claims)
    {
        if (jwtSettings.UseCognito)
        {
            throw new InvalidOperationException("AWS Cognito genera los tokens externamente cuando UseCognito=true.");
        }

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var expiration = now.AddMinutes(jwtSettings.ExpirationTimeInMinutes);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience ?? jwtSettings.Issuer,
            claims: claims,
            notBefore: now,
            expires: expiration,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
