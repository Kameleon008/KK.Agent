using System.ComponentModel;
using KK.Agent.Library;
using KK.Agent.Library.Agents;
using KK.Agent.Library.Attributes;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Configuration;
using KK.Agent.Library.Configuration.Models;
using KK.Agent.WebAPI.Agents;

namespace KK.Agent.WebAPI.Tools
{
    public class OrchestratorTools(AgentLogger logger, AgentHistory history)
    {
        [AgentTool("Call http client agent to execute some http call")]
        public async Task<string> call_lore_agent(
            [Description("description of task for agent")] string task)
        {
            var config = ConfigService.Get<ConfigRoot>();

            var loreAgent = new HttpAgent(new OpenApiClient(config.Provider), logger, history);

            return await loreAgent.RunStreamAsync(task);
        }
    }
}
