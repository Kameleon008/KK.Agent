using KK.Agent.WebAPI.Agents;
using Microsoft.AspNetCore.Mvc;

namespace KK.Agent.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        [HttpPost]
        public async Task<string> StreamChat([FromBody] ChatRequest request, OrchestratorAgent orchestrator, CancellationToken ct)
        {
            return await orchestrator.RunAsync(request.Message);
        }
    }
}
