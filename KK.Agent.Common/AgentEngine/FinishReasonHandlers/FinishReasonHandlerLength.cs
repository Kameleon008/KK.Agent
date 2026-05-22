using KK.Agent.Common.Clients.OpenApi.V1;

namespace KK.Agent.Common.AgentEngine.FinishReasonHandlers
{
    public class FinishReasonHandlerLength : IFinishReasonHandler
    {
        public bool Handles(string finishReason) => finishReason == "length";

        public Task<string?> HandleAsync(string caller, ChatCompletionChoice choice, ChatHistory history)
        {
            Console.WriteLine($"[Agent]: Maximum number of tokens specified in the request was reached...");
            return Task.FromResult(choice.Message?.Content);
        }
    }
}
