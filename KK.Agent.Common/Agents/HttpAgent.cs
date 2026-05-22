using KK.Agent.Library.AgentEngine;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Configuration;
using KK.Agent.Library.Tools;

namespace KK.Agent.Library.Agents
{
    public class HttpAgent(OpenApiClient client, ToolsProvider tools, ConfigAgent configuration, AgentLogger logger)
        : AgentBase(client, tools, configuration, logger)
    {
        protected override string SystemPrompt { get; set; } = File.ReadAllText($"./Agents/{nameof(HttpAgent)}.md");

        protected override string AgentId { get; set; } = nameof(HttpAgent);
    }
}
