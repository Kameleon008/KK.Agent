using KK.Agent.Common.AgentEngine;

namespace KK.Agent.Common
{
    public class ChatHistoryProvider : IChatHistoryProvider
    {
        private readonly Dictionary<string, ChatHistory> _chats = new();

        public ChatHistory GetChatHistory(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId) is false)
            {
                if (this._chats.TryGetValue(sessionId, out var chatHistory))
                {
                    return chatHistory;
                }

                this._chats.Add(sessionId, []);

                return this._chats[sessionId];
            }

            return new ChatHistory();
        }
    }
}
