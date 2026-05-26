using KK.Agent.Common.AgentEngine;
using KK.Agent.Common.Clients;
using KK.Agent.Common.Configuration;
using KK.Agent.Common.Tools;

namespace KK.Agent.Common.Agents
{
    public class CustomAgent(string name, string prompt, IApiProviderClient client, ToolsProvider tools, ConfigAgent configuration, AgentLogger logger)
        : AgentBase(client, tools, configuration, logger)
    {
        protected override string SystemPrompt { get; set; } = prompt;

        protected override string AgentId { get; set; } = name;
    }
}
