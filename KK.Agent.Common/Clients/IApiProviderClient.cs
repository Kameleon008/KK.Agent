using KK.Agent.Common.Clients.OpenApi.V1;

namespace KK.Agent.Common.Clients
{
    public interface IApiProviderClient
    {
        public string Model { get; }

        public Task<ChatCompletionsResponse> GetChatCompletionsAsync(ChatCompletionsRequest request, CancellationToken cancellationToken = default);

        public IAsyncEnumerable<ChatCompletionsChunk> GetChatCompletionsStreamAsync(ChatCompletionsRequest request, CancellationToken cancellationToken = default);

    }

    public interface IChatMessage
    {
        public string Role { get; set; }
    }
}
