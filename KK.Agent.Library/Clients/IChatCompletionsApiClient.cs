using KK.Agent.Library.Clients.OpenApi.V1;
using System.Runtime.CompilerServices;

namespace KK.Agent.Library.Clients
{
    internal interface IChatCompletionsApiClient
    {
        public Task<ChatCompletionsResponse> GetChatCompletionsAsync(CancellationToken cancelationToken);

        public IAsyncEnumerable<ChatCompletionsResponse> GetChatCompletionsStreamAsync(CancellationToken cancellationToken);

    }
}
