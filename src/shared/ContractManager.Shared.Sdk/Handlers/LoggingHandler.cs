using Microsoft.Extensions.Logging;

namespace ContractManager.Shared.Sdk.Handlers;
public class LoggingHandler(ILogger<LoggingHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var requestBodyContent = request.Content != null ? (await request.Content!.ReadAsStringAsync(cancellationToken).ConfigureAwait(false)) : string.Empty;

        logger.LogInformation("[LoggingHandler] Before Request: {Method} {Url} RequestHeaders: {AllRequestHeaders} RequestBodyContent:{RequestBodyContent}",
            request.Method,
            request.RequestUri,
            request.Headers.ToString(),
            requestBodyContent);

        var response = await base.SendAsync(request, cancellationToken)
            .ConfigureAwait(false);


        logger.LogInformation("[LoggingHandler] Response: {StatusCode} ResponseHeaders: {AllResponseHeaders} ResponseBodyContent:{ResponseBodyContent}",
            response.StatusCode,
            response.Headers.ToString(),
            await response.Content!.ReadAsStringAsync(cancellationToken).ConfigureAwait(false));

        return response;
    }
}
