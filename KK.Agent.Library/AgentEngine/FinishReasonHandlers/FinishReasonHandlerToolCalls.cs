using KK.Agent.Library.Clients.OpenApi.V1;
using KK.Agent.Library.Tools;

namespace KK.Agent.Library.AgentEngine.FinishReasonHandlers
{
    public class FinishReasonHandlerToolCalls(ToolsProvider toolsProvider, AgentLogger logger) : IFinishReasonHandler
    {
        public bool Handles(string finishReason) => finishReason == "tool_calls";

        public async Task<string?> HandleAsync(string caller, ChatCompletionChoice choice, ChatHistory history)
        {
            foreach (var toolCall in choice.Message.ToolCalls!)
            {
                await logger.PublishAsync("Tool_Call", $"{caller} calls tool: {toolCall.Function!.Name}..., arguments: {toolCall.Function.Arguments}", string.Empty);

                if (toolsProvider.Tools.ContainsKey(toolCall.Function!.Name))
                {
                    var result = await toolsProvider.Tools[toolCall.Function!.Name](toolCall.Function.Arguments);

                    await logger.PublishAsync("Tool_Call", $"{caller} calls tool: {toolCall.Function!.Name}..., result: {result}", string.Empty);

                    history.Add(new ChatMessage
                    {
                        Role = "tool",
                        Content = result,
                        ToolCallId = toolCall.Id
                    });
                }
                else
                {
                    if (toolsProvider.McpClients.Any() == false)
                    {
                        continue;
                    }

                    var result = await toolsProvider.McpClients.First().CallToolAsync(toolCall.Function!.Name, toolCall.Function.Arguments);

                    await logger.PublishAsync("Tool_Call_Result", $"\n result: {result}", string.Empty);

                    history.Add(new ChatMessage
                    {
                        Role = "tool",
                        Content = result,
                        ToolCallId = toolCall.Id
                    });
                }
            }

            return null;
        }
    }
}
