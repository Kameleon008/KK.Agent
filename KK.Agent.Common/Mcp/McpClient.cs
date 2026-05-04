using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using KK.Agent.Library.Clients.OpenApi.V1;

namespace KK.Agent.Library.Mcp;

public class McpClient(ConfigMcpServer options)
{
    public string Name => options.Name;

    private Process? _process;

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

        _process = new Process { StartInfo = psi };
        _process.Start();
    }

    public async Task LoadToolsAsync(List<ToolDefinition> tools)
    {
        this.Start();

        if (this._process == null )
        {
            return;
        }

        var request = new McpToolCallRequest
        {
            JsonRpc = "2.0",
            Id = 1,
            Method = "tools/list"
        };

        var json = JsonConvert.SerializeObject(request);

        await this._process.StandardInput.WriteLineAsync(json);
        await this._process.StandardInput.FlushAsync();

        var buffer = new StringBuilder();
        while (true)
        {
            var line = await this._process.StandardOutput.ReadLineAsync();

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
        var argumentsObj = arguments != "{}" && !string.IsNullOrEmpty(arguments)
            ? JsonConvert.DeserializeObject<Dictionary<string, object?>>(arguments)
            : new Dictionary<string, object?>();

        var request = new McpToolCallRequest
        {
            JsonRpc = "2.0",
            Id = 1,
            Method = "tools/call",
            Params = new McpToolCallParams
            {
                Name = toolName,
                Arguments = argumentsObj
            }
        };

        var json = JsonConvert.SerializeObject(request);

        this.Start();

        if (this._process == null)
        {
            return  $"Error: Failed to start MCP process for tool {toolName}";
        }

        await this._process.StandardInput.WriteLineAsync(json);
        await this._process.StandardInput.FlushAsync();

        var buffer = new StringBuilder();
        while (true)
        {
            var line = await this._process.StandardOutput.ReadLineAsync();
            Console.WriteLine(line);
            if (line == null) break;

            buffer.Append(line.Trim());
            if (line.Trim().EndsWith("}")) break;
        }

        var response = buffer.ToString();
        Console.WriteLine(response);

        return response;
    }


    private class McpToolCallRequest
    {
        [JsonProperty("jsonrpc")]
        public string JsonRpc { get; set; } = "2.0";

        [JsonProperty("id")]
        public int Id { get; set; } = 1;

        [JsonProperty("method")]
        public string Method { get; set; } = "tools/call";

        [JsonProperty("params", NullValueHandling = NullValueHandling.Ignore)]
        public McpToolCallParams? Params { get; set; }
    }

    private class McpToolCallParams
    {
        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        [JsonProperty("arguments")]
        public Dictionary<string, object?>? Arguments { get; set; }
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