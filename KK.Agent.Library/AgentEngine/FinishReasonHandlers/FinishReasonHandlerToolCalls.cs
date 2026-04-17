using KK.Agent.Library.Clients.OpenApi.V1;
using KK.Agent.Library.Mcp;

namespace KK.Agent.Library.Agents.FinishReasonHandlers
{
    public class FinishReasonHandlerToolCalls(Dictionary<string, Func<string, Task<string>>> tools, AgentLogger logger, List<McpClient> mcpClients) : IFinishReasonHandler
    {
        public bool Handles(string finishReason) => finishReason == "tool_calls";

        public async Task<string?> HandleAsync(string caller, ChatCompletionChoice choice, ChatHistory history)
        {
            foreach (var toolCall in choice.Message.ToolCalls!)
            {
                await logger.PublishAsync("Tool_Call", $"{caller} calls tool: {toolCall.Function!.Name}..., arguments: {toolCall.Function.Arguments}", string.Empty);

                if (tools.ContainsKey(toolCall.Function!.Name))
                {
                    var result = await tools[toolCall.Function!.Name](toolCall.Function.Arguments);

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
                    if (!mcpClients.Any())
                    {
                        continue;
                    }

                    var result = await mcpClients.First().CallToolAsync(toolCall.Function!.Name, toolCall.Function.Arguments);

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
