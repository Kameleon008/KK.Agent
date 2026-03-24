using Newtonsoft.Json;

namespace KK.Agent.Library.Clients.OpenApi.Models.V1
{
    public class ModelsResponse
    {
        [JsonProperty("data")]
        public IEnumerable<Model> Data { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; } 
    }

    public class Model
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("owned_by")]
        public string OwnedBy { get; set; }
    }
}
