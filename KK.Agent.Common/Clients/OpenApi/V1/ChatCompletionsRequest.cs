using System.Text;
using Newtonsoft.Json;

namespace KK.Agent.Common.Clients.OpenApi.V1;

public class ChatCompletionsRequest
{
    [JsonProperty("model", NullValueHandling = NullValueHandling.Ignore)]
    public string? Model { get; set; }

    [JsonProperty("messages", NullValueHandling = NullValueHandling.Ignore)]
    public List<object>? Messages { get; set; }

    [JsonProperty("tools", NullValueHandling = NullValueHandling.Ignore)]
    public List<ToolDefinition>? Tools { get; set; }

    [JsonProperty("response_format", NullValueHandling = NullValueHandling.Ignore)]
    public ChatCompletionResponseFormat? ResponseFormat { get; set; }

    [JsonProperty("reasoning_effort", NullValueHandling = NullValueHandling.Ignore)]
    public string? ReasoningEffort { get; set; }

    [JsonProperty("temperature", NullValueHandling = NullValueHandling.Ignore)]
    public double? Temperature { get; set; }

    [JsonProperty("maxTokens", NullValueHandling = NullValueHandling.Ignore)]
    public int? MaxTokens { get; set; }

    [JsonProperty("stop", NullValueHandling = NullValueHandling.Ignore)]
    public List<string>? Stop { get; set; }

    [JsonProperty("stream", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Stream { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public StringContent ToHttpContent()
    {
        var jsonStringContent = JsonConvert.SerializeObject(this, Formatting.Indented);
        return new StringContent(jsonStringContent, Encoding.UTF8, "application/json");
    }
}

public class ChatMessage : IChatMessage
{

    [JsonProperty("role", NullValueHandling = NullValueHandling.Ignore)]
    public string Role { get; set; } = null!;

    [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
    public string Content { get; set; } = null!;

    [JsonProperty("tool_calls", NullValueHandling = NullValueHandling.Ignore)]
    public List<ToolCall>? ToolCalls { get; set; }

    [JsonProperty("tool_call_id", NullValueHandling = NullValueHandling.Ignore)]
    public string? ToolCallId { get; set; }
}

public class ToolCall
{
    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public string Type { get; set; } = "function";

    [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
    public string Id { get; set; } = null!;

    [JsonProperty("function", NullValueHandling = NullValueHandling.Ignore)]
    public ChatMessageFunctionCall Function { get; set; } = null!;
}

public class ChatMessageFunctionCall
{
    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string Name { get; set; } = null!;

    [JsonProperty("arguments", NullValueHandling = NullValueHandling.Ignore)]
    public string Arguments { get; set; } = null!;
}

public class ChatCompletionResponseFormat
{
    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public string Type { get; set; } = "text";

    [JsonProperty("json_schema", NullValueHandling = NullValueHandling.Ignore)]
    public JsonSchema? JsonSchema { get; set; }
}

public class JsonSchema
{
    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public string Type { get; set; } = "object";

    [JsonProperty("schema")]
    public object Schema { get; set; } = null!;

    [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
    public string? Description { get; set; }
}

public class FunctionDefinition
{

    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string Name { get; set; } = null!;

    [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
    public string Description { get; set; } = null!;

    [JsonProperty("parameters", NullValueHandling = NullValueHandling.Ignore)]
    public object Parameters { get; set; } = null!;
}

public class ToolDefinition
{
    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public string Type { get; set; } = "function";

    [JsonProperty("function", NullValueHandling = NullValueHandling.Ignore)]
    public ToolDefinitionFunction? Function { get; set; }
}

public class ToolDefinitionFunction
{
    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string Name { get; set; } = null!;

    [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
    public string Description { get; set; } = null!;

    [JsonProperty("parameters", NullValueHandling = NullValueHandling.Ignore)]
    public ParametersSchema? Parameters { get; set; }

    [JsonProperty("strict", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Strict { get; set; }
}

public class ParametersSchema
{
    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public string Type { get; set; } = "object";

    [JsonProperty("properties", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, PropertyDefinition>? Properties { get; set; }

    [JsonProperty("required", NullValueHandling = NullValueHandling.Ignore)]
    public List<string>? Required { get; set; }


    [JsonProperty("additionalProperties", NullValueHandling = NullValueHandling.Ignore)]
    public bool? AdditionalProperties { get; set; } = false;
}

public class PropertyDefinition
{
    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public string Type { get; set; } = "string";

    [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
    public string? Description { get; set; }
}

public class ChatImageMessage : IChatMessage
{

    [JsonProperty("role", NullValueHandling = NullValueHandling.Ignore)]
    public string Role { get; set; } = null!;

    [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
    public List<ChatImageContent> Content { get; set; } = null!;
}

public class ChatImageContent
{
    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public string Type { get; set; } = "image";

    [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
    public string? Text { get; set; }
    
    [JsonProperty("image_url", NullValueHandling = NullValueHandling.Ignore)]
    public ChatImage? Image { get; set; }
}

public class ChatImage
{
    [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
    public string Url { get; set; } = null!;
}