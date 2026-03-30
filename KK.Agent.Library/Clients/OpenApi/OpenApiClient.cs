using KK.Agent.Library.Clients.OpenApi.V1.Builders;
using KK.Agent.Library.Clients.OpenApi.V1;
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

        public string Model => configuration.Model;

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

        public async Task<ChatCompletionsResponse> GetChatCompletionsAsync(ChatCompletionsRequest request, CancellationToken cancelationToken = default)
        {
            var body = request.ToHttpContent();

            var result = await this._httpClient.PostAsync("/v1/chat/completions", body, cancelationToken);

            var x = await result.Content.ReadAsStringAsync(cancelationToken);

            return JsonConvert.DeserializeObject<ChatCompletionsResponse>(x);
        }

        public async IAsyncEnumerable<ChatCompletionsChunk> GetChatCompletionsStreamAsync(ChatCompletionsRequest request, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var body = request.ToHttpContent();

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/v1/chat/completions") { Content = body };

            // 1. Get headers only
            using var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (response.IsSuccessStatusCode == false)
            {
                var result = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new Exception($"Request failed with status code {response.StatusCode}: {result}");
            }


            // 2. Open the stream
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);

            while (await reader.ReadLineAsync(cancellationToken) is { } line)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                if (line.StartsWith("data: [DONE]") || line.StartsWith("data: ") is false)
                {
                    break;
                }

                var json = line.Substring(6);
                var chunk = JsonConvert.DeserializeObject<ChatCompletionsChunk>(json);

                if (chunk != null)
                {
                    yield return chunk;
                }
            }
        }
    }
}
