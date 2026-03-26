using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Configuration;
using KK.Agent.Library.Configuration.Models;
using KK.Agent.Library.Examples.Agents;
using KK.Agent.Library.Examples.Tools;

namespace KK.Agent.ConsoleClient;

public static class Program
{
    public static async Task Main(string[] args)
    {
        ConfigService.Load();
        var config = ConfigService.Get<ConfigRoot>();

        var agent = new ExampleAgent(
            provider: new OpenApiClient(config.Provider),
            toolsInstances: [new ExampleLoreTools(), new ExampleWeatherTools()]);

        var question1 = "Where does Luke Skywalker come from?";
        Console.WriteLine($"Question: {question1}");
        Console.WriteLine($"Response: {await agent.RunAsync(question1)}\n");
        Console.WriteLine("===============================");

        var question2 = "What is the weather in Wroclaw, Gdańsk and Warsaw?";
        Console.WriteLine($"Question: {question2}");
        Console.WriteLine($"Response: {await agent.RunAsync(question2)}\n");
        Console.WriteLine("===============================");

        var question3 = "Who are you?";
        Console.WriteLine($"Question: {question3}");
        Console.WriteLine($"Response: {await agent.RunAsync(question3)}\n");
        Console.WriteLine("===============================");

        Console.WriteLine("\n\nPress any key to exit...");
        Console.ReadKey();
    }
}