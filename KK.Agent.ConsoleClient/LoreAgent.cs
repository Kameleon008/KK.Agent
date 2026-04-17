using KK.Agent.Library.AgentEngine;
using KK.Agent.Library.Clients.OpenApi;

namespace KK.Agent.ConsoleClient
{
    public class LoreAgent(OpenApiClient provider) : AgentBase(provider)
    {
        protected override string SystemPrompt { get; set; } = string.Empty;
    }
}
