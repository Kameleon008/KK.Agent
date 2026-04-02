using KK.Agent.Library.Agents;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.WebAPI.Tools;

namespace KK.Agent.WebAPI.Agents
{
    public class OrchestratorAgent : AgentBase
    {
        protected override string SystemPrompt { get; set; } = File.ReadAllText($"./Agents/{nameof(OrchestratorAgent)}.md");

        protected override string AgentId { get; set; } = "OrchestratorAgent";

        public OrchestratorAgent(OpenApiClient provider, AgentLogger logger) : base(provider, logger)
        {
            var orchestratorTools = new OrchestratorTools(logger);
            this.AddToolInstance(orchestratorTools);

        }
    }
}
