using KK.Agent.Library.Agents;
using KK.Agent.WebAPI.Agents;
using KK.Agent.WebAPI.Tools;
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
            return await orchestrator.RunAsync(request.Message);
        }

        [HttpPost]
        [Route("stream")]
        public async Task StreamChat([FromBody] ChatRequest request, OrchestratorAgent orchestrator, AgentLogger logger)
        {

            Response.ContentType = "text/event-stream";
            Response.Headers.Append("Cache-Control", "no-cache");

            var disconnectToken = HttpContext.RequestAborted;

            var orchestratorTask = Task.Run(async () =>
            {
                try
                {
                    await orchestrator.RunWithLoggerAsync(request.Message);
                }
                finally
                {
                    logger.Complete();
                }
            }, disconnectToken);

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
            }
            finally
            {
                await orchestratorTask;
            }
        }
    }
}
