namespace MauiHttpClient.Services.Request
{
    public interface IRequestService
    {
        Task<TResult> GetAsync<TResult>(string uri);

        Task<TResult> PostAsync<TResult>(string uri, string json);

        Task<TResult> PostAsync<TRequest, TResult>(string uri, TRequest data) where TRequest : class, new();

        Task<TResult> SendAsync<TResult>(HttpRequestMessage request);
    }
}
