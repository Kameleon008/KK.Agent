using KK.Agent.Library.Agents;
using KK.Agent.Library.Clients.OpenApi;

namespace KK.Agent.Library.Examples.Agents
{
    public class ExampleAgent : AgentBase
    {
        protected override string SystemPrompt { get; set; } = "You are Pirate! Talk like a pirate!";

        public ExampleAgent(OpenApiClient provider) : base(provider)
        {
        }

        public ExampleAgent(OpenApiClient provider, params object[] toolsInstances) : base(provider, toolsInstances)
        {
        }
    }
}
