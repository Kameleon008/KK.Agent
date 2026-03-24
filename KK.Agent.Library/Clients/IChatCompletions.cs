namespace KK.Agent.Library.Clients
{
    internal interface IChatCompletions
    {
        public Task<T> GetChatCompletionsAsync<T>();
    }
}
