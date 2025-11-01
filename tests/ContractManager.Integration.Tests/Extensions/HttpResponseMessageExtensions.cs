using System.Text.Json;

namespace ContractManager.Integration.Tests.Extensions;
public static class HttpResponseMessageExtensions
{
    public static async Task<T?> GetResponseContentAsync<T>(this HttpResponseMessage response)
    {
        using var stream = await response.Content.ReadAsStreamAsync();
        if (stream.Length == 0)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(stream, JsonOptions);
    }

    public static async Task<byte[]> GetResponseByteArrayAsync(this HttpResponseMessage response)
    {
        return await response.Content.ReadAsByteArrayAsync();
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
}
