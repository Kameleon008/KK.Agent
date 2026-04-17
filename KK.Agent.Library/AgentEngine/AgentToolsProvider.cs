using KK.Agent.Library.Clients.OpenApi.V1;
using KK.Agent.Library.Mcp;

namespace KK.Agent.Library.AgentEngine
{
    public class AgentToolsProvider
    {
        protected readonly List<ToolDefinition> _toolDefinitions = [];
        protected readonly Dictionary<string, Func<string, Task<string>>> _tools = new();

        protected readonly List<McpClient> _mcpClients = [];
        protected readonly ConfigMcpServers _mcpServers;
    }
}
