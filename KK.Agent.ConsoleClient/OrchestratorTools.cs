using KK.Agent.Library.Attributes;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Configuration;
using KK.Agent.Library.Configuration.Models;
using KK.Agent.Library.Examples.Tools;
using System.ComponentModel;

namespace KK.Agent.ConsoleClient
{
    public class OrchestratorTools
    {
        [AgentTool("Calls_lore_agent_to_execute_task")]
        public async Task<string> call_lore_agent([Description("descriptikon of task for agent")] string task)
        {
            var config = ConfigService.Get<ConfigRoot>();

            var loreAgent = new LoreAgent(new OpenApiClient(config.Provider));
            loreAgent.AddToolFromType<ExampleLoreTools>();
            loreAgent.AddToolFromType<ExamplePlanetaryDatabase>();

            return await loreAgent.RunAsync(task);
        }
    }
}
