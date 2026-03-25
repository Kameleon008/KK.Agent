using Newtonsoft.Json;
using System.Text;

namespace KK.Agent.Library.Clients.OpenApi.V1;

public class ChatCompletionsRequest
{

    [JsonProperty("model", NullValueHandling = NullValueHandling.Ignore)]
    public string? Model { get; set; }

    [JsonProperty("messages", NullValueHandling = NullValueHandling.Ignore)]
    public List<ChatMessage>? Messages { get; set; }

    [JsonProperty("temperature", NullValueHandling = NullValueHandling.Ignore)]
    public double? Temperature { get; set; }

    [JsonProperty("topP", NullValueHandling = NullValueHandling.Ignore)]
    public double? TopP { get; set; }

    [JsonProperty("maxTokens", NullValueHandling = NullValueHandling.Ignore)]
    public int? MaxTokens { get; set; }
    
    [JsonProperty("n", NullValueHandling = NullValueHandling.Ignore)]
    public int? N { get; set; }
    
    [JsonProperty("presence_penalty", NullValueHandling = NullValueHandling.Ignore)]
    public double? PresencePenalty { get; set; }
    
    [JsonProperty("frequency_penalty", NullValueHandling = NullValueHandling.Ignore)]
    public double? FrequencyPenalty { get; set; }
    
    [JsonProperty("logit_bias", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, int>? LogitBias { get; set; }
    
    [JsonProperty("stop", NullValueHandling = NullValueHandling.Ignore)]
    public List<string>? Stop { get; set; }
    
    [JsonProperty("response_format", NullValueHandling = NullValueHandling.Ignore)]
    public ChatCompletionResponseFormat? ResponseFormat { get; set; }
    
    [JsonProperty("stream", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Stream { get; set; }
    
    [JsonProperty("functions", NullValueHandling = NullValueHandling.Ignore)]
    public List<FunctionDefinition>? Functions { get; set; }
    
    [JsonProperty("function_call", NullValueHandling = NullValueHandling.Ignore)]
    public string? FunctionCall { get; set; }
    
    [JsonProperty("seed", NullValueHandling = NullValueHandling.Ignore)]
    public int? Seed { get; set; }
    
    [JsonProperty("user", NullValueHandling = NullValueHandling.Ignore)]
    public string? User { get; set; }

    public class ChatMessage
    {

        [JsonProperty("role", NullValueHandling = NullValueHandling.Ignore)]
        public string Role { get; set; } = null!;

        [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
        public string Content { get; set; } = null!;
    }

    public class ChatCompletionResponseFormat
    {
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; } = "text";
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