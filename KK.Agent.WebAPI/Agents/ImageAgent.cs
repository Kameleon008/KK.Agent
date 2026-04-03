using KK.Agent.Library;
using KK.Agent.Library.Agents;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Clients.OpenApi.V1;
using KK.Agent.Library.Clients.OpenApi.V1.Builders;
using KK.Agent.WebAPI.Tools;
using System.Text;

namespace KK.Agent.WebAPI.Agents
{
    public class ImageAgent : AgentBase
    {
        protected override string SystemPrompt { get; set; } = File.ReadAllText($"./Agents/{nameof(ImageAgent)}.md");

        protected override string AgentId { get; set; } = nameof(ImageAgent);

        public ImageAgent(OpenApiClient provider, AgentLogger logger, AgentHistory history) : base(provider, logger, history)
        {
            this.AddToolInstance(new ImageTools(logger));
        }

        public void AddMessage(string role, string message, string sessionId = "")
        {
            var history = _history.GetChatHistory(sessionId);
            history.AddSystemMessage(SystemPrompt);
            history.AddMessage(role, message);
        }

        public async Task<string> RunStreamAsync(string prompt, string sessionId = "")
        {
            var history = _history.GetChatHistory(sessionId);

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

                history.AddMessage(synthesizedResponse!.Choices.Single());

                var result = await _handlers
                    .Single(h => h.Handles(synthesizedResponse.Choices.Single().FinishReason))
                    .HandleAsync(AgentId, synthesizedResponse.Choices.Single(), history);

                if (result == null) continue;

                return result;
            }

            return "Iteration limit reached without final answer.";
        }

    }
}
