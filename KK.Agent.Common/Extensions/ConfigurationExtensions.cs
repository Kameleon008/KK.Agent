using KK.Agent.Common.Configuration;
using KK.Agent.Common.Mcp;

namespace KK.Agent.Common.Extensions
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
