using KK.Agent.Library.Clients.OpenApi.V1;

namespace KK.Agent.Library.Agents.FinishReasonHandlers
{
    public class FinishReasonHandlerToolCalls(Dictionary<string, Func<string, Task<string>>> tools) : IFinishReasonHandler
    {
        public bool Handles(string finishReason) => finishReason == "tool_calls";

        public async Task<string?> HandleAsync(ChatCompletionChoice choice, ChatHistory history)
        {
            foreach (var toolCall in choice.Message.ToolCalls!)
            {
                Console.WriteLine($"[Agent]: Calls tool: {toolCall.Function!.Name}...");

                var result = await tools[toolCall.Function!.Name](toolCall.Function.Arguments);

                history.Add(new ChatMessage
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
