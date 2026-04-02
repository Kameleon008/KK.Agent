using KK.Agent.Library;
using KK.Agent.Library.Agents;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.WebAPI.Tools;

namespace KK.Agent.WebAPI.Agents
{
    public class HttpAgent : AgentBase
    {
        protected override string SystemPrompt { get; set; } = File.ReadAllText($"./Agents/{nameof(HttpAgent)}.md");

        protected override string AgentId { get; set; } = nameof(HttpAgent);

        public HttpAgent(OpenApiClient provider, AgentLogger logger, AgentHistory history) : base(provider, logger, history)
        {
            this.AddToolInstance(new HttpClientTools());
            this.AddToolInstance(new WaitingTools());
        }
    }
}
