namespace ContractManager.Integration.Tests;
public class BaseIntegrationTests : BaseTest
{
    public HttpRequestMessage CreatePostRequest(string uri)
    {
        return new HttpRequestMessage(HttpMethod.Post, uri);
    }

    public HttpRequestMessage CreatePutRequest(string uri)
    {
        return new HttpRequestMessage(HttpMethod.Put, uri);
    }

    public HttpRequestMessage CreateGetRequest(string uri, object? query = null)
    {
        var queryString = GetQueryString(query);
        return new HttpRequestMessage(HttpMethod.Get, uri + queryString);
    }

    private string GetQueryString(object? query)
    {
        if (query == null)
        {
            return string.Empty;
        }

        var properties = query.GetType().GetProperties();
        var queryString = string.Join("&", properties.Select(p => $"{p.Name}={p.GetValue(query)}"));
        return $"?{queryString}";
    }
}
