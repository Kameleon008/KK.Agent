using KK.Agent.Library.Agents;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Configuration.Models;
using KK.Agent.WebAPI.Agents;
using KK.Agent.WebAPI.Extensions;
using KK.Agent.WebAPI.Tools;

namespace KK.Agent.WebAPI;
public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddSingleton<ConfigProvider>(_ =>
        {
            var config = new ConfigProvider();
            builder.Configuration.GetSection("Provider").Bind(config);
            return config;
        });

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


        builder.Services.AddSingleton<OrchestratorTools>();
        builder.Services.AddTransient<OpenApiClient>();
        builder.Services.AddSingleton<AgentLogger>();
        builder.Services.AddTransient<OrchestratorAgent>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
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