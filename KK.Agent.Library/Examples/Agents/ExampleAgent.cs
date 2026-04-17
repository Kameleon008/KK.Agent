using KK.Agent.Library.AgentEngine;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Mcp;

namespace KK.Agent.Library.Examples.Agents
{
    public class ExampleAgent(OpenApiClient provider, AgentLogger logger, ChatHistoryProvider historyProvider, ConfigMcpServers mcp) : AgentBase(provider, logger, mcp)
    {
        protected override string SystemPrompt { get; set; } = string.Empty;
    }
}
