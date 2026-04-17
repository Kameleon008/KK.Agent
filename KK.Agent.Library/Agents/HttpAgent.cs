using KK.Agent.Library.Agents;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Mcp;
using KK.Agent.WebAPI.Tools;

namespace KK.Agent.WebAPI.Agents
{
    public class HttpAgent : AgentBase
    {
        protected override string SystemPrompt { get; set; } = File.ReadAllText($"./Agents/{nameof(HttpAgent)}.md");

        protected override string AgentId { get; set; } = nameof(HttpAgent);

        public HttpAgent(OpenApiClient provider, AgentLogger logger, ConfigMcpServers mcp) : base(provider, logger, mcp)
        {
            this.AddToolInstance(new HttpClientTools());
            this.AddToolInstance(new WaitingTools());
        }
    }
}
