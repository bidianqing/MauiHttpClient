using MauiHttpClient.Services.Dialog;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace MauiHttpClient.Services.Request
{
    /// <summary>
    /// 
    /// https://stackoverflow.com/a/72427377
    /// https://stackoverflow.com/a/30163316
    /// https://github.com/dotnet/maui/discussions/3201
    /// </summary>
    public class RequestService : IRequestService
    {
        private readonly IDialogService _dialogService;
        private readonly Lazy<HttpClient> _httpClient =
            new(() =>
                {
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.BaseAddress = new Uri(Urls.Domain);
                    return httpClient;
                },
                LazyThreadSafetyMode.ExecutionAndPublication);

        public RequestService(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public async Task<TResult> GetAsync<TResult>(string uri)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);

            return await this.SendAsync<TResult>(request);
        }

        public async Task<TResult> PostAsync<TResult>(string uri, string json)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

            return await this.SendAsync<TResult>(request);
        }

        public async Task<TResult> PostAsync<TRequest, TResult>(string uri, TRequest data) where TRequest : class, new()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = new StringContent(JsonConvert.SerializeObject(data, Formatting.None), Encoding.UTF8, MediaTypeNames.Application.Json);

            return await this.SendAsync<TResult>(request);
        }

        public async Task<TResult> SendAsync<TResult>(HttpRequestMessage request)
        {
            var response = await _httpClient.Value.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            return await this.HandleResponse<TResult>(response);
        }

        private async Task<TResult> HandleResponse<TResult>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest)
            {
                var stream = await response.Content.ReadAsStringAsync();
                var resultModel = System.Text.Json.JsonSerializer.Deserialize<ResultModel<TResult>>(stream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });


                //using var stream = await response.Content.ReadAsStreamAsync();
                //using var sr = new StreamReader(stream);
                //using var reader = new JsonTextReader(sr);
                //var serializer = new JsonSerializer();
                //var resultModel = serializer.Deserialize<ResultModel<TResult>>(reader);

                //var content = await response.Content.ReadAsStringAsync();
                //var resultModel = JsonConvert.DeserializeObject<ResultModel<TResult>>(content);
                if (!resultModel.Success)
                {
                    await _dialogService.ShowAlertAsync(null, resultModel.Message, "确定");
                }

                return resultModel.Data;
            }
            else if(response.StatusCode == HttpStatusCode.Unauthorized)
            {

            }

            await _dialogService.ShowAlertAsync(null, $"服务器开小差了，状态码：{(int)response.StatusCode}，时间：{DateTime.Now}", "确定");
            return default(TResult);
        }
    }
}
