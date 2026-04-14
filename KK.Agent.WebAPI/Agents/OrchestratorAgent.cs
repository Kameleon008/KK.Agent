using KK.Agent.Library;
using KK.Agent.Library.Agents;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Mcp;
using KK.Agent.WebAPI.Tools;

namespace KK.Agent.WebAPI.Agents
{
    public class OrchestratorAgent : AgentBase
    {
        protected override string SystemPrompt { get; set; } = File.ReadAllText($"./Agents/{nameof(OrchestratorAgent)}.md");

        protected override string AgentId { get; set; } = nameof(OrchestratorAgent);

        public OrchestratorAgent(OpenApiClient provider, AgentLogger logger, AgentHistory history, ConfigMcpServers mcp) : base(provider, logger, history, mcp)
        {
            this.AddToolInstance(new OrchestratorTools(logger, history, mcp));
            this.AddMcpServer("test");
        }
    }
}
