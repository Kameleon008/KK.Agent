using KK.Agent.Common.Clients;
using KK.Agent.Common.Clients.OpenApi;
using KK.Agent.Common.Configuration;
using KK.Agent.Common.Extensions;
using KK.Agent.Common.Tools;
using Microsoft.Extensions.Options;

namespace KK.Agent.Common.AgentEngine
{
    public class AgentsFactory(AgentLogger logger, IServiceProvider provider, IOptions<ConfigAgents> config)
    {
        public async Task<T> CreateAgentAsync<T>()
            where T : AgentBase
        {
            var configuration = config.Value.Agents.FirstOrDefault(x => $"{x.Name}Agent" == typeof(T).Name);

            if (configuration == null)
            {
                throw new Exception($"No configuration found for agent type {typeof(T).Name}");
            }

            var llmProvider = this.ConfigureLlmProvider(configuration);
            var toolsProvider = await this.ConfigureToolsProvider(configuration);

            return (T)Activator.CreateInstance(typeof(T), llmProvider, toolsProvider, configuration, logger)!;

        }

        private async Task<ToolsProvider> ConfigureToolsProvider(ConfigAgent configuration)
        {
            var mcpServers = configuration.McpServers.AsConfigMcpServers();
            var toolsNames = configuration.Tools;

            var toolsProvider = new ToolsProvider(provider);

            await toolsProvider.RegisterToolsFromMcp(mcpServers);
            await toolsProvider.RegisterToolsFromNames(toolsNames);

            return toolsProvider;
        }

        private IApiProviderClient ConfigureLlmProvider(ConfigAgent configuration)
        {
            if (configuration is { OpenApi: not null })
            {
                return new OpenApiClient(configuration.OpenApi);
            }

            throw new Exception("Unprocessable LLM Provider");
        }
    }
}
