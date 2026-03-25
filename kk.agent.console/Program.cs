using KK.Agent.Library.Agents;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Configuration.Models;
using KK.Agent.Library.Entities.Examples;

namespace kk.agent.console
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Example usage of CognitiveAgent with reflection-based tools ===\n");

            // 1. API Configuration
            var config = new ConfigProvider
            {
                Model = "gpt-4o-mini",              // or another model like "llama-3.1-70b-versatile"
                Endpoint = "https://api.openai.com/v1"  // your API endpoint (e.g., LM Studio)
            };

            // 2. Create HTTP client for communication with LLM
            var apiClient = new OpenApiClient(config);

            // 3. Create tools instance - class containing methods marked with [AgentTool]
            var toolsInstance = new LoreDatabaseTools();

            // 4. Create agent with reflection-based tools
            var cognitiveConfig = new CognitiveAgentConfig();
            var agent = new CognitiveAgent(cognitiveConfig, apiClient, toolsInstance);

            // 5. Send question to the agent - it will automatically use available tools
            Console.WriteLine("Question: Where does Luke Skywalker come from?");
            string response1 = await agent.RunAsync("Where does Luke Skywalker come from?");
            Console.WriteLine($"Answer: {response1}\n");

            // 6. Check what tools are available
            var toolDefinitions = KK.Agent.Library.Entities.ToolDefinitionGenerator.GenerateFromObject(toolsInstance);
            Console.WriteLine("\nAvailable tools:");
            foreach (var tool in toolDefinitions)
            {
                Console.WriteLine($"  - {tool.Function.Name}: {tool.Function.Description}");
            }

            Console.WriteLine("\n\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
