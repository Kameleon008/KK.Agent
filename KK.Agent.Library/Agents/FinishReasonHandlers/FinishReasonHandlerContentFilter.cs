using KK.Agent.Library.Clients.OpenApi.V1;
using KK.Agent.Library.Mcp;

namespace KK.Agent.Library.Agents.FinishReasonHandlers
{
    public class FinishReasonHandlerContentFilter : IFinishReasonHandler
    {
        public bool Handles(string finishReason) => finishReason == "content_filter";

        public Task<string?> HandleAsync(string caller, ChatCompletionChoice choice, ChatHistory history, List<McpClient> mcpClients)
        {
            Console.WriteLine($"[Agent]: Content was omitted due to a flag from our content filters...");
            return Task.FromResult(choice.Message?.Content);
        }
    }
}
