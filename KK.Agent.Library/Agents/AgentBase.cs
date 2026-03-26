using System.Reflection;
using KK.Agent.Library.Agents.FinishReasonHandlers;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Clients.OpenApi.V1;
using KK.Agent.Library.Clients.OpenApi.V1.Builders;
using KK.Agent.Library.Extensions;
using KK.Agent.Library.Tools;
using Newtonsoft.Json;

namespace KK.Agent.Library.Agents
{
    public abstract class AgentBase
    {
        private readonly OpenApiClient _provider;
        private readonly ChatHistory _history = [];
        private readonly List<ToolDefinition> _toolDefinitions = [];
        private readonly Dictionary<string, Func<string, Task<string>>> _tools = new();
        private readonly List<IFinishReasonHandler> _handlers = [];

        protected virtual string SystemPrompt { get; set; } = "You are helpful AI assistant";

        protected AgentBase(OpenApiClient provider)
        {
            this._provider = provider;
            this.InitializeHandlers();
        }

        private void InitializeHandlers()
        {
            _handlers.Add(new FinishReasonHandlerStop());
            _handlers.Add(new FinishReasonHandlerLength());
            _handlers.Add(new FinishReasonHandlerToolCalls(_tools));
            _handlers.Add(new FinishReasonHandlerContentFilter());
        }

        public void AddToolFromType<T>() where T : class, new()
        {
            var instance = new T();
            RegisterTools(instance);
        }

        public void AddToolInstance(object toolInstance)
        {
            RegisterTools(toolInstance);
        }

        public async Task<string> RunAsync(string prompt)
        {
            this._history.Clear();
            this._history.AddSystemMessage(SystemPrompt);
            this._history.AddUserMessage(prompt);

            foreach (var _ in Enumerable.Range(0, 5))
            {
                var request = new ChatCompletionsRequestBuilder()
                    .SetModel(_provider.Model)
                    .SetMessages(_history)
                    .SetTools(_toolDefinitions)
                    .Build();

                var response = await _provider.GetChatCompletionsAsync(request);
                var choice = response.Choices.First();

                _history.AddMessage(choice);

                var result = await _handlers
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

        public async Task<T?> RunAsync<T>(string prompt)
            where T : class, new()
        {
            await this.RunAsync(prompt);

            var request = new ChatCompletionsRequestBuilder()
                .SetModel(_provider.Model)
                .SetMessages(_history)
                .SetTools(_toolDefinitions)
                .SetJsonResponseFormat<T>()
                .Build();

            var body = request.ToString();

            var response = await _provider.GetChatCompletionsAsync(request);
            var choice = response.Choices.First();

            _history.AddMessage(choice);

            var result = await _handlers
                .Single(handler => handler.Handles(choice.FinishReason))
                .HandleAsync(choice, _history);

            return JsonConvert.DeserializeObject<T>(result);
        }



        private void RegisterTools(object instance)
        {
            var toolDefinitions = ToolDefinitionGenerator.GenerateFromObject(instance);
            _toolDefinitions.AddRange(toolDefinitions);

            var tools = ToolGenerator.GenerateFromObject(instance);
            _tools.AddRange(tools);
        }
    }
}
