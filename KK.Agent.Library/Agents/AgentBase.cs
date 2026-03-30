using KK.Agent.Library.Agents.FinishReasonHandlers;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Clients.OpenApi.V1;
using KK.Agent.Library.Clients.OpenApi.V1.Builders;
using KK.Agent.Library.Extensions;
using KK.Agent.Library.Tools;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

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

            var response = await _provider.GetChatCompletionsAsync(request);
            var choice = response.Choices.First();

            _history.AddMessage(choice);

            var result = await _handlers
                .Single(handler => handler.Handles(choice.FinishReason))
                .HandleAsync(choice, _history);

            return JsonConvert.DeserializeObject<T>(result);
        }

        public async IAsyncEnumerable<string> RunStreamAsync(string prompt)
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
                    .SetStream(true)
                    .Build();

                StringBuilder fullContent = new StringBuilder();
                ChatCompletionsResponse synthesizedResponse = null;

                await foreach (var chunk in _provider.GetChatCompletionsStreamAsync(request))
                {
                    var choice = chunk.Choices.First();

                    if (choice.Delta.ReasoningContent != null)
                    {
                        fullContent.Append(choice.Delta.ReasoningContent);

                        yield return choice.Delta.ReasoningContent;
                    }

                    // Jeśli to zwykły tekst (content), wypychaj go od razu
                    if (choice.Delta.Content != null)
                    {
                        fullContent.Append(choice.Delta.Content);

                        yield return choice.Delta.Content;
                    }

                    // Zbieraj informacje o Tool Calls (jeśli występują w streamie)
                    // Stream zwraca Tool Calls w kawałkach, musisz je agregować w 'synthesizedResponse'
                    UpdateSynthesizedResponse(ref synthesizedResponse, chunk);
                }

                // Po zakończeniu streama, dodaj pełną odpowiedź do historii
                _history.AddMessage(synthesizedResponse.Choices.Single());

                // Obsługa FinishReason (np. wywołanie narzędzi)
                var result = await _handlers
                    .Single(h => h.Handles(synthesizedResponse.Choices.Single().FinishReason))
                    .HandleAsync(synthesizedResponse.Choices.Single(), _history);

                if (result == null) continue;

                var wasStreamingContent = fullContent.Length > 0;

                if (!wasStreamingContent && !string.IsNullOrEmpty(result))
                {
                    yield return result;
                }

                yield break;
            }
        }

        private void RegisterTools(object instance)
        {
            var toolDefinitions = ToolDefinitionGenerator.GenerateFromObject(instance);
            _toolDefinitions.AddRange(toolDefinitions);

            var tools = ToolGenerator.GenerateFromObject(instance);
            _tools.AddRange(tools);
        }


        private void UpdateSynthesizedResponse(ref ChatCompletionsResponse synthesized, ChatCompletionsChunk chunk)
        {
            if (synthesized == null)
            {
                synthesized = new ChatCompletionsResponse
                {
                    Id = chunk.Id,
                    Choices = new List<ChatCompletionChoice> { new ChatCompletionChoice { Message = new ChatCompletionMessage() { Content = "" } } }
                };
            }

            var choice = chunk.Choices[0];
            var message = synthesized.Choices[0].Message;

            // ✅ Role
            if (!string.IsNullOrEmpty(choice.Delta?.Role))
            {
                message.Role = choice.Delta.Role;
            }

            // Agregacja tekstu
            if (!string.IsNullOrEmpty(choice.Delta?.Content))
            {
                message.Content += choice.Delta.Content;
            }

            if (!string.IsNullOrEmpty(choice.Delta.ReasoningContent))
            {
                message.ReasoningContent += choice.Delta.ReasoningContent;
            }

            // Agregacja Tool Calls
            if (choice.Delta?.ToolCalls != null)
            {
                if (message.ToolCalls == null) message.ToolCalls = new List<ChatCompletionToolCall>();

                foreach (var toolDelta in choice.Delta.ToolCalls)
                {
                    // Upewnienie się, że lista ma odpowiedni rozmiar dla danego indeksu
                    while (message.ToolCalls.Count <= toolDelta.Index)
                    {
                        message.ToolCalls.Add(new ChatCompletionToolCall { Function = new ChatCompletionToolCallFunction() { Arguments = "" } });
                    }

                    var existingTool = message.ToolCalls[toolDelta.Index];

                    if (!string.IsNullOrEmpty(toolDelta.Id))
                        existingTool.Id = toolDelta.Id;

                    if (!string.IsNullOrEmpty(toolDelta.Type))
                        existingTool.Type = toolDelta.Type;

                    if (toolDelta.Function != null)
                    {
                        if (!string.IsNullOrEmpty(toolDelta.Function.Name))
                            existingTool.Function.Name += toolDelta.Function.Name;

                        if (!string.IsNullOrEmpty(toolDelta.Function.Arguments))
                            existingTool.Function.Arguments += toolDelta.Function.Arguments;
                    }
                }
            }

            // Zapisanie powodu zakończenia (pojawia się w ostatnim chunku)
            if (!string.IsNullOrEmpty(choice.FinishReason))
            {
                synthesized.Choices.Single().FinishReason = choice.FinishReason;
            }
        }

    }
}
