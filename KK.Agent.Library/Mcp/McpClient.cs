using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using KK.Agent.Library.Clients.OpenApi.V1;

namespace KK.Agent.Library.Mcp;

public class McpClient(ConfigMcpServer options)
{
    public string Name => options.Name;

    public Process? Process;

    public StreamWriter? Input => Process?.StandardInput;

    public StreamReader? Output => Process?.StandardOutput;

    public List<McpTool> Tools { get; private set; } = [];

    public void Start()
    {
        var psi = new ProcessStartInfo
        {
            FileName = options.Command,
            Arguments = options.Arguments,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = options.UseShellExecute
        };

        foreach (var kv in options.EnvironmentVariables)
        {
            psi.Environment[kv.Key] = kv.Value;
        }

        if (!string.IsNullOrEmpty(options.WorkingDirectory))
        {
            psi.WorkingDirectory = options.WorkingDirectory;
        }

        Process = new Process { StartInfo = psi };
        Process.Start();
    }

    public async Task LoadToolsAsync(List<ToolDefinition> tools)
    {
        this.Start();

        var request = new
        {
            jsonrpc = "2.0",
            id = 1,
            method = "tools/list"
        };

        var json = JsonConvert.SerializeObject(request);
        await this.Input.WriteLineAsync(json);
        await this.Input.FlushAsync();

        var buffer = new StringBuilder();
        while (true)
        {
            var line = await this.Output.ReadLineAsync();
            Console.WriteLine(line);
            if (line == null) break;

            buffer.Append(line.Trim());
            if (line.Trim().EndsWith("}")) break;
        }

        var response = buffer.ToString();
        var parsedResponse = JsonConvert.DeserializeObject<McpResponse>(response);
        if (parsedResponse?.Result?.Tools != null)
        {
            Tools = parsedResponse.Result.Tools;
        }

        var toolDefinitions = this.Tools.Select(tool => new ToolDefinition()
        {
            Type = "function",
            Function = new ToolDefinitionFunction()
            {
                Name = tool.Name,
                Description = tool.Description,
                Parameters = tool.InputSchema,
                Strict = true,
            }
        }).ToList();

        tools.AddRange(toolDefinitions);
    }

    public async Task<string> CallToolAsync(string toolName, string arguments)
    {
        var json = $"{{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"tools/call\",\"params\":{{\"name\":\"{toolName}\",\"arguments\":{arguments}}}}}";

        this.Start();

        await this.Process.StandardInput.WriteLineAsync(json);
        await this.Process.StandardInput.FlushAsync();

        var buffer = new StringBuilder();
        while (true)
        {
            var line = await this.Process.StandardOutput.ReadLineAsync();
            Console.WriteLine(line);
            if (line == null) break;

            buffer.Append(line.Trim());
            if (line.Trim().EndsWith("}")) break;
        }

        var response = buffer.ToString();
        Console.WriteLine(response);

        return response;
    }


    private class McpResponse
    {
        [JsonProperty("result")]
        public McpContent? Result { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("jsonrpc")]
        public string JsonRpc { get; set; } = string.Empty;
    }

    private class McpContent
    {
        [JsonProperty("tools")]
        public List<McpTool> Tools { get; set; } = new();
    }

    public class McpTool
    {
        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        [JsonProperty("description")]
        public string Description { get; set; } = null!;

        [JsonProperty("inputSchema")]
        public ParametersSchema InputSchema { get; set; } = null!;
    }
}