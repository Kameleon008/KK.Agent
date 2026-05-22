using Microsoft.Extensions.Configuration;

namespace KK.Agent.Common.Configuration;

public static class ConfigService
{
    private static IConfiguration? _configuration;

    public static void Load(string basePath = "")
    {
        var directory = !string.IsNullOrEmpty(basePath) 
            ? basePath 
            : AppDomain.CurrentDomain.BaseDirectory;

        var env = Environment.GetEnvironmentVariable(ConfigServiceConst.EnvironmentVariables.Environment) ?? "Development";
        
        _configuration = new ConfigurationBuilder()
            .SetBasePath(directory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }

    public static IConfiguration GetConfiguration()
    {
        _configuration ??= LoadFromDefaultPath();
        return _configuration;
    }

    public static TOptions Get<TOptions>() where TOptions : class, new()
    {
        var config = GetConfiguration();
        var options = new TOptions();
        config.Bind(options);
        return options;
    }

    private static IConfiguration LoadFromDefaultPath()
    {
        Load(AppDomain.CurrentDomain.BaseDirectory);
        return _configuration!;
    }
}
