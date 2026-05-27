using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Newtonsoft.Json;

namespace KK.Agent.Common.AgentEngine
{
    public class AgentLogger
    {
        private Channel<AgentLoggerModel> _channel = Channel.CreateUnbounded<AgentLoggerModel>();

        public async Task PublishAsync(string agentId, string reasoning, string content)
        {
            Console.WriteLine($"[AgentLogger] AgentId: {agentId}, Reasoning: {reasoning}, Content: {content}");
            await _channel.Writer.WriteAsync(new AgentLoggerModel
            {
                AgentId = agentId,
                Reasoning = reasoning,
                Content = content
            });
        }

        public async Task PublishReasoningAsync(string agentId, string reasoning)
        {
            Console.WriteLine($"[AgentLogger] AgentId: {agentId}, Reasoning: {reasoning}, Content: {string.Empty}");
            await _channel.Writer.WriteAsync(new AgentLoggerModel
            {
                AgentId = agentId,
                Reasoning = reasoning,
                Content = string.Empty
            });
        }

        public async Task PublishContentAsync(string agentId, string content)
        {
            Console.WriteLine($"[AgentLogger] AgentId: {agentId}, Reasoning: {string.Empty}, Content: {content}");
            await _channel.Writer.WriteAsync(new AgentLoggerModel
            {
                AgentId = agentId,
                Reasoning = string.Empty,
                Content = content
            });
        }

        public async IAsyncEnumerable<string> GetLogsAsync([EnumeratorCancellation] CancellationToken ct)
        {
            await foreach (var log in _channel.Reader.ReadAllAsync(ct))
            {
                yield return JsonConvert.SerializeObject(log);
            }
        }

        public void Complete()
        {
            _channel.Writer.Complete();
            this._channel = Channel.CreateUnbounded<AgentLoggerModel>();
        }
    }

    public class AgentLoggerModel
    {
        [JsonProperty("agentId")]
        public required string AgentId { get; set; }

        [JsonProperty("reasoning")]
        public required string Reasoning { get; set; }

        [JsonProperty("content")]
        public required string Content { get; set; }
    }
}
