using KK.Agent.Library.AgentEngine;

namespace KK.Agent.Library
{
    public interface IChatHistoryProvider
    {
        public ChatHistory GetChatHistory(string sessionId);
    }
}
