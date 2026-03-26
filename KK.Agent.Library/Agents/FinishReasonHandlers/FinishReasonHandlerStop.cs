using KK.Agent.Library.Clients.OpenApi.V1;

namespace KK.Agent.Library.Agents.FinishReasonHandlers
{
    public class FinishReasonHandlerStop : IFinishReasonHandler
    {
        public bool Handles(string finishReason) => finishReason == "stop";

        public Task<string?> HandleAsync(ChatCompletionChoice choice)
        {
            return Task.FromResult(choice.Message?.Content);
        }
    }
}
