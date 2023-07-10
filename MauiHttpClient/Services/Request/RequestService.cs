using Newtonsoft.Json;
using System.Net.Mime;
using System.Text;

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
        private readonly HttpClient _client;
        public RequestService(HttpClient client)
        {
            _client = client;
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
            var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            return await this.HandleResponse<TResult>(response);
        }

        private async Task<TResult> HandleResponse<TResult>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var rm = await this.ReadResponse<TResult>(response);
                    await App.Current.MainPage.DisplayAlert("Warning", rm.Message, "确定");

                    return rm.Data;
                }

                await App.Current.MainPage.DisplayAlert("Error", $"服务器开小差了，状态码：{response.StatusCode}，时间：{DateTime.Now}", "确定");
                return default(TResult);
            }

            var resultModel = await this.ReadResponse<TResult>(response);

            if (!resultModel.Success)
            {
                await App.Current.MainPage.DisplayAlert("Warning", resultModel.Message, "确定");
                return default(TResult);
            }

            return resultModel.Data;
        }

        private async Task<ResultModel<TResult>> ReadResponse<TResult>(HttpResponseMessage response)
        {
            using var stream = await response.Content.ReadAsStreamAsync();
            using var sr = new StreamReader(stream);
            using var reader = new JsonTextReader(sr);
            var serializer = new JsonSerializer();

            return serializer.Deserialize<ResultModel<TResult>>(reader);
        }
    }
}
