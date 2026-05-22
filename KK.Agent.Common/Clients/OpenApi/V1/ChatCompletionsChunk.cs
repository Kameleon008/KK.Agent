using Newtonsoft.Json;

namespace KK.Agent.Common.Clients.OpenApi.V1
{
    public class ChatCompletionsChunk
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("created")]
        public long Created { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("system_fingerprint")]
        public string SystemFingerprint { get; set; }

        [JsonProperty("choices")]
        public List<ChunkChoice>? Choices { get; set; }
    }

    public class ChunkChoice
    {
        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("delta")]
        public ChunkDelta Delta { get; set; }

        [JsonProperty("logprobs")]
        public object Logprobs { get; set; }

        [JsonProperty("finish_reason")]
        public string FinishReason { get; set; }
    }

    public class ChunkDelta
    {
        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("reasoning_content")]
        public string ReasoningContent { get; set; }

        [JsonProperty("tool_calls")]
        public List<ChunkToolCall>? ToolCalls { get; set; }
    }

    public class ChunkToolCall
    {
        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("function")]
        public ChunkFunctionCall? Function { get; set; }
    }

    public class ChunkFunctionCall
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("arguments")]
        public string Arguments { get; set; }
    }
}
