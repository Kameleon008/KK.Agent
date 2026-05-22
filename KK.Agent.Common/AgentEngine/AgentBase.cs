using System.Text;
using KK.Agent.Common.AgentEngine.FinishReasonHandlers;
using KK.Agent.Common.Clients;
using KK.Agent.Common.Clients.OpenApi.V1;
using KK.Agent.Common.Clients.OpenApi.V1.Builders;
using KK.Agent.Common.Configuration;
using KK.Agent.Common.Tools;

namespace KK.Agent.Common.AgentEngine
{
    public abstract class AgentBase(IApiProviderClient client, ToolsProvider tools, ConfigAgent configuration, AgentLogger logger)
    {
        protected readonly IApiProviderClient Client = client;
        protected readonly ToolsProvider Tools = tools;
        protected readonly AgentLogger Logger = logger;

        protected readonly List<IFinishReasonHandler> Handlers =
        [
            new FinishReasonHandlerStop(),
            new FinishReasonHandlerLength(),
            new FinishReasonHandlerToolCalls(tools, logger),
            new FinishReasonHandlerContentFilter(),
        ];
        
        protected virtual string AgentId { get; set; } = Guid.NewGuid().ToString();

        protected virtual string SystemPrompt { get; set; } = "You are helpful AI assistant";

        public async Task<string> AskAgentAsync(ChatHistory history)
        {
            InitializeChatHistory(history);

            foreach (var _ in Enumerable.Range(0, 5))
            {
                var request = new ChatCompletionsRequestBuilder()
                    .SetModel(Client.Model)
                    .SetMessages(history)
                    .SetTools(Tools.ToolDefinitions)
                    .SetTemperature(configuration.Temperature)
                    .SetReasoningEffort(configuration.ReasoningEffort)
                    .Build();

                var response = await Client.GetChatCompletionsAsync(request);
                var choice = response.Choices.Single();

                history.AddMessage(choice);

                var result = await Handlers
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

        public async Task<string> AskAgentStream(ChatHistory history)
        {
            InitializeChatHistory(history);

            foreach (var client in Tools.McpClients)
            {
                await client.LoadToolsAsync(Tools.ToolDefinitions);
            }

            foreach (var _ in Enumerable.Range(0, 100))
            {
                ChatCompletionsResponse? synthesizedResponse = null;

                var request = new ChatCompletionsRequestBuilder()
                    .SetModel(Client.Model)
                    .SetMessages(history)
                    .SetTools(Tools.ToolDefinitions)
                    .SetStream(true)
                    .SetTemperature(configuration.Temperature)
                    .SetReasoningEffort(configuration.ReasoningEffort)
                    .Build();

                var fullContent = new StringBuilder();

                await foreach (var chunk in Client.GetChatCompletionsStreamAsync(request))
                {
                    var choice = chunk.Choices?.FirstOrDefault();

                    if (choice?.Delta == null)
                    {
                        continue;
                    }

                    fullContent.Append(choice.Delta.ReasoningContent);
                    fullContent.Append(choice.Delta.Content);

                    await Logger.PublishAsync(
                        agentId: AgentId,
                        reasoning: choice.Delta.ReasoningContent,
                        content: choice.Delta.Content);

                    UpdateChatCompletionsResponseFromChunk(ref synthesizedResponse, chunk);
                }

                if (synthesizedResponse != null)
                {
                    history.AddMessage(synthesizedResponse.Choices.Single());

                    var result = await Handlers
                        .Single(h => h.Handles(synthesizedResponse.Choices.Single().FinishReason))
                        .HandleAsync(AgentId, synthesizedResponse.Choices.Single(), history);

                    if (result == null) continue;

                    return result;
                }
            }

            return "Iteration limit reached without final answer.";
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
