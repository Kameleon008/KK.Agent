using System.Text.Json.Serialization;

namespace KK.Agent.Library.Configuration;

public class ConfigAgents
{
    public List<Agent> Agents { get; set; } = [];
}

public class Agent
{
    public string Name { get; set; } = string.Empty;

    public Provider Provider { get; set; } = new();

    public List<string> Tools { get; set; } = [];

    public List<McpServer> McpServers { get; set; } = [];
}

public class Provider
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
}