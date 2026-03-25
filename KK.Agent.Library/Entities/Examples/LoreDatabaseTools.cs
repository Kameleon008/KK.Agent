using KK.Agent.Library.Clients.OpenApi.V1;

namespace KK.Agent.Library.Entities.Examples
{
    public class LoreDatabaseTools
    {
        [AgentTool("Search the database for character home planets.")]
        public async Task<string> search_lore(
            [ParameterDescription("name of character")] string character_name,
            [ParameterDescription("is main character?")] bool main = false)
        {
            await Task.Delay(100);

            if (main)
            {
                return $"{character_name} comes from Mars and is main character";
            }
            else
            {
                return $"{character_name} comes from Mars and is secondary character";
            }
        }

        [AgentTool("Get weather information for a specific location.")]
        public string get_weather(string city)
        {
            return $"{city}, 15°C, sunny";
        }

        [AgentTool("Search wiki for information about any topic.")]
        public async Task<string> search_wiki([ParameterDescription("topic to search")] string query)
        {
            await Task.Delay(200);
            return $"Wiki results for: {query} - Agent AI is a program that performs tasks autonomously.";
        }

        [AgentTool("Calculate the sum of two numbers.")]
        public double add_numbers([ParameterDescription("first number")] int a, 
                                  [ParameterDescription("second number")] int b)
        {
            return a + b;
        }
    }
}
