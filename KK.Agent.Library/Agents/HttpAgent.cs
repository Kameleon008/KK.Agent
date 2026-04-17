using KK.Agent.Library.AgentEngine;
using KK.Agent.Library.Agents.Tools;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Tools;

namespace KK.Agent.Library.Agents
{
    public class HttpAgent : AgentBase
    {
        protected override string SystemPrompt { get; set; } = File.ReadAllText($"./Agents/{nameof(HttpAgent)}.md");

        protected override string AgentId { get; set; } = nameof(HttpAgent);

        public HttpAgent(OpenApiClient client, ToolsProvider tools, AgentLogger logger) : base(client, tools, logger)
        {
            //this.AddToolInstance(new HttpClientTools());
            //this.AddToolInstance(new WaitingTools());
        }
    }
}
