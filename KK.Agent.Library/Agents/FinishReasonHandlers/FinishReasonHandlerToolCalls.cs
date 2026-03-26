using KK.Agent.Library.Clients.OpenApi.V1;

namespace KK.Agent.Library.Agents.FinishReasonHandlers
{
    public class FinishReasonHandlerToolCalls : IFinishReasonHandler
    {
        private readonly Dictionary<string, Func<string, Task<string>>> _tools;
        private readonly List<ChatMessage> _history;

        public FinishReasonHandlerToolCalls(
            Dictionary<string, Func<string, Task<string>>> tools,
            List<ChatMessage> history)
        {
            _tools = tools;
            _history = history;
        }

        public bool Handles(string finishReason) => finishReason == "tool_calls";

        public async Task<string?> HandleAsync(ChatCompletionChoice choice)
        {
            foreach (var toolCall in choice.Message.ToolCalls!)
            {
                Console.WriteLine($"[Agent]: Calls tool: {toolCall.Function!.Name}...");

                var result = await _tools[toolCall.Function!.Name](toolCall.Function.Arguments);

                _history.Add(new ChatMessage
                {
                    Role = "tool",
                    Content = result,
                    ToolCallId = toolCall.Id
                });
            }

            return null;
        }
    }
}
