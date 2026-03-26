using KK.Agent.Library.Clients.OpenApi.V1;

namespace KK.Agent.Library.Agents.FinishReasonHandlers
{
    public class FinishReasonHandlerLength : IFinishReasonHandler
    {
        public bool Handles(string finishReason) => finishReason == "length";

        public Task<string?> HandleAsync(ChatCompletionChoice choice, ChatHistory history)
        {
            Console.WriteLine($"[Agent]: Maximum number of tokens specified in the request was reached...");
            return Task.FromResult(choice.Message?.Content);
        }
    }
}
