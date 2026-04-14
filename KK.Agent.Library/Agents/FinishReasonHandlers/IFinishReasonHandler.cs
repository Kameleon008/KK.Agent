using KK.Agent.Library.Clients.OpenApi.V1;
using KK.Agent.Library.Mcp;

namespace KK.Agent.Library.Agents.FinishReasonHandlers
{
    public interface IFinishReasonHandler
    {
        bool Handles(string finishReason);

        Task<string?> HandleAsync(string caller, ChatCompletionChoice choice, ChatHistory history, List<McpClient> mcpClients);
    }
}
