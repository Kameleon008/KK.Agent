using KK.Agent.Library.AgentEngine;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Mcp;

namespace KK.Agent.ConsoleClient
{
    public class OrchestratorAgent(OpenApiClient client, AgentLogger logger, McpClient mcp) : AgentBase(client, logger, mcp)
    {
        protected override string SystemPrompt { get; set; } = string.Empty;
    }
}
