using KK.Agent.Library.Agents;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Configuration;
using KK.Agent.Library.Configuration.Models;
using KK.Agent.Library.Entities.Examples;

namespace KK.Agent.ConsoleClient;

public static class Program
{
    public static async Task Main(string[] args)
    {
        ConfigService.Load();

        var config = ConfigService.Get<ConfigRoot>();

        var openApiClient = new OpenApiClient(config.Provider);
        var agentConfig = new CognitiveAgentConfig();

        var agent = new CognitiveAgent(agentConfig, openApiClient, new LoreDatabaseTools());

        var response = await agent.RunAsync("jaka pogoda we wro wariacie? Co słychać na wikipedii?");

        Console.WriteLine(response);


        //Console.WriteLine(chat.Choices.First().Message.Content);

        //await foreach (var word in openApiClient.GetChatCompletionsStreamAsync([]))
        //{
        //    Console.Write(word); // You'll see the AI "typing" in real-time
        //}

        await Task.Delay(100);
    }
}