using System.ComponentModel;
using KK.Agent.Library.Tools.Attributes;

namespace KK.Agent.ConsoleClient.Tools
{
    public class ExampleTool
    {
        [AgentTool]
        [Description("Search the database for character home planets.")]
        public async Task<string> search_lore(
            [Description("name of character")] string character_name,
            [Description("is main character?")] bool main = false)
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
    }
}
