using KK.Agent.Library.Mcp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;
using KK.Agent.Library;
using KK.Agent.Library.AgentEngine;

namespace KK.Agent.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class McpController : ControllerBase
    {
        [HttpPost]
        public async Task<string> McpStdioTest([FromBody] JsonElement request, McpClient client, AgentLogger logger)
        {
            client.Start();

            var json = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(request.GetRawText()));

            if(client.Process?.StandardInput == null || client.Process.StandardOutput == null)
            {
                throw new InvalidOperationException("MCP client process is not properly initialized.");
            }

            await client.Process.StandardInput.WriteLineAsync(json);
            await client.Process.StandardInput.FlushAsync();

            var buffer = new StringBuilder();
            while (true)
            {
                var line = await client.Process.StandardOutput.ReadLineAsync();
                Console.WriteLine(line);
                if (line == null) break;

                buffer.Append(line.Trim());
                if (line.Trim().EndsWith($"}}")) break;
            }

            var response = buffer.ToString();
            Console.WriteLine(response);

            return response;
        }
    }
}
