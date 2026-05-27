using KK.Agent.Common.Agents;
using KK.Agent.Common.Clients;
using KK.Agent.Common.Clients.OpenApi;
using KK.Agent.Common.Configuration;
using KK.Agent.Common.Extensions;
using KK.Agent.Common.Tools;
using Microsoft.Extensions.Options;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace KK.Agent.Common.AgentEngine
{
    public class AgentsFactory(AgentLogger logger, IServiceProvider provider, IOptions<ConfigAgents> config)
    {
        public async Task<CustomAgent> CreateAgentAsync(string name)
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "Agents", $"{name}.md");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"No prompt/configuration file found for agent: {name} at {filePath}");
            }

            var fileContent = await File.ReadAllTextAsync(filePath);
            var configuration = ParseFrontMatter<ConfigAgent>(fileContent);
            var prompt = ExtractMarkdownBody(fileContent);

            var llmProvider = ConfigureLlmProvider(configuration);
            var toolsProvider = await this.ConfigureToolsProvider(configuration);

            return new CustomAgent(name, prompt, llmProvider, toolsProvider, configuration, logger);

        }

        public async Task<T> CreateAgentAsync<T>()
            where T : AgentBase
        {
            var configuration = config.Value.Agents.FirstOrDefault(x => $"{x.Name}Agent" == typeof(T).Name);

            if (configuration == null)
            {
                throw new Exception($"No configuration found for agent type {typeof(T).Name}");
            }

            var llmProvider = ConfigureLlmProvider(configuration);
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

        private static IApiProviderClient ConfigureLlmProvider(ConfigAgent configuration)
        {
            return configuration is { OpenApi: not null } 
                ? new OpenApiClient(configuration.OpenApi) 
                : throw new Exception("Unprocessable LLM Provider");
        }

        private static TConfig ParseFrontMatter<TConfig>(string fileContent)
        {
            var parts = fileContent.Split(["---"], StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                throw new Exception("Invalid Markdown format. Missing Front Matter data block.");
            }

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance) 
                .IgnoreUnmatchedProperties()
                .Build();

            var result =  deserializer.Deserialize<TConfig>(parts[0]);

            return result;
        }

        private static string ExtractMarkdownBody(string fileContent)
        {
            var parts = fileContent.Split(["---"], StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 1 ? parts[1].Trim() : fileContent.Trim();
        }
    }
}
