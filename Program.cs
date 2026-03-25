using KK.Agent.Library.Agents;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Configuration.Models;
using KK.Agent.Library.Entities.Examples;

namespace KK.Agent.Library
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Przykład użycia CognitiveAgent z narzędziami reflection-based ===\n");

            // 1. Konfiguracja API
            var config = new ConfigProvider
            {
                Model = "gpt-4o-mini",              // lub inny model np. "llama-3.1-70b-versatile"
                Endpoint = "https://api.openai.com/v1"  // endpoint Twojego API (np. LM Studio)
            };

            // 2. Utwórz klienta HTTP do komunikacji z LLM
            var apiClient = new OpenApiClient(config);

            // 3. Utwórz instancję narzędzi - klasa zawierająca metody oznaczone [AgentTool]
            var toolsInstance = new LoreDatabaseTools();

            // 4. Stwórz agenta z reflection-based toolami
            var cognitiveConfig = new CognitiveAgentConfig();
            var agent = new CognitiveAgent(cognitiveConfig, apiClient, toolsInstance);

            // 5. Wyślij pytanie do agenta - on automatycznie użyje dostępnych narzędzi
            Console.WriteLine("Pytanie: Gdzie pochodzi Luke Skywalker?");
            string response1 = await agent.RunAsync("Gdzie pochodzi Luke Skywalker?");
            Console.WriteLine($"Odpowiedź: {response1}\n");

            // 6. Spróbuj z innym pytaniem - np. o pogodę (jeśli masz takie narzędzie)
            Console.WriteLine("Pytanie: Jaka jest pogoda w Warszawie?");
            string response2 = await agent.RunAsync("Jaka jest pogoda w Warszawie?");
            Console.WriteLine($"Odpowiedź: {response2}\n");

            // 7. Sprawdź jakie narzędzia są dostępne
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
}
