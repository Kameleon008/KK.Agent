using Newtonsoft.Json;
using System.Text;

namespace KK.Agent.Library.Clients.OpenApi.Models.V1;

public class ChatCompletionsRequest
{

    [JsonProperty("model", 
        DefaultValueHandling = DefaultValueHandling.Ignore, 
        NullValueHandling = NullValueHandling.Ignore)]
    public string? Model { get; set; }

    [JsonProperty("messages",
        DefaultValueHandling = DefaultValueHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore)]
    public List<ChatMessage>? Messages { get; set; }

    [JsonProperty("temperature",
        DefaultValueHandling = DefaultValueHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore)]
    public double? Temperature { get; set; }

    [JsonProperty("topP",
        DefaultValueHandling = DefaultValueHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore)]
    public double? TopP { get; set; }

    [JsonProperty("maxTokens",
        DefaultValueHandling = DefaultValueHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore)]
    public int? MaxTokens { get; set; }
    
    [JsonProperty("n",
        DefaultValueHandling = DefaultValueHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore)]
    public int? N { get; set; }
    
    [JsonProperty("presence_penalty",
        DefaultValueHandling = DefaultValueHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore)]
    public double? PresencePenalty { get; set; }
    
    [JsonProperty("frequency_penalty",
        DefaultValueHandling = DefaultValueHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore)]
    public double? FrequencyPenalty { get; set; }
    
    [JsonProperty("logit_bias",
        DefaultValueHandling = DefaultValueHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, int>? LogitBias { get; set; }
    
    [JsonProperty("stop",
        DefaultValueHandling = DefaultValueHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore)]
    public List<string>? Stop { get; set; }
    
    [JsonProperty("response_format",
        DefaultValueHandling = DefaultValueHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore)]
    public ChatCompletionResponseFormat? ResponseFormat { get; set; }
    
    [JsonProperty("stream",
        DefaultValueHandling = DefaultValueHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore)]
    public bool? Stream { get; set; }
    
    [JsonProperty("functions",
        DefaultValueHandling = DefaultValueHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore)]
    public List<FunctionDefinition>? Functions { get; set; }
    
    [JsonProperty("function_call",
        DefaultValueHandling = DefaultValueHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore)]
    public string? FunctionCall { get; set; }
    
    [JsonProperty("seed",
        DefaultValueHandling = DefaultValueHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore)]
    public int? Seed { get; set; }
    
    [JsonProperty("user",
        DefaultValueHandling = DefaultValueHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore)]
    public string? User { get; set; }

    public class ChatMessage
    {

        [JsonProperty("role",
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore)]
        public string Role { get; set; } = null!;

        [JsonProperty("content",
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore)]
        public string Content { get; set; } = null!;
    }

    public class ChatCompletionResponseFormat
    {
        [JsonProperty("type",
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; } = "text";
    }

    public class FunctionDefinition
    {

        [JsonProperty("name",
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; } = null!;

        [JsonProperty("description",
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; } = null!;

        [JsonProperty("parameters",
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore)]
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

public class ChatCompletionsRequestBuilder
{
    private readonly ChatCompletionsRequest _request = new();

    public ChatCompletionsRequestBuilder SetModel(string model)
    {
        _request.Model = model;
        return this;
    }

    public ChatCompletionsRequestBuilder SetMessages(params ChatCompletionsRequest.ChatMessage[]? messages)
    {
        _request.Messages = messages?.ToList();
        return this;
    }

    public ChatCompletionsRequestBuilder AddMessage(ChatCompletionsRequest.ChatMessage message)
    {
        _request.Messages ??= [];
        _request.Messages.Add(message);
        return this;
    }

    public ChatCompletionsRequestBuilder SetTemperature(double temperature)
    {
        _request.Temperature = temperature;
        return this;
    }

    public ChatCompletionsRequestBuilder SetTopP(double topP)
    {
        _request.TopP = topP;
        return this;
    }

    public ChatCompletionsRequestBuilder SetMaxTokens(int maxTokens)
    {
        _request.MaxTokens = maxTokens;
        return this;
    }

    public ChatCompletionsRequestBuilder SetN(int n)
    {
        _request.N = n;
        return this;
    }

    public ChatCompletionsRequestBuilder SetPresencePenalty(double penalty)
    {
        _request.PresencePenalty = penalty;
        return this;
    }

    public ChatCompletionsRequestBuilder SetFrequencyPenalty(double penalty)
    {
        _request.FrequencyPenalty = penalty;
        return this;
    }

    public ChatCompletionsRequestBuilder SetLogitBias(Dictionary<string, int> bias)
    {
        _request.LogitBias = bias;
        return this;
    }

    public ChatCompletionsRequestBuilder SetStop(params string[]? stopSequences)
    {
        _request.Stop = stopSequences?.ToList();
        return this;
    }

    public ChatCompletionsRequestBuilder SetResponseFormat(string type)
    {
        _request.ResponseFormat = new ChatCompletionsRequest.ChatCompletionResponseFormat { Type = type };
        return this;
    }

    public ChatCompletionsRequestBuilder SetStream(bool stream)
    {
        _request.Stream = stream;
        return this;
    }

    public ChatCompletionsRequestBuilder SetFunctions(params ChatCompletionsRequest.FunctionDefinition[]? functions)
    {
        _request.Functions = functions?.ToList();
        return this;
    }

    public ChatCompletionsRequestBuilder SetFunctionCall(string functionCall)
    {
        _request.FunctionCall = functionCall;
        return this;
    }

    public ChatCompletionsRequestBuilder SetSeed(int seed)
    {
        _request.Seed = seed;
        return this;
    }

    public ChatCompletionsRequestBuilder SetUser(string user)
    {
        _request.User = user;
        return this;
    }

    public ChatCompletionsRequest Build()
    {
        return _request;
    }
}
