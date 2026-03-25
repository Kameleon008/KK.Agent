using KK.Agent.Library.Agents;
using KK.Agent.Library.Clients.OpenApi;

namespace KK.Agent.ConsoleClient.Agents
{
    internal class ExampleAgent : CognitiveAgentBase
    {
        public ExampleAgent(CognitiveAgentConfig configuration, OpenApiClient provider) : base(configuration, provider)
        {
        }

        public ExampleAgent(CognitiveAgentConfig configuration, OpenApiClient provider, object toolsInstance) : base(configuration, provider, toolsInstance)
        {
        }
    }
}
