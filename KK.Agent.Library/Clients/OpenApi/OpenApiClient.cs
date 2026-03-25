using KK.Agent.Library.Clients.OpenApi.Models.V1;
using KK.Agent.Library.Clients.OpenApi.Models.V1.Builders;
using KK.Agent.Library.Configuration.Models;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Text;

namespace KK.Agent.Library.Clients.OpenApi
{
    public class OpenApiClient(ConfigProvider configuration) : IChatCompletionsApiClient
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

        public async Task<ChatCompletionsResponse> GetChatCompletionsAsync()
        {
            var body = new ChatCompletionsRequestBuilder()
                .SetModel(configuration.Model)
                .AddMessage("user", "HelloWorld!")
                .SetStream(false)
                .Build()
                .ToHttpContent();

            var result = await this._httpClient.PostAsync("/v1/chat/completions", body);

            var x = await result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ChatCompletionsResponse>(x);
        }


        public async IAsyncEnumerable<string> GetChatCompletionsStreamAsync([EnumeratorCancellation] CancellationToken ct = default)
        {
            var body = new ChatCompletionsRequestBuilder()
                .SetModel(configuration.Model)
                .AddMessage("user", "HelloWorld!")
                .SetStream(true)
                .Build()
                .ToHttpContent();

            using var request = new HttpRequestMessage(HttpMethod.Post, "/v1/chat/completions") { Content = body };

            // 1. Get headers only
            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
            response.EnsureSuccessStatusCode();

            // 2. Open the stream
            using var stream = await response.Content.ReadAsStreamAsync(ct);
            using var reader = new StreamReader(stream);

            // 3. Read line by line and yield back to the caller
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync(ct);
                if (string.IsNullOrWhiteSpace(line) || line == "data: [DONE]") continue;

                if (line.StartsWith("data: "))
                {
                    var json = line.Substring(6);
                    var chunk = JsonConvert.DeserializeObject<ChatCompletionsResponse>(json);
                    var content = chunk?.Choices.First().Delta.Content;

                    if (!string.IsNullOrEmpty(content))
                    {
                        yield return content; // This sends the word to the caller immediately
                    }
                }
            }
        }
    }
}
