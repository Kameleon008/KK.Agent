using KK.Agent.Library.Clients.OpenApi.V1.Builders;
using KK.Agent.Library.Examples.Models;

namespace TestSchemaGeneration;

class Program
{
    static void Main(string[] args)
    {
        var builder = new ChatCompletionsRequestBuilder();
        builder.SetJsonResponseFormat<CharacterWithPlanetInfo>();
        
        Console.WriteLine(builder.BuildToString());
    }
}
