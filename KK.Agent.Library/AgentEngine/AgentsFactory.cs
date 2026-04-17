using KK.Agent.Library.Agents;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Mcp;

namespace KK.Agent.Library.AgentEngine
{
    public class AgentsFactory(OpenApiClient llmProvider, AgentLogger logger, ConfigMcpServers mcpServers)
    {
        public async Task<T> CreateAgentAsync<T>()
            where T : AgentBase
        {
            await Task.Delay(100);

            Console.WriteLine($"Typ T: {typeof(T).FullName}");
            Console.WriteLine($"Typ oczekiwany: {typeof(OrchestratorAgent).FullName}");

            if (typeof(OrchestratorAgent).IsAssignableFrom(typeof(T)))
            {
                return (T)Activator.CreateInstance(typeof(T), llmProvider, this, logger, mcpServers)!;
            }

            Console.WriteLine($"Assembly T: {typeof(T).Assembly.Location}");
            Console.WriteLine($"Assembly Oczekiwanego: {typeof(OrchestratorAgent).Assembly.Location}");

            return (T)Activator.CreateInstance(typeof(T), llmProvider, logger, mcpServers)!;
        }
    }
}
