using KK.Agent.Library.AgentEngine;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Tools;

namespace KK.Agent.Library.Agents
{
    public class SensorsAgent(OpenApiClient client, ToolsProvider tools, AgentLogger logger)
        : AgentBase(client, tools, logger)
    {
        protected override string SystemPrompt { get; set; } = File.ReadAllText($"./Agents/{nameof(SensorsAgent)}.md");

        protected override string AgentId { get; set; } = nameof(SensorsAgent);
    }
}
