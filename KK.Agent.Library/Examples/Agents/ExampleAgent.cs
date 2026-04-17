using KK.Agent.Library.AgentEngine;
using KK.Agent.Library.Clients.OpenApi;

namespace KK.Agent.Library.Examples.Agents
{
    public class ExampleAgent(OpenApiClient provider, AgentToolsProvider tools, AgentLogger logger) : AgentBase(provider, tools, logger)
    {
        protected override string SystemPrompt { get; set; } = string.Empty;
    }
}
