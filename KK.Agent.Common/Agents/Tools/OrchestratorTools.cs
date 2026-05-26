using System.ComponentModel;
using KK.Agent.Common.AgentEngine;
using KK.Agent.Common.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace KK.Agent.Common.Agents.Tools
{
    public class OrchestratorTools(IServiceProvider provider)
    {
        private readonly AgentsFactory _agentsFactory = provider.GetRequiredService<AgentsFactory>();

        [AgentTool("Orchestrate task to Agent responsible for execute http calls")]
        public async Task<string> orchestrate_task_to_agent_network_operator(
            [Description("description of task for agent")] string task)
        {
            var chat = new ChatHistory();
            var agent = await _agentsFactory.CreateAgentAsync<HttpAgent>();

            chat.AddMessage("user", task);

            return await agent.AskAgentStream(chat);
        }

        [AgentTool("Orchestrate task to Agent responsible for some operations and task related to sensors")]
        public async Task<string> orchestrate_task_to_sensors_operator(
            [Description("description of task for agent")] string task)
        {
            var chat = new ChatHistory();
            var agent = await _agentsFactory.CreateAgentAsync<SensorsAgent>();

            chat.AddMessage("user", task);

            return await agent.AskAgentStream(chat);
        }

        [AgentTool("Orchestrate task to Agent responsible for Image management")]
        public async Task<string> orchestrate_task_to_image_operator(
            [Description("description of task for agent")] string task)
        {
            var chat = new ChatHistory();
            var agent = await _agentsFactory.CreateAgentAsync<ImageAgent>();

            chat.AddMessage("user", task);

            return await agent.AskAgentStream(chat);
        }
    }
}
