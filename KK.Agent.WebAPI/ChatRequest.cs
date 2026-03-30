using Newtonsoft.Json;

namespace KK.Agent.WebAPI
{
    public class ChatRequest
    {
        [JsonProperty("sessionId")]
        public string SessionId { get; set; } = string.Empty;

        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;
    }
}
