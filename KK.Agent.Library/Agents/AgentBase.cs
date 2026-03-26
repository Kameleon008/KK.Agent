using KK.Agent.Library.Agents.FinishReasonHandlers;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Clients.OpenApi.V1;
using KK.Agent.Library.Extensions;
using KK.Agent.Library.Tools;

namespace KK.Agent.Library.Agents
{
    public abstract class AgentBase(OpenApiClient provider)
    {
        private readonly ChatHistory _history = [];
        private readonly List<ToolDefinition> _toolDefinitions = [];
        private readonly Dictionary<string, Func<string, Task<string>>> _tools = new();
        private readonly List<IFinishReasonHandler> _handlers = [];

        protected virtual string SystemPrompt { get; set; } = "You are helpful AI assistant";

        protected AgentBase(OpenApiClient provider, params object[] toolsInstances) : this(provider)
        {
            foreach (var instance in toolsInstances)
            {
                var toolDefinitions = ToolDefinitionGenerator.GenerateFromObject(instance);
                _toolDefinitions.AddRange(toolDefinitions);

                var tools = ToolGenerator.GenerateFromObject(instance);
                _tools.AddRange(tools);
            }

            _handlers.Add(new FinishReasonHandlerStop());
            _handlers.Add(new FinishReasonHandlerLength());
            _handlers.Add(new FinishReasonHandlerToolCalls(_tools));
            _handlers.Add(new FinishReasonHandlerContentFilter());
        }

        public async Task<string> RunAsync(string prompt)
        {
            this._history.Clear();
            this._history.AddSystemMessage(SystemPrompt);
            this._history.AddUserMessage(prompt);

            foreach (var _ in Enumerable.Range(0, 5))
            {
                var response = await provider.GetChatCompletionsAsync(_history, _toolDefinitions);
                var choice = response.Choices.First();

                _history.AddMessage(choice);

                var result =  await _handlers
                    .Single(handler => handler.Handles(choice.FinishReason))
                    .HandleAsync(choice, _history);

                if (result == null)
                {
                    continue;
                }

                return result;
            }

            return "Iteration limit reached without final answer.";
        }
    }
}
