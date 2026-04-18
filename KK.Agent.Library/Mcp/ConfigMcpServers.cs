using System.Collections.Specialized;

namespace KK.Agent.Library.Mcp
{
    public class ConfigMcpServers
    {
        public static string Name = "McpServers";

        public List<ConfigMcpServer> Servers { get; set; } = [];

        public List<McpClient> Clients => Servers.Select(server => new McpClient(server)).ToList();
    }

    public class ConfigMcpServer
    {
        public string Name { get; set; } = string.Empty;

        public string Command { get; set; } = string.Empty;

        public string Arguments { get; set; } = string.Empty;

        public Dictionary<string,string> EnvironmentVariables { get; set; } = new ();

        public string? WorkingDirectory { get; set; }
        
        public bool UseShellExecute { get; set; } = false;
        
        public bool RedirectStdErr { get; set; } = true;
    }
}
