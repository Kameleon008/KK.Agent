using KK.Agent.Library.Agents;
using KK.Agent.Library.Clients.OpenApi;

namespace KK.Agent.WebAPI.Agents
{
    public class LoreAgent(OpenApiClient provider, AgentLogger logger) : AgentBase(provider, logger)
    {
        protected override string SystemPrompt { get; set; } = File.ReadAllText($"./Agents/{nameof(LoreAgent)}.md");

        protected override string AgentId { get; set; } = "LoreAgent";
    }
}
