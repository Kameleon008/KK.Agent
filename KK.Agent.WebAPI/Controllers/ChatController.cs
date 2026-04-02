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
        public async Task<string> Chat([FromBody] ChatRequest request, OrchestratorAgent orchestrator, CancellationToken ct)
        {
            return await orchestrator.RunAsync(request.Message, request.SessionId);
        }

        [HttpPost]
        [Route("stream")]
        public async Task StreamChat([FromBody] ChatRequest request, OrchestratorAgent orchestrator, AgentLogger logger)
        {
            Response.ContentType = "text/event-stream";

            var disconnectToken = HttpContext.RequestAborted;

            RunOrchestratorStreamAsync(request, orchestrator, logger, disconnectToken);

            await foreach (var log in logger.GetLogsAsync(disconnectToken))
            {
                await Response.WriteAsync($"data: {log}\n\n", disconnectToken);
                await Response.Body.FlushAsync(disconnectToken);
            }
        }

        private static void RunOrchestratorStreamAsync(ChatRequest request, OrchestratorAgent orchestrator, AgentLogger logger, CancellationToken disconnectToken)
        {
            var _ = Task.Run(async () =>
            {
                await orchestrator.RunStreamAsync(request.Message, request.SessionId);
                logger.Complete();
            }, disconnectToken);
        }
    }
}
