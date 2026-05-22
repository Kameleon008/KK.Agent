using System.ComponentModel;
using KK.Agent.Common.AgentEngine;
using KK.Agent.Common.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace KK.Agent.Common.Agents.Tools
{
    public class OrchestratorTools(IServiceProvider provider)
    {
        private readonly AgentsFactory _agentsFactory = provider.GetRequiredService<AgentsFactory>();

        [AgentTool("Call http client agent to execute some http call")]
        public async Task<string> call_http_client_agent(
            [Description("description of task for agent")] string task)
        {
            var chat = new ChatHistory();
            var agent = await _agentsFactory.CreateAgentAsync<HttpAgent>();

            chat.AddMessage("user", task);

            return await agent.AskAgentStream(chat);
        }

        [AgentTool("Call sensors agent to execute some operations and task related to sensors")]
        public async Task<string> call_sensors_agent(
            [Description("description of task for agent")] string task)
        {
            var chat = new ChatHistory();
            var agent = await _agentsFactory.CreateAgentAsync<SensorsAgent>();

            chat.AddMessage("user", task);

            return await agent.AskAgentStream(chat);
        }

        [AgentTool("Call image agent to describe some image")]
        public async Task<string> call_image_agent(
            [Description("url of image to describe")] string url,
            [Description("description of details of image on which agent should focus")] string focus,
            [Description("description of task for agent - main task")] string task)
        {
            var prompt =
                $"""
                 IMAGE URL: {url}
                 TASK TO DO: {task}
                 ADDITIONAL DETAILS AGENT SHOULD FOCUS ON: {focus}         
                 """;


            var chat = new ChatHistory();
            var agent = await _agentsFactory.CreateAgentAsync<ImageAgent>();

            var image = await agent.FetchImageAsBase64Async(url);
            chat.AddImage("user", prompt, image);

            return await agent.AskAgentStream(chat);
        }
    }
}
