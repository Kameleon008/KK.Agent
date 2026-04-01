using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Newtonsoft.Json;

namespace KK.Agent.Library.Agents
{
    public class AgentLogger
    {
        private readonly Channel<AgentLoggerModel> _channel = Channel.CreateUnbounded<AgentLoggerModel>();

        public async Task PublishAsync(string agentId, string message)
        {
            var log = $"[{agentId}]: {message}";
            Console.WriteLine(log);
            await _channel.Writer.WriteAsync(new AgentLoggerModel
            {
                AgentId = agentId,
                Message = message
            });
        }

        public async IAsyncEnumerable<string> GetLogsAsync([EnumeratorCancellation] CancellationToken ct)
        {
            while (await _channel.Reader.WaitToReadAsync(ct))
            {
                while (_channel.Reader.TryRead(out var log))
                {
                    yield return JsonConvert.SerializeObject(log);
                }
            }
        }

        public void Complete()
        {
            _channel.Writer.Complete();
        }
    }

    public class AgentLoggerModel
    {
        [JsonProperty("agentId")]
        public string AgentId { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
