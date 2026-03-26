using KK.Agent.Library.Attributes;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Clients.OpenApi.V1;
using KK.Agent.Library.Tools;

namespace KK.Agent.Library.Agents
{
    public abstract class AgentBase(OpenApiClient provider)
    {
        private readonly List<ChatMessage> _history = [];
        private readonly List<ToolDefinition> _toolDefinitions = [];
        private readonly Dictionary<string, Func<string, Task<string>>> _tools = new ();

        private const string SystemPrompt = "You are helpful AI assistant";

        protected AgentBase(OpenApiClient provider, params object[] toolsInstances) : this(provider)
        {
            foreach (var instance in toolsInstances)
            {
                var methods = instance.GetType()
                    .GetMethods()
                    .Where(method => method.GetCustomAttributes(typeof(AgentToolAttribute), false).Any())
                    .ToDictionary(
                        method => method.Name,
                        m => ToolDelegateFactory.CreateFromMethodInfo(m, instance));

                foreach (var kvp in methods)
                {
                    _tools[kvp.Key] = kvp.Value;
                }
            }

            foreach (var instance in toolsInstances)
            {
                var toolDefinitions = ToolDefinitionGenerator.GenerateFromObject(instance);
                _toolDefinitions.AddRange(toolDefinitions);
            }
        }

        public async Task<string> RunAsync(string prompt)
        {
            this._history.Clear();

            this._history.Add(new ChatMessage
            {
                Role = "system",
                Content = SystemPrompt
            });

            this._history.Add(new ChatMessage
            {
                Role = "user",
                Content = prompt
            });

            for (var i = 0; i < 5; i++)
            {
                // 1. Send query to the model with tools
                var response = await provider.GetChatCompletionsAsync(_history, _toolDefinitions);

                var choice = response.Choices.First();

                // 2. Add model's (assistant) response to history
                _history.Add(new ChatMessage
                {
                    Role = choice.Message.Role,
                    Content = choice.Message.Content,
                    ToolCalls = choice.Message.ToolCalls!.Select(call => new ToolCall
                    {
                        Id = call.Id!,
                        Type = call.Type!,
                        Function = new ChatMessageFunctionCall
                        {
                            Arguments = call.Function!.Arguments,
                            Name = call.Function.Name
                        }
                    }).ToList()

                });

                // 3. Check if model wants to finish (FinishReason == "stop")
                if (choice.FinishReason == "stop")
                {
                    return choice.Message.Content;
                }

                // 4. Handle Tool Calling (FinishReason == "tool_calls")
                if (choice.FinishReason == "tool_calls")
                {
                    foreach (var toolCall in choice.Message.ToolCalls!)
                    {
                        Console.WriteLine($"[Agent]: Calls tool: {toolCall.Function!.Name}...");

                        var result = await _tools[toolCall.Function!.Name](toolCall.Function.Arguments);

                        // 5. Add tool result to history with role "tool" and ToolCallId
                        _history.Add(new ChatMessage()
                        {
                            Role = "tool",
                            Content = result,
                            ToolCallId = toolCall.Id
                        });
                    }
                }
            }

            return "Iteration limit reached without final answer.";
        }
    }
}
