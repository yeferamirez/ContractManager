using ContractManager.Shared.Application.Clients;
using ContractManager.Shared.Application.Security;
using ContractManager.Shared.Application.Security.Configuration;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Security.Claims;

namespace ContractManager.Shared.Api.Swagger;
public class AddDefaultAuthTokenFilter : IDocumentFilter
{
    private readonly IJwtGeneratorService tokenService;
    private readonly JwtSettings jwtSettings;
    private readonly ClientsSettings clientsSettings;

    public AddDefaultAuthTokenFilter(
        IJwtGeneratorService tokenService,
        JwtSettings jwtSettings,
        ClientsSettings clientsSettings)
    {
        this.tokenService = tokenService;
        this.jwtSettings = jwtSettings;
        this.clientsSettings = clientsSettings;
    }

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        if (jwtSettings.UseCognito)
        {
            swaggerDoc.Components.SecuritySchemes["Bearer"].Description =
                "Autenticación con AWS Cognito (usa un token real obtenido del pool).";
            return;
        }

        var defaultToken = $"Bearer {this.tokenService.BuildToken(this.jwtSettings, GetClaims(true))}";

        if (swaggerDoc.Components.SecuritySchemes.ContainsKey("Bearer"))
        {
            swaggerDoc.Components.SecuritySchemes["Bearer"].Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            };

            swaggerDoc.Components.SecuritySchemes["Bearer"].Description = defaultToken;
        }
    }

    private Claim[] GetClaims(bool isAdmin)
    {
        var userId = isAdmin ? clientsSettings.DefaultAdmin.Id : clientsSettings.DefaultAnalyst.Id;

        var claims = new List<Claim>
        {
            new Claim("id", userId.ToString()),
            new Claim("name", isAdmin ? "Admin User" : "Analyst User"),
            new Claim(ClaimTypes.Email, isAdmin ? "admin@local.test" : "analyst@local.test"),
            new Claim("role", isAdmin ? "Admin" : "Analyst")
        };

        var permissions = isAdmin ? clientsSettings.DefaultAdmin.Permissions : clientsSettings.DefaultAnalyst.Permissions;

        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permissions", permission));
        }

        return claims.ToArray();
    }
}
