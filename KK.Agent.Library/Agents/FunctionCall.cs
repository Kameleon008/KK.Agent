using KK.Agent.Library.Clients.OpenApi.V1;

namespace KK.Agent.Library.Agents
{
    internal class FunctionCall : ChatMessageFunctionCall
    {
        public object Name { get; set; }
        public string? Arguments { get; set; }
    }
}