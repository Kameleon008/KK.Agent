using KK.Agent.Library.Clients.OpenApi.V1;

namespace KK.Agent.Library.Agents.FinishReasonHandlers
{
    public class FinishReasonHandlerToolCalls(Dictionary<string, Func<string, Task<string>>> tools, AgentLogger logger) : IFinishReasonHandler
    {
        public bool Handles(string finishReason) => finishReason == "tool_calls";

        public async Task<string?> HandleAsync(string caller, ChatCompletionChoice choice, ChatHistory history)
        {
            foreach (var toolCall in choice.Message.ToolCalls!)
            {
                await logger.PublishAsync("System", $"{caller} calls tool: {toolCall.Function!.Name}..., arguments: {toolCall.Function.Arguments}", string.Empty);

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
