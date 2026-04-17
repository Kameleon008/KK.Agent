using KK.Agent.Library.AgentEngine;
using KK.Agent.Library.Agents.Tools;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Mcp;

namespace KK.Agent.Library.Agents
{
    public class OrchestratorAgent : AgentBase
    {
        protected override string SystemPrompt { get; set; } = File.ReadAllText($"./Agents/{nameof(OrchestratorAgent)}.md");

        protected override string AgentId { get; set; } = nameof(OrchestratorAgent);

        public OrchestratorAgent(OpenApiClient provider, AgentsFactory agentsFactory, AgentLogger logger, ConfigMcpServers mcp) : base(provider, logger, mcp)
        {
            this.AddToolInstance(new OrchestratorTools(logger, agentsFactory, mcp));
            this.AddMcpServer("test");
        }
    }
}
