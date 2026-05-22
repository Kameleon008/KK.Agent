using KK.Agent.Common.Clients.OpenApi.V1;

namespace KK.Agent.Common.AgentEngine.FinishReasonHandlers
{
    public interface IFinishReasonHandler
    {
        bool Handles(string finishReason);

        Task<string?> HandleAsync(string caller, ChatCompletionChoice choice, ChatHistory history);
    }
}
