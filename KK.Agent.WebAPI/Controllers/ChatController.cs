using KK.Agent.Library;
using KK.Agent.Library.Agents;
using KK.Agent.WebAPI.Agents;
using Microsoft.AspNetCore.Mvc;

namespace KK.Agent.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        [HttpPost]
        public async Task<string> Chat([FromBody] ChatRequest request, ChatHistoryProvider chatProvider, AgentsFactory factory, AgentLogger logger)
        {
            var agent = await factory.CreateAgentAsync<OrchestratorAgent>();

            var chat = chatProvider.GetChatHistory(request.SessionId);
            chat.AddMessage("user", request.Message);

            return await agent.AskAgentAsync(chat);
        }

        [HttpPost]
        [Route("stream")]
        public async Task ChatStream([FromBody] ChatRequest request, ChatHistoryProvider chatProvider, AgentsFactory factory, AgentLogger logger)
        {
            Response.ContentType = "text/event-stream";

            var agent = await factory.CreateAgentAsync<OrchestratorAgent>();

            var chat = chatProvider.GetChatHistory(request.SessionId);
            chat.AddMessage("user", request.Message);

            RunOrchestratorStreamAsync(chat, agent, logger, HttpContext.RequestAborted);

            try
            {
                await foreach (var log in logger.GetLogsAsync(HttpContext.RequestAborted))
                {
                    await Response.WriteAsync($"data: {log}\n\n", HttpContext.RequestAborted);
                    await Response.Body.FlushAsync(HttpContext.RequestAborted);
                }
            }
            catch (OperationCanceledException)
            {
                logger.Complete();
            }
        }

        private static void RunOrchestratorStreamAsync(ChatHistory chat, OrchestratorAgent orchestrator, AgentLogger logger, CancellationToken disconnectToken)
        {
            Task.Run(async () =>
            {
                await orchestrator.AskAgentStream(chat);
                logger.Complete();
            }, disconnectToken);
        }
    }
}
