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

        var orchestratorAgent = new OrchestratorAgent(new OpenApiClient(config.Provider));
        orchestratorAgent.AddToolFromType<OrchestratorTools>();


        //var question1 = "Where does Luke Skywalker come from? Tell me about his home planet";
        //Console.WriteLine($"Question: {question1}");
        //Console.WriteLine($"Response: \n\n{await agent.RunAsync<CharacterWithPlanetInfo>(question1)}\n");
        //Console.WriteLine("-------------------------------");

        var question3 = "Where does Luke Skywalker come from? Tell me about his home planet";
        Console.WriteLine($"Question: {question3}");
        Console.Write("Response: "); // Używamy Write, żeby tekst pojawiał się w tej samej linii

        // Używamy await foreach do konsumowania strumienia
        await foreach (var chunk in orchestratorAgent.RunStreamAsync(question3))
        {
            // Wypisujemy każdy kawałek natychmiast bez nowej linii
            Console.Write(chunk);
        }

        Console.WriteLine("\n-------------------------------");

        //var question4 = "Who are you?";
        //Console.WriteLine($"Question: {question4}");
        //Console.WriteLine($"Response: \n\n{await agent.RunAsync(question4)}\n");
        //Console.WriteLine("-------------------------------");

        Console.WriteLine("\n\nPress any key to exit...");
        Console.ReadKey();
    }
}