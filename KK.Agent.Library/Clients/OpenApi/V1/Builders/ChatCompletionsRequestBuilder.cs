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

        public ChatCompletionsRequestBuilder AddMessages(IEnumerable<ChatMessage> messages)
        {
            _request.Messages = messages.ToList<ChatMessage>();

            return this;
        }

        public ChatCompletionsRequestBuilder AddMessage(string role, string content)
        {
            var message = new ChatMessage()
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

        public ChatCompletionsRequestBuilder SetMaxTokens(int maxTokens)
        {
            _request.MaxTokens = maxTokens;
            return this;
        }

        public ChatCompletionsRequestBuilder SetStop(params string[]? stopSequences)
        {
            _request.Stop = stopSequences?.ToList();
            return this;
        }

        public ChatCompletionsRequestBuilder SetResponseFormat(string type)
        {
            _request.ResponseFormat = new ChatCompletionResponseFormat { Type = type };
            return this;
        }

        public ChatCompletionsRequestBuilder SetStream(bool stream)
        {
            _request.Stream = stream;
            return this;
        }

        public ChatCompletionsRequestBuilder SetTools(params ToolDefinition[]? tools)
        {
            _request.Tools = tools?.ToList();
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