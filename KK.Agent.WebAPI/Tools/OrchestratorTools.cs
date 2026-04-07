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
        public async Task<string> call_http_client_agent(
            [Description("description of task for agent")] string task)
        {
            var sessionId = Guid.NewGuid().ToString();

            var config = ConfigService.Get<ConfigRoot>();

            var agent = new HttpAgent(new OpenApiClient(config.Provider), logger, history);

            agent.AddMessage("User", task);

            return await agent.RunStreamAsync(sessionId);
        }

        [AgentTool("Call image agent to describe some image")]
        public async Task<string> call_image_agent(
            [Description("url of image to describe")] string url,
            [Description("description of details of image on which agent should focus")] string focus,
            [Description("description of task for agent - main task")] string task)
        {
            var config = ConfigService.Get<ConfigRoot>();

            var agent = new ImageAgent(new OpenApiClient(config.Provider), logger, history);

            var prompt =
                $"""
                IMAGE URL: {url}
                TASK TO DO: {task}
                ADDITIONAL DETAILS AGENT SHOULD FOCUS ON: {focus}         
                """;

            var sessionId = Guid.NewGuid().ToString();
            var image = await agent.FetchImageAsBase64Async(url);

            agent.AddImage("user", prompt, image, sessionId);

            return await agent.RunStreamAsync(sessionId);
        }
    }
}
