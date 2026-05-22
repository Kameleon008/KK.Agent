using KK.Agent.Common.AgentEngine;
using KK.Agent.Common.Clients.OpenApi;
using KK.Agent.Common.Configuration;
using KK.Agent.Common.Tools;

namespace KK.Agent.Common.Agents
{
    public class HttpAgent(OpenApiClient client, ToolsProvider tools, ConfigAgent configuration, AgentLogger logger)
        : AgentBase(client, tools, configuration, logger)
    {
        protected override string SystemPrompt { get; set; } = File.ReadAllText($"./Agents/{nameof(HttpAgent)}.md");

        protected override string AgentId { get; set; } = nameof(HttpAgent);
    }
}
