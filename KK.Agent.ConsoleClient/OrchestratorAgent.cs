using KK.Agent.Library.Agents;
using KK.Agent.Library.Clients.OpenApi;

namespace KK.Agent.ConsoleClient
{
    public class OrchestratorAgent(OpenApiClient provider) : AgentBase(provider)
    {
        protected override string SystemPrompt { get; set; } = string.Empty;
    }
}
