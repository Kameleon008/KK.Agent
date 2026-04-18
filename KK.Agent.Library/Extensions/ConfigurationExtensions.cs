using KK.Agent.Library.Configuration;
using KK.Agent.Library.Mcp;

namespace KK.Agent.Library.Extensions
{
    public static class ConfigurationExtensions
    {
        public static ConfigMcpServers AsConfigMcpServers(this List<McpServer> mcpServers)
        {
            return new ConfigMcpServers
            {
                Servers = mcpServers.Select(x => new ConfigMcpServer
                {
                    EnvironmentVariables = x.EnvironmentVariables,
                    Arguments = x.Arguments,
                    Command = x.Command,
                    Name = x.Name
                }).ToList(),
            };
        }

    }
}
