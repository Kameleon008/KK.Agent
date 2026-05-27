using KK.Agent.Common.Clients.OpenApi.V1;
using KK.Agent.Common.Tools;

namespace KK.Agent.Common.AgentEngine.FinishReasonHandlers
{
    public class FinishReasonHandlerToolCalls(ToolsProvider toolsProvider, AgentLogger logger) : IFinishReasonHandler
    {
        public bool Handles(string finishReason) => finishReason == "tool_calls";

        public async Task<string?> HandleAsync(string caller, ChatCompletionChoice choice, ChatHistory history)
        {
            foreach (var toolCall in choice.Message.ToolCalls!)
            {
                await logger.PublishReasoningAsync("Tool_Call", $"\n{caller} calls tool: {toolCall.Function!.Name}, \n\n{toolCall.Function.Arguments}");

                if (toolsProvider.Tools.ContainsKey(toolCall.Function!.Name))
                {
                    var result = await toolsProvider.Tools[toolCall.Function!.Name](toolCall.Function.Arguments);

                    await logger.PublishReasoningAsync("Tool_Call_Result", $"\n{caller} calls tool: {toolCall.Function!.Name}, \n\n{result}");

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

                    await logger.PublishReasoningAsync("Tool_Call_Result", $"\n{caller} calls tool: {toolCall.Function!.Name}, \n\n{result}");

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
