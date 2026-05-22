using KK.Agent.Common.AgentEngine;

namespace KK.Agent.Common
{
    public interface IChatHistoryProvider
    {
        public ChatHistory GetChatHistory(string sessionId);
    }
}
