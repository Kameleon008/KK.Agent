namespace KK.Agent.Library.Clients
{
    internal interface IChatCompletions
    {
        public Task<string> GetChatCompletionsAsync();
    }
}
