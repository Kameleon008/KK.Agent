using System.Reflection;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Clients.OpenApi.V1;
using KK.Agent.Library.Entities;

namespace KK.Agent.Library.Agents
{
    public class CognitiveAgent
    {
        private OpenApiClient _llmService;
        private CognitiveAgentConfig configuration;
        private List<ChatMessage> _history = [];
        private Dictionary<string, Func<string, Task<string>>> _tools;
        private object? _toolsInstance;

        /// <summary>
        /// Creates a new CognitiveAgent with dictionary-based tools (legacy)
        /// </summary>
        public CognitiveAgent(CognitiveAgentConfig configuration, OpenApiClient provider)
        {
            this._llmService = provider;
            this.configuration = configuration;
        }

        /// <summary>
        /// Creates a new CognitiveAgent with reflection-based tools from an instance
        /// </summary>
        public CognitiveAgent(CognitiveAgentConfig configuration, OpenApiClient provider, object toolsInstance) : this(configuration, provider)
        {
            _toolsInstance = toolsInstance;

            // Build tool dictionary from instance
            var methods = toolsInstance.GetType().GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(AgentToolAttribute), false).Any())
                .ToDictionary(
                    m => m.Name,
                    m => CreateDelegateFromMethodInfo(m)
                );

            _tools = methods;
        }

        private Func<string, Task<string>> CreateDelegateFromMethodInfo(MethodInfo method)
        {
            return async args =>
            {
                var parameters = method.GetParameters();
                var argDict = System.Text.Json.JsonDocument.Parse(args).RootElement.EnumerateObject().ToDictionary(p => p.Name, p => p.Value.ToString());

                var parameterValues = new object?[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
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

                // Use _toolsInstance as target for non-static method invocation
                var result = method.Invoke(_toolsInstance, parameterValues);

                if (result is Task task)
                {
                    await task;
                    return (string?)task.GetType().GetProperty("Result")?.GetValue(task) ?? string.Empty;
                }

                return (string?)result ?? string.Empty;
            };
        }

        private List<ToolDefinition> GetTools()
        {
            if (_toolsInstance != null)
            {
                // Generate from reflection attributes
                return ToolDefinitionGenerator.GenerateFromObject(_toolsInstance);
            }

            // For legacy dictionary-based tools
            return _tools.Keys.Select(toolName => new ToolDefinition
            {
                Type = "function",
                Function = new ToolDefinitionFunction()
                {
                    Name = toolName,
                    Description = $"Execute the {toolName} function to get results.",
                    Parameters = new ParametersSchema
                    {
                        Type = "object",
                        Properties = new Dictionary<string, PropertyDefinition>
                        {
                            { "args", new PropertyDefinition { Type = "string" } }
                        },
                        Required = new List<string> { "args" },
                        AdditionalProperties = false
                    },
                    Strict = true
                }
            }).ToList();
        }

        public async Task<string> RunAsync(string prompt)
        {
            this._history.Add(new()
            {
                Role = "user",
                Content = prompt
            });

            for (int i = 0; i < 5; i++)
            {
                // 1. Send query to the model with tools
                var tools = GetTools();
                var response = await _llmService.GetChatCompletionsAsync(_history, tools);

                var choice = response.Choices.First();

                // 2. Add model's (assistant) response to history
                _history.Add(new ChatMessage()
                {
                    Role = choice.Message.Role,
                    Content = choice.Message.Content,
                    ToolCalls = choice.Message.ToolCalls.Select(call => new ToolCall()
                    {
                        Id = call.Id,
                        Type = call.Type,
                        Function = new ChatMessageFunctionCall()
                        {
                            Arguments = call.Function.Arguments,
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
                    foreach (var toolCall in choice.Message.ToolCalls)
                    {
                        Console.WriteLine($"[Agent]: Calls tool: {toolCall.Function.Name}...");

                        string result = await _tools[toolCall.Function.Name](toolCall.Function.Arguments);

                        // 5. Add tool result to history with role "tool" and ToolCallId
                        _history.Add(new ChatMessage()
                        {
                            Role = "tool",
                            Content = result,
                            ToolCallId = toolCall.Id
                        });
                    }
                }

                // Loop continues - in next iteration we'll send results to LLM
                continue;
            }

            return "Iteration limit reached without final answer.";
        }
    }
}
