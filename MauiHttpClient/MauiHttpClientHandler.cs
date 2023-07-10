namespace MauiHttpClient
{
    public class MauiHttpClientHandler : HttpClientHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Process request


            HttpResponseMessage httpResponseMessage = await base.SendAsync(request, cancellationToken);

            // Process response


            return httpResponseMessage;
        }
    }
}
