using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Clients.OpenApi.V1;

namespace KK.Agent.Library.Entities
{
    public class Agent
    {
        private OpenApiClient _llmService;
        private Configuration configuration;
        private List<ChatCompletionsRequest.ChatMessage> _history = [];
        private Dictionary<string, Func<string, string>> _tools;

        public Agent(Configuration configuration, OpenApiClient provider)
        {
            this._llmService = provider;
            this.configuration = configuration;

            this._tools = new Dictionary<string, Func<string, string>>()
            {
                { "get_weather", args => "Wrocław, 15°C, słonecznie" },
                { "search_wiki", args => "Agent AI to program wykonujący zadania autonomicznie." },
            };
        }

        public async Task<string> RunAsync(string prompt)
        {
            this._history.Add(new()
            {
                Role = "user",
                Content = prompt
            });

            for (int i = 0; i < 5; i++)
            {
                // 1. Wyślij zapytanie do modelu
                var response = await _llmService.GetChatCompletionsAsync(_history);

                var choice = response.Choices.First();

                // 2. Dodaj odpowiedź modelu (asystenta) do historii
                _history.Add(new ChatCompletionsRequest.ChatMessage()
                {
                    Role = choice.Message.Role,
                    Content = choice.Message.Content,
                    
                });

                // 3. Sprawdź, czy model chce zakończyć (FinishReason == "stop")
                if (choice.FinishReason == "stop")
                {
                    return choice.Message.Content;
                }

                // 4. Obsługa Tool Calling (FinishReason == "tool_calls")
                if (choice.FinishReason == "tool_calls")
                {
                    foreach (var toolCall in choice.Message.ToolCalls)
                    {
                        Console.WriteLine($"[Agent]: Wywołuję narzędzie {toolCall.Function.Name}...");

                        // Wykonaj logikę narzędzia
                        string result = _tools[toolCall.Function.Name](toolCall.Function.Arguments);

                        // 5. Dodaj wynik narzędzia do historii z rolą "tool" i ToolCallId
                        _history.Add(new ChatCompletionsRequest.ChatMessage()
                        {
                            Role = "tool",
                            Content = result,
                        });
                    }

                    // Pętla kontynuuje działanie – w następnej iteracji wyślemy wyniki do LLM
                    continue;
                }
            }

            return "Osiągnięto limit iteracji bez finalnej odpowiedzi.";
        }
    }
}
