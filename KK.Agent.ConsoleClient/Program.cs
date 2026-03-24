using KK.Agent.Library.Configuration;
using KK.Agent.Library.Configuration.Models;

namespace KK.Agent.ConsoleClient;

public static class Program
{
    public static async Task Main(string[] args)
    {
        ConfigService.Load();
        
        var config = ConfigService.Get<ConfigRoot>();
        
        Console.WriteLine($"Agent: {config.Agent.Name}");
        Console.WriteLine($"Polling Interval: {config.Agent.PollingInterval}ms");
        Console.WriteLine($"Enabled: {config.Agent.Enabled}");

        await Task.Delay(100);
    }
}