using KK.Agent.Library.Agents;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Configuration;
using KK.Agent.Library.Configuration.Models;
using KK.Agent.Library.Entities.Examples;

namespace KK.Agent.ConsoleClient;

public static class Program
{
    public static async Task Main(string[] args)
    {
        // 1. Konfiguracja API
        ConfigService.Load();
        var config = ConfigService.Get<ConfigRoot>();

        Console.WriteLine("=== Przykład użycia CognitiveAgent z narzędziami reflection-based ===\n");
        
        // 2. Utwórz klienta HTTP do komunikacji z LLM
        var apiClient = new OpenApiClient(config.Provider);
        
        // 3. Utwórz instancję narzędzi - klasa zawierająca metody oznaczone [AgentTool]
        var toolsInstance = new LoreDatabaseTools();
        
        // 4. Stwórz agenta z reflection-based toolami
        var cognitiveConfig = new CognitiveAgentConfig();
        var agent = new CognitiveAgent(cognitiveConfig, apiClient, toolsInstance);
        
        // 5. Wyślij pytanie do agenta - on automatycznie użyje dostępnych narzędzi
        Console.WriteLine("Pytanie: Skąd pochodzi Luke Skywalker?");
        string response1 = await agent.RunAsync("Skąd pochodzi Luke Skywalker?");
        Console.WriteLine($"Odpowiedź: {response1}\n");

        Console.WriteLine("Pytanie: Jaka pogoda wariacie we Wrocku, jaka w Gdańsku a jaka w Warszawie?");
        string response2 = await agent.RunAsync("Jaka pogoda wariacie we Wrocku, jaka w Gdańsku a jaka w Warszawie?");
        Console.WriteLine($"Odpowiedź: {response2}\n");


        // 6. Sprawdź jakie narzędzia są dostępne
        var toolDefinitions = KK.Agent.Library.Entities.ToolDefinitionGenerator.GenerateFromObject(toolsInstance);
        Console.WriteLine("\nDostępne narzędzia:");
        foreach (var tool in toolDefinitions)
        {
            Console.WriteLine($"  - {tool.Function.Name}: {tool.Function.Description}");
        }
        Console.WriteLine("\n\nNaciśnij dowolny klawisz, aby zakończyć...");
        Console.ReadKey();
    }
}