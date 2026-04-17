using KK.Agent.Library.Agents;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Configuration;
using KK.Agent.Library.Mcp;
using Microsoft.Extensions.Options;

namespace KK.Agent.Library.AgentEngine
{
    public class AgentsFactory(AgentLogger logger, IOptions<ConfigAgents> config)
    {
        public async Task<T> CreateAgentAsync<T>()
            where T : AgentBase
        {
            await Task.Delay(100);

            var configuration = config.Value.Agents.FirstOrDefault(x => $"{x.Name}Agent" == typeof(T).Name);

            if (configuration == null)
            {
                throw new Exception($"No configuration found for agent type {typeof(T).Name}");
            }

            var llmProvider = this.ConfigureLlmProvider(configuration);
            var toolsProvider = this.ConfigureToolsProvider(configuration);

            if (typeof(OrchestratorAgent).IsAssignableFrom(typeof(T)))
            {
                return (T)Activator.CreateInstance(typeof(T), this, llmProvider, toolsProvider, logger)!;
            }

            return (T)Activator.CreateInstance(typeof(T), llmProvider, toolsProvider, logger)!;

        }

        private AgentToolsProvider ConfigureToolsProvider(ConfigAgent configuration)
        {
            var mcp = new ConfigMcpServers
            {
                Servers = configuration.McpServers.Select(x => new ConfigMcpServer()
                {
                    Arguments = x.Arguments,
                    Command = x.Command,
                    Name = x.Name
                }).ToList(),
            };

            return new AgentToolsProvider(mcp);
        }

        private OpenApiClient ConfigureLlmProvider(ConfigAgent configuration)
        {
            return new OpenApiClient(configuration.OpenAPI);
        }
    }
}
