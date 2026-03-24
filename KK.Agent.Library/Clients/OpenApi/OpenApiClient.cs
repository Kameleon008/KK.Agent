using KK.Agent.Library.Configuration.Models;
using System.Text;
using KK.Agent.Library.Clients.OpenApi.Models.V1;
using Newtonsoft.Json;

namespace KK.Agent.Library.Clients.OpenApi
{
    public class OpenApiClient(ConfigProvider configuration) : IChatCompletions
    {
        private readonly HttpClient _httpClient = new()
        {
            BaseAddress = new Uri(configuration.Endpoint)
        };

        public async Task<ModelsResponse> GetModelsAsync()
        {
            try
            {
                var result = await this._httpClient.GetAsync("/v1/models");
                var content = await result.Content.ReadAsStringAsync();
                var deserialized = JsonConvert.DeserializeObject<ModelsResponse>(content);

                return deserialized;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<object> GetResponsesAsync()
        {
            var body = new StringContent("", Encoding.UTF8, "application/json");
            var result = await this._httpClient.PostAsync("/v1/responses", body);

            return result;
        }

        public async Task<object> GetChatCompletionsAsync()
        {
            var body = new StringContent("", Encoding.UTF8, "application/json");
            var result = await this._httpClient.PostAsync("/v1/chat/completions", body);

            return result;
        }

        public async Task<object> GetCompletionsAsync()
        {
            var body = new StringContent("", Encoding.UTF8, "application/json");
            var result = await this._httpClient.PostAsync("/v1/completions", body);

            return result;
        }

        public async Task<object> GetEmbeddingsAsync()
        {
            var body = new StringContent("", Encoding.UTF8, "application/json");
            var result = await this._httpClient.PostAsync("/v1/embeddings", body);

            return result;
        }

        public Task<T> GetChatCompletionsAsync<T>()
        {
            throw new NotImplementedException();
        }
    }
}
