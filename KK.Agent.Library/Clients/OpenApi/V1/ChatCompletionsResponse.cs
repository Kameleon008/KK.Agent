using System.Text.Json;
using Newtonsoft.Json;

namespace KK.Agent.Library.Clients.OpenApi.V1;

public class ChatCompletionsResponse
{
    [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("object", NullValueHandling = NullValueHandling.Ignore)]
    public string Object => "chat.completion";

    [JsonProperty("created", NullValueHandling = NullValueHandling.Ignore)]
    public int Created { get; set; }

    [JsonProperty("model", NullValueHandling = NullValueHandling.Ignore)]
    public string Model { get; set; } = string.Empty;

    [JsonProperty("choices", NullValueHandling = NullValueHandling.Ignore)]
    public ChatCompletionChoice[] Choices { get; set; } = [];

    [JsonProperty("usage", NullValueHandling = NullValueHandling.Ignore)]
    public ChatCompletionUsage? Usage { get; set; }

    [JsonProperty("stats", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, object>? Stats { get; set; }

    [JsonProperty("system_fingerprint", NullValueHandling = NullValueHandling.Ignore)]
    public string SystemFingerprint { get; set; } = string.Empty;
}

public class ChatCompletionChoice
{
    [JsonProperty("index", NullValueHandling = NullValueHandling.Ignore)]
    public int Index { get; set; }

    [JsonProperty("delta", NullValueHandling = NullValueHandling.Ignore)]
    public ChatCompletionMessage Delta { get; set; } = new();

    [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
    public ChatCompletionMessage Message { get; set; } = new();

    [JsonProperty("logprobs", NullValueHandling = NullValueHandling.Ignore)]
    public JsonElement? LogProbs { get; set; }

    [JsonProperty("finish_reason", NullValueHandling = NullValueHandling.Ignore)]
    public string FinishReason { get; set; } = string.Empty;
}

public class ChatCompletionMessage
{
    /// <summary>
    /// Role of the message sender (e.g., "assistant", "user", "system").
    /// </summary>
    [JsonProperty("role", NullValueHandling = NullValueHandling.Ignore)]
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// The content of the message, if any.
    /// </summary>
    [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
    public string? Content { get; set; }

    /// <summary>
    /// Tool calls made by the model (if applicable).
    /// </summary>
    [JsonProperty("tool_calls", NullValueHandling = NullValueHandling.Ignore)]
    public ChatCompletionToolCall[]? ToolCalls { get; set; }
}

public class ChatCompletionToolCall
{
    [JsonProperty("index", NullValueHandling = NullValueHandling.Ignore)]
    public int Index { get; set; }

    [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
    public string? Id { get; set; }

    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public string? Type { get; set; }

    [JsonProperty("function", NullValueHandling = NullValueHandling.Ignore)]
    public ChatCompletionToolCallFunction? Function { get; set; }
}

public class ChatCompletionToolCallFunction
{
    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("arguments", NullValueHandling = NullValueHandling.Ignore)]
    public string Arguments { get; set; } = string.Empty;
}

public class ChatCompletionUsage
{
    public int PromptTokens { get; set; }

    [JsonProperty("completion_tokens", NullValueHandling = NullValueHandling.Ignore)]
    public int CompletionTokens { get; set; }

    [JsonProperty("total_tokens", NullValueHandling = NullValueHandling.Ignore)]
    public int TotalTokens { get; set; }
}
