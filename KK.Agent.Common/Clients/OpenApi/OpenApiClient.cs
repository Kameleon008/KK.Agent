using System.Runtime.CompilerServices;
using System.Text;
using KK.Agent.Common.Clients.OpenApi.V1;
using Newtonsoft.Json;

namespace KK.Agent.Common.Clients.OpenApi
{
    public class OpenApiClient(Configuration.OpenApi configuration) : IApiProviderClient
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

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/v1/chat/completions") { Content = body };

            var response = await _httpClient.SendAsync(httpRequest, cancelationToken);

            var content = await response.Content.ReadAsStringAsync(cancelationToken);

            var result = JsonConvert.DeserializeObject<ChatCompletionsResponse>(content);

            return result ?? throw new Exception("ChatCompletionsResponse - invalid deserialization");
        }

        public async IAsyncEnumerable<ChatCompletionsChunk> GetChatCompletionsStreamAsync(ChatCompletionsRequest request, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var body = request.ToHttpContent();

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/v1/chat/completions") { Content = body };

            var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (response.IsSuccessStatusCode == false)
            {
                var result = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new Exception($"Request failed with status code {response.StatusCode}: {result}");
            }

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

                yield return chunk ?? throw new Exception("ChatCompletionsResponse - invalid deserialization");
            }
        }
    }
}
