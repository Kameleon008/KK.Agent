using KK.Agent.Library.AgentEngine;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Tools;

namespace KK.Agent.Library.Examples.Agents
{
    public class ExampleAgent(OpenApiClient client, ToolsProvider tools, AgentLogger logger) : AgentBase(client, tools, logger)
    {
        protected override string SystemPrompt { get; set; } = string.Empty;
    }
}
