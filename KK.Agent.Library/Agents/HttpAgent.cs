using KK.Agent.Library.AgentEngine;
using KK.Agent.Library.Agents.Tools;
using KK.Agent.Library.Clients.OpenApi;

namespace KK.Agent.Library.Agents
{
    public class HttpAgent : AgentBase
    {
        protected override string SystemPrompt { get; set; } = File.ReadAllText($"./Agents/{nameof(HttpAgent)}.md");

        protected override string AgentId { get; set; } = nameof(HttpAgent);

        public HttpAgent(OpenApiClient provider, AgentToolsProvider tools, AgentLogger logger) : base(provider, tools, logger)
        {
            this.AddToolInstance(new HttpClientTools());
            this.AddToolInstance(new WaitingTools());
        }
    }
}
