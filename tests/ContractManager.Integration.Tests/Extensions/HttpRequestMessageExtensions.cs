using ContractManager.Integration.Tests.Models;
using ContractManager.Shared.Application.Security;
using ContractManager.Shared.Application.Security.Configuration;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace ContractManager.Integration.Tests.Extensions;
public static class HttpRequestMessageExtensions
{
    private static StringContent GetStringContent(object obj)
        => new(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");

    public static HttpRequestMessage AddJwt(
        this HttpRequestMessage requestMessage,
        JwtSettings jwtSettings,
        PermissionType[]? permissions = null,
        int userId = 1)
    {
        var jwtService = new JwtGeneratorService(TimeProvider.System);
        var jwt = jwtService.BuildToken(jwtSettings, GetClaims(userId, permissions ?? []));
        requestMessage.Headers.Add("Authorization", $"Bearer {jwt}");
        return requestMessage;
    }

    public static HttpRequestMessage AddInvalidJwt(this HttpRequestMessage requestMessage)
    {
        requestMessage.Headers.Add("Authorization", $"Bearer invalid");
        return requestMessage;
    }

    public static HttpRequestMessage AddHeader(this HttpRequestMessage requestMessage, string key, string value)
    {
        requestMessage.Headers.Add(key, value);
        return requestMessage;
    }

    public static async Task<ApiResponseModel<T>> SendAsync<T>(
        this HttpRequestMessage requestMessage,
        HttpClient client,
        object? model = null,
        bool ensureSuccessStatusCode = false) where T : class
    {
        if (model != null)
        {
            requestMessage.Content = GetStringContent(model);
        }

        var response = await client.SendAsync(requestMessage);

        if (ensureSuccessStatusCode)
        {
            response.EnsureSuccessStatusCode();
        }

        T? result;

        switch (typeof(T))
        {
            case Type t when t == typeof(byte[]):
                result = await response.GetResponseByteArrayAsync() as T;
                break;
            case Type t when t != typeof(EmptyContentModel):
                result = await response.GetResponseContentAsync<T>();
                break;
            default:
                result = null;
                break;
        }

        return new ApiResponseModel<T> { Content = result, HttpResponse = response };
    }

    public static async Task<ApiResponseModel<string>> SendAsync(
        this HttpRequestMessage requestMessage,
        HttpClient client,
        object model,
        bool ensureSuccessStatusCode = false)
    {
        if (model != null)
        {
            requestMessage.Content = GetStringContent(model);
        }

        var response = await client.SendAsync(requestMessage);

        if (ensureSuccessStatusCode)
        {
            response.EnsureSuccessStatusCode();
        }

        return new ApiResponseModel<string> { Content = await response.Content.ReadAsStringAsync(), HttpResponse = response };
    }

    private static Claim[] GetClaims(int userId, PermissionType[] permissions)
    {
        var claims = new List<Claim>
        {
            new Claim("id", userId.ToString()),
            new Claim("email", "any@any.com"),
            new Claim("name", "Any Name"),
            new Claim("role", "Admin")
        };

        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permissions", permission.ToString()));
        }

        return claims.ToArray();
    }
}
