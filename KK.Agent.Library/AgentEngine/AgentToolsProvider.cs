using KK.Agent.Library.Clients.OpenApi.V1;
using KK.Agent.Library.Mcp;

namespace KK.Agent.Library.AgentEngine
{
    public class AgentToolsProvider(ConfigMcpServers mcpServers)
    {
        public readonly List<ToolDefinition> ToolDefinitions = [];
        public readonly Dictionary<string, Func<string, Task<string>>> Tools = new();

        public readonly List<McpClient> McpClients = [];
        public readonly ConfigMcpServers McpServers = mcpServers;
    }
}
