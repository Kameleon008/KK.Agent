using KK.Agent.Library.Agents;
using KK.Agent.Library.Clients.OpenApi;

namespace KK.Agent.ConsoleClient.Agents
{
    internal class ExampleAgent : CognitiveAgentBase
    {
        public ExampleAgent(OpenApiClient provider) : base(provider)
        {
        }

        public ExampleAgent(OpenApiClient provider, object toolsInstance) : base(provider, toolsInstance)
        {
        }
    }
}
