using KK.Agent.Library.Clients.OpenApi.V1;

namespace KK.Agent.Library.Clients
{
    public interface IChatCompletionsApiClient
    {
        public Task<ChatCompletionsResponse> GetChatCompletionsAsync(ChatCompletionsRequest request, CancellationToken cancellationToken = default);

        public IAsyncEnumerable<ChatCompletionsChunk> GetChatCompletionsStreamAsync(ChatCompletionsRequest request, CancellationToken cancellationToken);

    }

    public interface IChatMessage
    {
        public string Role { get; set; }
    }
}
