using System.ComponentModel;
using KK.Agent.Library;
using KK.Agent.Library.Agents;
using KK.Agent.Library.Attributes;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Configuration;
using KK.Agent.Library.Configuration.Models;
using KK.Agent.Library.Mcp;
using KK.Agent.WebAPI.Agents;

namespace KK.Agent.WebAPI.Tools
{
    public class OrchestratorTools(AgentLogger logger, AgentsFactory agentsFactory, ConfigMcpServers mcp)
    {
        [AgentTool("Call http client agent to execute some http call")]
        public async Task<string> call_http_client_agent(
            [Description("description of task for agent")] string task)
        {
            var chat = new ChatHistory();
            var agent = await agentsFactory.CreateAgentAsync<HttpAgent>();

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
            var agent = await agentsFactory.CreateAgentAsync<ImageAgent>();

            var image = await agent.FetchImageAsBase64Async(url);
            chat.AddImage("user", prompt, image);

            return await agent.AskAgentStream(chat);
        }
    }
}
