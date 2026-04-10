namespace KK.Agent.Library.Mcp
{
    public class ConfigMcpServers
    {
        public static string Name = "McpServers";

        public List<ConfigMcpServer> Servers { get; set; } = [];
    }

    public class ConfigMcpServer
    {
        public string Command { get; set; } = "";
        public string Arguments { get; set; } = "";
        public string? WorkingDirectory { get; set; }
        public bool UseShellExecute { get; set; } = false;
        public bool RedirectStdErr { get; set; } = true;
    }
}
