namespace KK.Agent.Library.AgentEngine
{
    public class AgentResponse<T>(string agent, T response, ChatHistory history)
        where T : class
    {
        public string Agent { get; set; } = agent;

        public T Response { get; set; } = response;

        public ChatHistory History { get; set; } = history;

        public AgentResponse(string agent, T response) : this(agent, response, [])
        {

        }
    }

    public class AgentResponse
    {
        public string Agent { get; set; } = string.Empty;

        public string SessionId { get; set; } = string.Empty;

        public string Response { get; set; }

        public ChatHistory History { get; set; } = [];
    }
}
