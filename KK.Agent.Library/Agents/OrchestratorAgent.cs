using KK.Agent.Library.AgentEngine;
using KK.Agent.Library.Agents.Tools;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Mcp;
using KK.Agent.Library.Tools;

namespace KK.Agent.Library.Agents
{
    public class OrchestratorAgent : AgentBase
    {
        protected override string SystemPrompt { get; set; } = File.ReadAllText($"./Agents/{nameof(OrchestratorAgent)}.md");

        protected override string AgentId { get; set; } = nameof(OrchestratorAgent);

        public OrchestratorAgent(AgentsFactory agentsFactory, OpenApiClient client, ToolsProvider tools, AgentLogger logger) : base(client, tools, logger)
        {
            //this.AddToolInstance(new OrchestratorTools(agentsFactory));
        }
    }
}
