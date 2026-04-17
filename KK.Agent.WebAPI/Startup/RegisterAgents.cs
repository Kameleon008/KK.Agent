using KK.Agent.Library.Agents;
using KK.Agent.Library.Mcp;

namespace KK.Agent.WebAPI.Startup
{
    public static class RegisterAgents
    {
        public static void AddMcpServers(this IServiceCollection services, IConfiguration configuration)
        {

            var config = new ConfigMcpServers();
            var section = configuration.GetSection(ConfigMcpServers.Name);
            section.Bind(config.Servers);

            services.AddSingleton(config ?? new ConfigMcpServers());
;        }

    }
}
