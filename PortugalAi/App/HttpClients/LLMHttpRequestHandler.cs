namespace App.HttpClients;

public class LlmHttpRequestHandler: HttpClientHandler
{
    private const string host = "http://host.docker.internal:8080";
    //private const string host = "http://localhost:8080";

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.RequestUri != null &&
            request.RequestUri.Host.Equals("api.openai.com", StringComparison.OrdinalIgnoreCase))
        {
            request.RequestUri = new Uri($"{host}{request.RequestUri.PathAndQuery}");
        }
        
        return base.SendAsync(request, cancellationToken);
    }
}