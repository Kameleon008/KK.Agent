using KK.Agent.Library.Mcp;
using KK.Agent.WebAPI.Agents;

namespace KK.Agent.WebAPI.Startup
{
    public static class RegisterAgents
    {
        public static void AddAgents(this IServiceCollection services)
        {
            services.AddScoped<OrchestratorAgent>();
        }

        public static void AddMcpServers(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection(ConfigMcpServers.Name);
            var config = section.Get<ConfigMcpServers>();

            if (config == null)
            {
                return;
            }

            services.AddSingleton(config);

            foreach (var server in config.Servers)
            {
                services.AddScoped<McpClient>(sp => new McpClient(server));
            }
        }

    }
}
