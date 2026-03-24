namespace KK.Agent.Library.Configuration.Models;

public class ConfigRoot
{
    public ConfigLogging Logging { get; set; } = new();

    public ConfigProvider Provider { get; set; } = new();
}
