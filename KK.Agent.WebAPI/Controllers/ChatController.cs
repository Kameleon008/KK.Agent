using KK.Agent.Library.Agents;
using KK.Agent.Library.Mcp;
using KK.Agent.WebAPI.Agents;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;

namespace KK.Agent.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        [HttpPost]
        public async Task<string> Chat([FromBody] ChatRequest request, OrchestratorAgent orchestrator, CancellationToken ct)
        {
            orchestrator.AddMessage("user", request.Message, request.SessionId);
            return await orchestrator.RunAsync(request.SessionId);
        }

        [HttpPost]
        [Route("stream")]
        public async Task StreamChat([FromBody] ChatRequest request, OrchestratorAgent orchestrator, AgentLogger logger)
        {
            Response.ContentType = "text/event-stream";

            var disconnectToken = HttpContext.RequestAborted;

            orchestrator.AddMessage("user", request.Message, request.SessionId);
            RunOrchestratorStreamAsync(request, orchestrator, logger, disconnectToken);

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

        [HttpPost]
        [Route("mcp")]
        public async Task<string> McpStdioTest([FromBody] JsonElement request, McpClient client, AgentLogger logger)
        {
            client.Start();

            var json = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(request.GetRawText()));
            await client.Process.StandardInput.WriteLineAsync(json);
            await client.Process.StandardInput.FlushAsync();

            await client.Process.StandardInput.WriteLineAsync(json);
            await client.Process.StandardInput.FlushAsync();

            var buffer = new StringBuilder();
            while (true)
            {
                var line = await client.Process.StandardOutput.ReadLineAsync();
                Console.WriteLine(line);
                if (line == null) break;

                buffer.Append(line.Trim());
                if (line.Trim().EndsWith("}")) break;
            }

            var response = buffer.ToString();
            Console.WriteLine(response);

            return response;
        }

        private static void RunOrchestratorStreamAsync(ChatRequest request, OrchestratorAgent orchestrator, AgentLogger logger, CancellationToken disconnectToken)
        {
            var _ = Task.Run(async () =>
            {
                await orchestrator.RunStreamAsync(request.SessionId);
                logger.Complete();
            }, disconnectToken);
        }
    }
}
