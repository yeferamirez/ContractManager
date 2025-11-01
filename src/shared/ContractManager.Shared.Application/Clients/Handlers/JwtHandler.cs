using ContractManager.Shared.Application.Security;
using ContractManager.Shared.Application.Security.Configuration;

namespace ContractManager.Shared.Application.Clients.Handlers;
public class JwtHandler : DelegatingHandler
{
    private readonly JwtSettings contractJwtSettings;
    private readonly IJwtGeneratorService jwtGeneratorService;
    private readonly ClientsSettings clientsSettings;

    public JwtHandler(
        JwtSettings contractJwtSettings,
        IJwtGeneratorService jwtGeneratorService,
        ClientsSettings clientsSettings)
    {
        this.contractJwtSettings = contractJwtSettings;
        this.jwtGeneratorService = jwtGeneratorService;
        this.clientsSettings = clientsSettings;
    }

    //protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    //{
    //    _ = bool.TryParse(request.Options.GetValueOrDefault(ClientConstants.RequestOptionsKeys.IsAdmin, string.Empty)!.ToString()!, out var isAdmin);

    //    var jwtSettings = isAdmin ? contractJwtSettings.admin : contractJwtSettings.Sites;

    //    var token = jwtGeneratorService.BuildToken(jwtSettings, this.GetClaims(isAdmin));

    //    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

    //    return await base.SendAsync(request, cancellationToken);
    //}

    //private Claim[] GetClaims(bool isSuperAdmin)
    //{
    //    var userId = isSuperAdmin ? clientsSettings.DefaultAdminUser.Id : clientsSettings.DefaultUser.Id;

    //    var claims = new List<Claim>
    //    {
    //        new Claim("id", userId.ToString())
    //    };

    //    foreach (var permission in clientsSettings.DefaultUser.Permissions)
    //    {
    //        claims.Add(new Claim("permissions", permission));
    //    }

    //    return claims.ToArray();
    //}
}
