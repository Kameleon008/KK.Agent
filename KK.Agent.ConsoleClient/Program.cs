using KK.Agent.Library.Clients;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Configuration;
using KK.Agent.Library.Configuration.Models;

namespace KK.Agent.ConsoleClient;

public static class Program
{
    public static async Task Main(string[] args)
    {
        ConfigService.Load();
        
        var config = ConfigService.Get<ConfigRoot>();

        var openApiClient = new OpenApiClient(config.Provider);

        var result = await openApiClient.GetModelsAsync();


        foreach (var models in result.Data)
        {
            Console.WriteLine(models.Id);
        }

        await Task.Delay(100);
    }
}