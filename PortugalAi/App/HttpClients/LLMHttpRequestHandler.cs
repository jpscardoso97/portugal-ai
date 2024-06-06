namespace App.HttpClients;

public class LlmHttpRequestHandler: HttpClientHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.RequestUri != null &&
            request.RequestUri.Host.Equals("api.openai.com", StringComparison.OrdinalIgnoreCase))
        {
            request.RequestUri = new Uri($"http://localhost:8080{request.RequestUri.PathAndQuery}");
        }

        return base.SendAsync(request, cancellationToken);
    }
}