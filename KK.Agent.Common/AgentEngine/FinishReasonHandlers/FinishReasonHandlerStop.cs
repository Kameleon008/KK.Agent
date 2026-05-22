using KK.Agent.Common.Clients.OpenApi.V1;

namespace KK.Agent.Common.AgentEngine.FinishReasonHandlers
{
    public class FinishReasonHandlerStop : IFinishReasonHandler
    {
        public bool Handles(string finishReason) => finishReason == "stop";

        public Task<string?> HandleAsync(string caller, ChatCompletionChoice choice, ChatHistory history)
        {
            return Task.FromResult(choice.Message?.Content);
        }
    }
}
