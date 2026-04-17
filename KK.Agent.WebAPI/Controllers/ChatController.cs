using KK.Agent.Library.Agents;
using KK.Agent.WebAPI.Agents;
using Microsoft.AspNetCore.Mvc;
using KK.Agent.Library;

namespace KK.Agent.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        [HttpPost]
        public async Task<string> Chat([FromBody] ChatRequest request, OrchestratorAgent orchestrator, ChatHistoryProvider chatProvider, CancellationToken ct)
        {
            var chat = chatProvider.GetChatHistory(request.SessionId);
            chat.AddMessage("user", request.Message);
            return await orchestrator.AskAgentAsync(chat);
        }

        [HttpPost]
        [Route("stream")]
        public async Task ChatStream([FromBody] ChatRequest request, OrchestratorAgent orchestrator, ChatHistoryProvider chatProvider, AgentLogger logger)
        {
            Response.ContentType = "text/event-stream";

            var disconnectToken = HttpContext.RequestAborted;

            var chat = chatProvider.GetChatHistory(request.SessionId);
            chat.AddMessage("user", request.Message);

            RunOrchestratorStreamAsync(chat, orchestrator, logger, disconnectToken);

            try
            {
                await foreach (var log in logger.GetLogsAsync(disconnectToken))
                {
                    await Response.WriteAsync($"data: {log}\n\n", disconnectToken);
                    await Response.Body.FlushAsync(disconnectToken);
                }
            }
            catch (OperationCanceledException)
            {
                logger.Complete();
            }
        }

        private static void RunOrchestratorStreamAsync(ChatHistory chat, OrchestratorAgent orchestrator, AgentLogger logger, CancellationToken disconnectToken)
        {
            var _ = Task.Run(async () =>
            {
                await orchestrator.AskAgentStream(chat);
                logger.Complete();
            }, disconnectToken);
        }
    }
}
