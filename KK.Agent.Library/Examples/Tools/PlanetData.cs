using Newtonsoft.Json;

namespace KK.Agent.Library.Examples.Tools;

public class PlanetData
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("population")]
    public long Population { get; set; }

    [JsonProperty("climate")]
    public string Climate { get; set; } = string.Empty;

    [JsonProperty("terrain")]
    public string Terrain { get; set; } = string.Empty;

    [JsonProperty("gravity")]
    public string Gravity { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;
}
