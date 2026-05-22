using KK.Agent.Common.Clients.OpenApi.V1;

namespace KK.Agent.Common.AgentEngine.FinishReasonHandlers
{
    public class FinishReasonHandlerContentFilter : IFinishReasonHandler
    {
        public bool Handles(string finishReason) => finishReason == "content_filter";

        public Task<string?> HandleAsync(string caller, ChatCompletionChoice choice, ChatHistory history)
        {
            Console.WriteLine($"[Agent]: Content was omitted due to a flag from our content filters...");
            return Task.FromResult(choice.Message?.Content);
        }
    }
}
