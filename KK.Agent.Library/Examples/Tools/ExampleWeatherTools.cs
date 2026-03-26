using KK.Agent.Library.Attributes;

namespace KK.Agent.Library.Examples.Tools
{
    public class ExampleWeatherTools
    {
        [AgentTool("Get weather information for a specific location.")]
        public string get_weather(string city)
        {
            return $"{city}, {new Random().NextInt64(5, 20)}°C";
        }
    }
}
