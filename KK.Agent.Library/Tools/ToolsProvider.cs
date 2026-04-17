using KK.Agent.Library.Clients.OpenApi.V1;
using KK.Agent.Library.Extensions;
using KK.Agent.Library.Mcp;

namespace KK.Agent.Library.Tools
{
    public class ToolsProvider()
    {
        public readonly List<ToolDefinition> ToolDefinitions = [];
        public readonly Dictionary<string, Func<string, Task<string>>> Tools = new();

        public readonly List<McpClient> McpClients = [];

        public void AddToolFromType<T>() where T : class, new()
        {
            var instance = new T();
            RegisterTools(instance);
        }

        public void AddToolInstance(object toolInstance)
        {
            RegisterTools(toolInstance);
        }

        private void RegisterTools(object instance)
        {
            var toolDefinitions = ToolDefinitionGenerator.GenerateFromObject(instance);
            this.ToolDefinitions.AddRange(toolDefinitions);

            var tools = ToolGenerator.GenerateFromObject(instance);
            this.Tools.AddRange(tools);
        }

        public void RegisterToolsFromNames(List<string> toolNames)
        {
            var assembly = typeof(ToolsProvider).Assembly;

            foreach (var name in toolNames)
            {
                var type = assembly.GetType(name) ?? assembly.GetTypes().FirstOrDefault(t => t.Name == name);

                if (type == null) continue;

                var instance = Activator.CreateInstance(type);

                if (instance == null) continue;

                RegisterTools(instance);
            }
        }

        public async Task RegisterMcpTools(ConfigMcpServers mcpServers)
        {
            McpClients.AddRange(mcpServers.Clients);
            foreach (var client in McpClients)
            {
                await client.LoadToolsAsync(ToolDefinitions);
            }
        }

    }
}
