using KK.Agent.Library.Clients.OpenApi.Models.V1;

namespace KK.Agent.Library.Clients
{
    internal interface IChatCompletionsApiClient
    {
        public Task<ChatCompletionsResponse> GetChatCompletionsAsync();
    }
}
