using KK.Agent.Library.Agents;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Configuration.Models;
using KK.Agent.Library.Entities.Examples;

namespace KK.Agent.Library.Examples
{
    public class ToolUsageExample
    {
        /// <summary>
        /// Example usage of CognitiveAgent with reflection-based attribute tools
        /// </summary>
        public static async Task ExampleWithReflectionTools()
        {
            // 1. Configuration
            var config = new ConfigProvider
            {
                Model = "gpt-4o-mini",
                Endpoint = "https://api.openai.com/v1"
            };

            // 2. Create API client
            var apiClient = new OpenApiClient(config);

            // 3. Create tools instance - class with [AgentTool] and [ParameterDescription] attributes
            var toolsInstance = new LoreDatabaseTools();

            // 4. Create agent with reflection-based tools
            var cognitiveConfig = new CognitiveAgentConfig();
            var agent = new CognitiveAgent(cognitiveConfig, apiClient, toolsInstance);

            // 5. Send prompt to the agent
            string response = await agent.RunAsync("Where does Luke Skywalker come from?");

            Console.WriteLine($"Answer: {response}");
        }

        /// <summary>
        /// Example usage of CognitiveAgent with traditional dictionary-based tools (legacy)
        /// </summary>
        public static async Task ExampleWithDictionaryTools()
        {
            // 1. Configuration
            var config = new ConfigProvider
            {
                Model = "gpt-4o-mini",
                Endpoint = "https://api.openai.com/v1"
            };

            // 2. Create API client
            var apiClient = new OpenApiClient(config);

            // 3. Create agent with traditional dictionary-based tools
            var cognitiveConfig = new CognitiveAgentConfig();
            var agent = new CognitiveAgent(cognitiveConfig, apiClient);

            // 4. Send prompt to the agent
            string response = await agent.RunAsync("What is the weather in Wrocław?");

            Console.WriteLine($"Answer: {response}");
        }

        /// <summary>
        /// Example showing what the generated payload looks like for search_lore tool
        /// </summary>
        public static void ShowGeneratedToolDefinition()
        {
            var toolsInstance = new LoreDatabaseTools();
            
            // Generate tool definitions
            var toolDefinitions = KK.Agent.Library.Entities.ToolDefinitionGenerator.GenerateFromObject(toolsInstance);

            // Serialize to JSON for viewing
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
