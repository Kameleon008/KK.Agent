namespace KK.Agent.Library.Mcp
{
    public class ConfigMcpServers
    {
        public static string Name = "McpServers";

        public List<ConfigMcpServer> Servers { get; set; } = [];
    }

    public class ConfigMcpServer
    {
        public string Name { get; set; } = string.Empty;

        public string Command { get; set; } = string.Empty;

        public string Arguments { get; set; } = string.Empty;
        
        public string? WorkingDirectory { get; set; }
        
        public bool UseShellExecute { get; set; } = false;
        
        public bool RedirectStdErr { get; set; } = true;
    }
}
