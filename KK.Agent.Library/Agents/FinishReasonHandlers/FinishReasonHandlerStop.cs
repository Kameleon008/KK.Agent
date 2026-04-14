using KK.Agent.Library.Clients.OpenApi.V1;
using KK.Agent.Library.Mcp;

namespace KK.Agent.Library.Agents.FinishReasonHandlers
{
    public class FinishReasonHandlerStop : IFinishReasonHandler
    {
        public bool Handles(string finishReason) => finishReason == "stop";

        public Task<string?> HandleAsync(string caller, ChatCompletionChoice choice, ChatHistory history, List<McpClient> mcpClients)
        {
            return Task.FromResult(choice.Message?.Content);
        }
    }
}
