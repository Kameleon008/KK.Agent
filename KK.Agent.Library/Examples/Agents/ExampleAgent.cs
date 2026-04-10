using KK.Agent.Library.Agents;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Mcp;

namespace KK.Agent.Library.Examples.Agents
{
    public class ExampleAgent(OpenApiClient provider, AgentLogger logger, AgentHistory history, ConfigMcpServers mcp) : AgentBase(provider, logger, history, mcp)
    {
        protected override string SystemPrompt { get; set; } = string.Empty;
    }
}
