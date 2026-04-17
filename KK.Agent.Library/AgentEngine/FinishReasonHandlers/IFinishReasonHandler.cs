using KK.Agent.Library.Clients.OpenApi.V1;

namespace KK.Agent.Library.Agents.FinishReasonHandlers
{
    public interface IFinishReasonHandler
    {
        bool Handles(string finishReason);

        Task<string?> HandleAsync(string caller, ChatCompletionChoice choice, ChatHistory history);
    }
}
