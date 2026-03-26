using KK.Agent.Library.Clients.OpenApi.V1;

namespace KK.Agent.Library.Agents
{
    public class ChatHistory : List<ChatMessage>
    {

        public void AddMessage(string role, string content)
        {
            this.Add(new ChatMessage
            {
                Role = role,
                Content = content
            });
        }

        public void AddUserMessage(string content)
        {
            this.AddMessage("user", content);
        }

        public void AddSystemMessage(string content)
        {
            this.AddMessage("system", content);
        }

        public void AddMessage(ChatCompletionChoice choice)
        {
            this.Add(new ChatMessage
            {
                Role = choice.Message.Role,
                Content = choice.Message.Content,
                ToolCalls = choice.Message.ToolCalls!.Select(call => new ToolCall
                {
                    Id = call.Id!,
                    Type = call.Type!,
                    Function = new ChatMessageFunctionCall
                    {
                        Arguments = call.Function!.Arguments,
                        Name = call.Function.Name
                    }
                }).ToList()
            });
        }
    }
}
