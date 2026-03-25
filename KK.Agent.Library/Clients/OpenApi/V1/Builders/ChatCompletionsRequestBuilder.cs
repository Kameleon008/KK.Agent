using KK.Agent.Library.Clients.OpenApi.V1;
using Newtonsoft.Json;
using System.Text;

namespace KK.Agent.Library.Clients.OpenApi.V1.Builders
{

    public class ChatCompletionsRequestBuilder
    {
        private readonly ChatCompletionsRequest _request = new();

        public ChatCompletionsRequestBuilder SetModel(string model)
        {
            _request.Model = model;
            return this;
        }

        public ChatCompletionsRequestBuilder AddMessages(IEnumerable<ChatCompletionsRequest.ChatMessage> messages)
        {
            _request.Messages = messages.ToList<ChatCompletionsRequest.ChatMessage>();

            return this;
        }

        public ChatCompletionsRequestBuilder AddMessage(string role, string content)
        {
            var message = new ChatCompletionsRequest.ChatMessage()
            {
                Role = role,
                Content = content
            };

            _request.Messages ??= [];
            _request.Messages.Add(message);

            return this;
        }

        public ChatCompletionsRequestBuilder SetTemperature(double temperature)
        {
            _request.Temperature = temperature;
            return this;
        }

        public ChatCompletionsRequestBuilder SetTopP(double topP)
        {
            _request.TopP = topP;
            return this;
        }

        public ChatCompletionsRequestBuilder SetMaxTokens(int maxTokens)
        {
            _request.MaxTokens = maxTokens;
            return this;
        }

        public ChatCompletionsRequestBuilder SetN(int n)
        {
            _request.N = n;
            return this;
        }

        public ChatCompletionsRequestBuilder SetPresencePenalty(double penalty)
        {
            _request.PresencePenalty = penalty;
            return this;
        }

        public ChatCompletionsRequestBuilder SetFrequencyPenalty(double penalty)
        {
            _request.FrequencyPenalty = penalty;
            return this;
        }

        public ChatCompletionsRequestBuilder SetLogitBias(Dictionary<string, int> bias)
        {
            _request.LogitBias = bias;
            return this;
        }

        public ChatCompletionsRequestBuilder SetStop(params string[]? stopSequences)
        {
            _request.Stop = stopSequences?.ToList();
            return this;
        }

        public ChatCompletionsRequestBuilder SetResponseFormat(string type)
        {
            _request.ResponseFormat = new ChatCompletionsRequest.ChatCompletionResponseFormat { Type = type };
            return this;
        }

        public ChatCompletionsRequestBuilder SetStream(bool stream)
        {
            _request.Stream = stream;
            return this;
        }

        public ChatCompletionsRequestBuilder SetFunctions(params ChatCompletionsRequest.FunctionDefinition[]? functions)
        {
            _request.Functions = functions?.ToList();
            return this;
        }

        public ChatCompletionsRequestBuilder SetFunctionCall(string functionCall)
        {
            _request.FunctionCall = functionCall;
            return this;
        }

        public ChatCompletionsRequestBuilder SetSeed(int seed)
        {
            _request.Seed = seed;
            return this;
        }

        public ChatCompletionsRequestBuilder SetUser(string user)
        {
            _request.User = user;
            return this;
        }

        public ChatCompletionsRequest Build()
        {
            return _request;
        }

        public string BuildToString()
        {
            return JsonConvert.SerializeObject(this._request, Formatting.Indented);
        }

        public StringContent BuildToHttpContent()
        {
            var jsonStringContent = JsonConvert.SerializeObject(this._request, Formatting.Indented);
            return new StringContent(jsonStringContent, Encoding.UTF8, "application/json");
        }
    }
}