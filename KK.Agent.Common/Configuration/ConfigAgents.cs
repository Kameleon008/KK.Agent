namespace KK.Agent.Common.Configuration;

public class ConfigAgents
{
    public List<ConfigAgent> Agents { get; set; } = [];
}

public class ConfigAgent
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ProviderType { get; set; } = string.Empty;

    public string ReasoningEffort { get; set; } = "low";

    public double Temperature { get; set; } = 0.7;

    public OpenApi? OpenApi { get; set; } = new();

    public List<string> Tools { get; set; } = [];

    public List<McpServer> McpServers { get; set; } = [];
}

public class OpenApi
{
    public string Model { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    public string Endpoint { get; set; } = string.Empty;
}

public class McpServer
{
    public string Name { get; set; } = string.Empty;

    public string Command { get; set; } = string.Empty;

    public string Arguments { get; set; } = string.Empty;

    public Dictionary<string, string> EnvironmentVariables { get; set; } = new ();
}