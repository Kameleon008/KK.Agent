using KK.Agent.Library.Agents;
using KK.Agent.Library.Clients.OpenApi;

namespace KK.Agent.Library.Examples.Agents
{
    public class ExampleAgent(OpenApiClient provider, AgentLogger logger) : AgentBase(provider, logger)
    {
        protected override string SystemPrompt { get; set; } = string.Empty;
    }
}
