using KK.Agent.Library.Agents;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Configuration.Models;
using KK.Agent.Library.Entities.Examples;

namespace KK.Agent.Library.Examples
{
    public class ToolUsageExample
    {
        /// <summary>
        /// Przykład użycia CognitiveAgent z narzędziami opartymi na atrybutach reflection
        /// </summary>
        public static async Task ExampleWithReflectionTools()
        {
            // 1. Konfiguracja
            var config = new ConfigProvider
            {
                Model = "gpt-4o-mini",
                Endpoint = "https://api.openai.com/v1"
            };

            // 2. Utwórz klienta API
            var apiClient = new OpenApiClient(config);

            // 3. Utwórz instancję narzędzi - klasa z atrybutami [AgentTool] i [ParameterDescription]
            var toolsInstance = new LoreDatabaseTools();

            // 4. Utwórz agenta z narzędziami reflection-based
            var cognitiveConfig = new CognitiveAgentConfig();
            var agent = new CognitiveAgent(cognitiveConfig, apiClient, toolsInstance);

            // 5. Wyślij prompt do agenta
            string response = await agent.RunAsync("Gdzie pochodzi Luke Skywalker?");

            Console.WriteLine($"Odpowiedź: {response}");
        }

        /// <summary>
        /// Przykład użycia CognitiveAgent z tradycyjnymi narzędziami słownikowymi (legacy)
        /// </summary>
        public static async Task ExampleWithDictionaryTools()
        {
            // 1. Konfiguracja
            var config = new ConfigProvider
            {
                Model = "gpt-4o-mini",
                Endpoint = "https://api.openai.com/v1"
            };

            // 2. Utwórz klienta API
            var apiClient = new OpenApiClient(config);

            // 3. Utwórz agenta z tradycyjnymi narzędziami słownikowymi
            var cognitiveConfig = new CognitiveAgentConfig();
            var agent = new CognitiveAgent(cognitiveConfig, apiClient);

            // 4. Wyślij prompt do agenta
            string response = await agent.RunAsync("Jaka jest pogoda we Wrocławiu?");

            Console.WriteLine($"Odpowiedź: {response}");
        }

        /// <summary>
        /// Przykład pokazujący jak wygląda wygenerowany payload dla narzędzia search_lore
        /// </summary>
        public static void ShowGeneratedToolDefinition()
        {
            var toolsInstance = new LoreDatabaseTools();
            
            // Wygeneruj definicje narzędzi
            var toolDefinitions = KK.Agent.Library.Entities.ToolDefinitionGenerator.GenerateFromObject(toolsInstance);

            // Serializuj do JSON dla zobaczenia
            foreach (var tool in toolDefinitions)
            {
                Console.WriteLine($"Tool: {tool.Function.Name}");
                Console.WriteLine($"Description: {tool.Function.Description}");
                Console.WriteLine("Parameters:");
                
                if (tool.Function.Parameters?.Properties != null)
                {
                    foreach (var prop in tool.Function.Parameters.Properties)
                    {
                        var required = tool.Function.Parameters.Required?.Contains(prop.Key) == true ? " [REQUIRED]" : "";
                        Console.WriteLine($"  - {prop.Key}: {prop.Value.Type}{required}");
                        if (!string.IsNullOrEmpty(prop.Value.Description))
                        {
                            Console.WriteLine($"    Description: {prop.Value.Description}");
                        }
                    }
                }

                Console.WriteLine($"Strict: {tool.Function.Strict}\n");
            }
        }
    }
}
