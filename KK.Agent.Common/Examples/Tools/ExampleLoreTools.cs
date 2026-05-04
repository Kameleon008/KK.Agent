
using KK.Agent.Library.Attributes;

namespace KK.Agent.Library.Examples.Tools
{
    public class ExampleLoreTools
    {
        [AgentTool("Search the database for character home planets.")]
        public async Task<string> search_lore(
            [ParameterDescription("name of character")] string character_name,
            [ParameterDescription("is main character?")] bool main = false)
        {
            await Task.Delay(100);

            if (main)
            {
                return $"{character_name} comes from Tatooine and is main character";
            }
            else
            {
                return $"{character_name} comes from Tatooine and is secondary character";
            }
        }

        [AgentTool("Search wiki for information about any topic.")]
        public async Task<string> search_wiki(
            [ParameterDescription("topic to search")] string query)
        {
            await Task.Delay(200);
            return $"Wiki results for: {query} - Agent AI is a program that performs tasks autonomously.";
        }
    }
}
