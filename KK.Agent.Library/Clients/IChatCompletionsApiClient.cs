using KK.Agent.Library.Clients.OpenApi.V1;

namespace KK.Agent.Library.Clients
{
    public interface IChatCompletionsApiClient
    {
        public Task<ChatCompletionsResponse> GetChatCompletionsAsync(IEnumerable<IChatMessage> messages, CancellationToken cancellationToken);

        public IAsyncEnumerable<ChatCompletionsResponse> GetChatCompletionsStreamAsync(IEnumerable<IChatMessage> messages, CancellationToken cancellationToken);

    }

    public interface IChatMessage
    {
        public string Role { get; set; }

        public string Content { get; set; }
    }
}
