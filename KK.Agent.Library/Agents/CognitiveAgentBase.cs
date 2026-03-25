using KK.Agent.Library.Attributes;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Clients.OpenApi.V1;
using KK.Agent.Library.Tools;
using System.Reflection;

namespace KK.Agent.Library.Agents
{
    public abstract class CognitiveAgentBase(OpenApiClient provider)
    {
        private readonly List<ChatMessage> _history = [];
        private readonly Dictionary<string, Func<string, Task<string>>> _tools = new ();
        private readonly object? _toolsInstances;

        private const string SystemPrompt = "You are helpful AI assistant";

        protected CognitiveAgentBase(OpenApiClient provider, object toolsInstance) : this(provider)
        {
            this._toolsInstances = toolsInstance;

            var methods = toolsInstance
                .GetType()
                .GetMethods()
                .Where(method => method.GetCustomAttributes(typeof(AgentToolAttribute), false).Any())
                .ToDictionary(method => method.Name, CreateDelegateFromMethodInfo);

            this._tools = methods;
        }

        private Func<string, Task<string>> CreateDelegateFromMethodInfo(MethodInfo method)
        {
            return async args =>
            {
                var parameters = method.GetParameters();
                var argDict = System.Text.Json.JsonDocument.Parse(args).RootElement.EnumerateObject().ToDictionary(p => p.Name, p => p.Value.ToString());

                var parameterValues = new object?[parameters.Length];
                for (var i = 0; i < parameters.Length; i++)
                {
                    var param = parameters[i];
                    if (argDict.TryGetValue(param.Name!, out var argValue) && !string.IsNullOrEmpty(argValue))
                    {
                        parameterValues[i] = Convert.ChangeType(argValue, param.ParameterType);
                    }
                    else if (param.HasDefaultValue)
                    {
                        parameterValues[i] = param.DefaultValue;
                    }
                }

                var result = method.Invoke(_toolsInstances, parameterValues);

                if (result is not Task task)
                {
                    return (string?)result ?? string.Empty;
                }

                await task;

                return (string?)task.GetType().GetProperty("Result")?.GetValue(task) ?? string.Empty;

            };
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
                var tools = ToolDefinitionGenerator.GenerateFromObject(this._toolsInstances);
                var response = await provider.GetChatCompletionsAsync(_history, tools);

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
