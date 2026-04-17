using KK.Agent.Library.Agents;

namespace KK.Agent.Library
{
    public interface IChatHistoryProvider
    {
        public ChatHistory GetChatHistory(string sessionId);
    }
}
