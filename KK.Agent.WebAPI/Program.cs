using KK.Agent.Common;
using KK.Agent.Common.AgentEngine;
using KK.Agent.Common.Configuration;
using KK.Agent.WebAPI.Extensions;

namespace KK.Agent.WebAPI;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        
        builder.Services.AddOpenApi();

        builder.Services.AddOptions<ConfigAgents>().Bind(builder.Configuration);

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        builder.Services.AddSingleton<ChatHistoryProvider>();

        builder.Services.AddScoped<AgentLogger>();
        builder.Services.AddScoped<AgentsFactory>();

        builder.Services.AddSingleton<IChatHistoryProvider, ChatHistoryProvider>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseContentSecurityPolicy();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}