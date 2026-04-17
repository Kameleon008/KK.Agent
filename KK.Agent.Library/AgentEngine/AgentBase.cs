using KK.Agent.Library.Agents.FinishReasonHandlers;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Clients.OpenApi.V1;
using KK.Agent.Library.Clients.OpenApi.V1.Builders;
using KK.Agent.Library.Extensions;
using KK.Agent.Library.Tools;
using Newtonsoft.Json;
using System.Text;
using KK.Agent.Library.Mcp;

namespace KK.Agent.Library.Agents
{
    public abstract class AgentBase
    {
        protected readonly OpenApiClient _provider;
        protected readonly List<ToolDefinition> _toolDefinitions = [];
        protected readonly Dictionary<string, Func<string, Task<string>>> _tools = new();
        protected readonly List<IFinishReasonHandler> _handlers = [];
        protected readonly AgentLogger _logger;
        protected readonly ConfigMcpServers _mcpServers;
        protected readonly List<McpClient> _mcpClients = [];
        protected readonly AgentToolsProvider _toolsProvider;

        protected virtual string AgentId { get; set; } = Guid.NewGuid().ToString();

        protected virtual string SystemPrompt { get; set; } = "You are helpful AI assistant";

        protected AgentBase(OpenApiClient provider, AgentLogger logger, ConfigMcpServers mcpServers)
        {
            this._logger = logger;
            this._provider = provider;
            this._mcpServers = mcpServers;
            this._handlers =
            [
                new FinishReasonHandlerStop(),
                new FinishReasonHandlerLength(),
                new FinishReasonHandlerToolCalls(_tools, logger, _mcpClients),
                new FinishReasonHandlerContentFilter(),
            ];

            this._toolsProvider = new AgentToolsProvider();
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

        public void AddMcpServer(string name)
        {
            var config = this._mcpServers.Servers.SingleOrDefault(s => s.Name == name);
            if (config != null)
            {
                var mcpClient = new McpClient(config);
                _mcpClients.Add(mcpClient);
            }
        }

        public async Task<string> AskAgentAsync(ChatHistory history)
        {
            InitializeChatHistory(history);

            foreach (var _ in Enumerable.Range(0, 5))
            {
                var request = new ChatCompletionsRequestBuilder()
                    .SetModel(_provider.Model)
                    .SetMessages(history)
                    .SetTools(_toolDefinitions)
                    .Build();

                var response = await _provider.GetChatCompletionsAsync(request);
                var choice = response.Choices.First();

                history.AddMessage(choice);

                var result = await _handlers
                    .Single(handler => handler.Handles(choice.FinishReason))
                    .HandleAsync(AgentId, choice, history);

                if (result == null)
                {
                    continue;
                }

                return result;
            }

            return "Iteration limit reached without final answer.";
        }

        public async Task<T?> AskAgentAsync<T>(ChatHistory history)
            where T : class, new()
        {
            InitializeChatHistory(history);

            await this.AskAgentAsync(history);

            var request = new ChatCompletionsRequestBuilder()
                .SetModel(_provider.Model)
                .SetMessages(history)
                .SetTools(_toolDefinitions)
                .SetJsonResponseFormat<T>()
                .Build();

            var response = await _provider.GetChatCompletionsAsync(request);
            var choice = response.Choices.First();

            history.AddMessage(choice);

            var result = await _handlers
                .Single(handler => handler.Handles(choice.FinishReason))
                .HandleAsync(AgentId, choice, history);

            return result == null ? null : JsonConvert.DeserializeObject<T>(result);
        }

        public async Task<string> AskAgentStream(ChatHistory history)
        {
            InitializeChatHistory(history);

            foreach (var client in this._mcpClients)
            {
                await client.LoadToolsAsync(this._toolDefinitions);
            }

            foreach (var _ in Enumerable.Range(0, 5))
            {
                ChatCompletionsResponse? synthesizedResponse = null;

                var request = new ChatCompletionsRequestBuilder()
                    .SetModel(_provider.Model)
                    .SetMessages(history)
                    .SetTools(_toolDefinitions)
                    .SetStream(true)
                    .Build();

                var fullContent = new StringBuilder();

                await foreach (var chunk in _provider.GetChatCompletionsStreamAsync(request))
                {
                    var choice = chunk.Choices?.FirstOrDefault();

                    if (choice?.Delta == null)
                    {
                        continue;
                    }


                    fullContent.Append(choice.Delta.ReasoningContent);
                    fullContent.Append(choice.Delta.Content);

                    await _logger.PublishAsync(
                        agentId: AgentId,
                        reasoning: choice.Delta.ReasoningContent,
                        content: choice.Delta.Content);

                    UpdateChatCompletionsResponseFromChunk(ref synthesizedResponse, chunk);
                }

                if (synthesizedResponse != null)
                {
                    history.AddMessage(synthesizedResponse.Choices.Single());

                    var result = await _handlers
                        .Single(h => h.Handles(synthesizedResponse.Choices.Single().FinishReason))
                        .HandleAsync(AgentId, synthesizedResponse.Choices.Single(), history);

                    if (result == null) continue;

                    return result;
                }
            }

            return "Iteration limit reached without final answer.";
        }


        private void RegisterTools(object instance)
        {
            var toolDefinitions = ToolDefinitionGenerator.GenerateFromObject(instance);
            _toolDefinitions.AddRange(toolDefinitions);

            var tools = ToolGenerator.GenerateFromObject(instance);
            _tools.AddRange(tools);
        }

        private void InitializeChatHistory(ChatHistory history)
        {
            if (history.Any() is false)
            {
                history.AddSystemMessage(SystemPrompt);
            }

            if (history.First().Role != "system")
            {
                history.Insert(0, new ChatMessage { Role = "system", Content = SystemPrompt });
            }
        }

        protected static void UpdateChatCompletionsResponseFromChunk(ref ChatCompletionsResponse? response, ChatCompletionsChunk chunk)
        {
            response ??= new ChatCompletionsResponse
            {
                Id = chunk.Id,
                Choices = [new ChatCompletionChoice { Message = new ChatCompletionMessage { Content = "" } }]
            };

            var choice = chunk.Choices?.FirstOrDefault();
            var message = response.Choices?.FirstOrDefault()?.Message;

            if (choice == null || message == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(choice.Delta.Role))
            {
                message.Role = choice.Delta.Role;
            }

            if (!string.IsNullOrEmpty(choice.Delta.Content))
            {
                message.Content += choice.Delta.Content;
            }

            if (!string.IsNullOrEmpty(choice.Delta.ReasoningContent))
            {
                message.ReasoningContent += choice.Delta.ReasoningContent;
            }

            if (choice.Delta.ToolCalls != null)
            {
                message.ToolCalls ??= [];

                foreach (var toolDelta in choice.Delta.ToolCalls)
                {
                    while (message.ToolCalls.Count <= toolDelta.Index)
                    {
                        message.ToolCalls.Add(new ChatCompletionToolCall { Function = new ChatCompletionToolCallFunction() { Arguments = "" } });
                    }

                    var existingTool = message.ToolCalls[toolDelta.Index];

                    if (!string.IsNullOrEmpty(toolDelta.Id))
                        existingTool.Id = toolDelta.Id;

                    if (!string.IsNullOrEmpty(toolDelta.Type))
                        existingTool.Type = toolDelta.Type;

                    if (toolDelta.Function != null && !string.IsNullOrEmpty(toolDelta.Function.Name))
                        existingTool.Function?.Name += toolDelta.Function.Name;

                    if (toolDelta.Function != null && !string.IsNullOrEmpty(toolDelta.Function.Arguments))
                        existingTool.Function?.Arguments += toolDelta.Function.Arguments;
                }
            }

            if (!string.IsNullOrEmpty(choice.FinishReason))
            {
                response.Choices?.Single().FinishReason = choice.FinishReason;
            }
        }
    }
}
