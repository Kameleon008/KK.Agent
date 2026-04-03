using KK.Agent.Library.Agents;

namespace KK.Agent.Library
{
    public class AgentHistory
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
