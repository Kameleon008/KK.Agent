using KK.Agent.Library.AgentEngine;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Mcp;

namespace KK.Agent.ConsoleClient
{
    public class OrchestratorAgent(OpenApiClient provider, AgentLogger logger, McpClient mcp) : AgentBase(provider, logger, mcp)
    {
        protected override string SystemPrompt { get; set; } = string.Empty;
    }
}
