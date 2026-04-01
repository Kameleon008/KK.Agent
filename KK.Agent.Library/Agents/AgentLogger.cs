using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace KK.Agent.Library.Agents
{
    public class AgentLogger
    {
        private readonly Channel<string> _channel = Channel.CreateUnbounded<string>();

        // 🔥 To wołasz z innych serwisów (zamiast Observer.Notify)
        public async Task PublishAsync(string agentId, string message)
        {
            var log = $"[{agentId}]: {message}";
            Console.WriteLine(log);
            await _channel.Writer.WriteAsync(log);
        }

        // 🔥 To konsumuje controller
        public async IAsyncEnumerable<string> GetLogsAsync(
            [EnumeratorCancellation] CancellationToken ct)
        {
            while (await _channel.Reader.WaitToReadAsync(ct))
            {
                while (_channel.Reader.TryRead(out var log))
                {
                    yield return log;
                }
            }
        }

        // opcjonalnie — zakończenie streama
        public void Complete()
        {
            _channel.Writer.Complete();
        }
    }
}
