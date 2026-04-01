using System.ComponentModel;
using KK.Agent.Library.Agents;
using KK.Agent.Library.Attributes;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Configuration;
using KK.Agent.Library.Configuration.Models;
using KK.Agent.Library.Examples.Tools;
using KK.Agent.WebAPI.Agents;

namespace KK.Agent.WebAPI.Tools
{
    public class OrchestratorTools(IServiceProvider serviceProvider)
    {
        private IServiceProvider serviceProvider = serviceProvider;

        [AgentTool("Calls_lore_agent_to_execute_task")]
        public async Task<string> call_lore_agent([Description("descriptikon of task for agent")] string task)
        {
            var config = ConfigService.Get<ConfigRoot>();
            var logger = serviceProvider.GetRequiredService<AgentLogger>();

            var loreAgent = new LoreAgent(new OpenApiClient(config.Provider), logger);
            loreAgent.AddToolFromType<ExampleLoreTools>();
            loreAgent.AddToolFromType<ExamplePlanetaryDatabase>();

            return await loreAgent.RunWithLoggerAsync(task);
        }
    }
}
