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
            return await orchestrator.RunAsync(request.Message);
        }

        [HttpPost]
        [Route("stream")]
        public async Task StreamChat([FromBody] ChatRequest request, OrchestratorAgent orchestrator, CancellationToken ct)
        {
            Response.ContentType = "text/event-stream";
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");

            await foreach (var chunk in orchestrator.RunStreamAsync(request.Message).WithCancellation(ct))
            {
                if (string.IsNullOrEmpty(chunk) is false)
                {
                    await Response.WriteAsync($"data: {chunk}\n\n", ct);
                    await Response.Body.FlushAsync(ct); 
                }
            }
        }
    }
}
