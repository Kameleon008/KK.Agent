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
        public async Task StreamChat([FromBody] ChatRequest request, OrchestratorAgent orchestrator, AgentLogger logger, OrchestratorTools tools, CancellationToken ct)
        {
            //orchestrator.AddToolInstance(tools);
            orchestrator.AddToolInstance(tools);

            Response.ContentType = "text/event-stream";
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");

            var task = orchestrator.RunWithLoggerAsync(request.Message);

            _ = Task.Run(async () =>
            {
                try
                {
                    await task;
                }
                finally
                {
                    logger.Complete();
                }
            }, ct);

            await foreach (var log in logger.GetLogsAsync(HttpContext.RequestAborted))
            {
                await Response.WriteAsync($"data: {log}\n\n");
                await Response.Body.FlushAsync();
            }
        }
    }
}
