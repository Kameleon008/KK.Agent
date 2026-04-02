using KK.Agent.WebAPI.Middlewares;

namespace KK.Agent.WebAPI.Extensions
{
    public static class ContentSecurityPolicyMiddlewareExtension
    {
        public static IApplicationBuilder UseContentSecurityPolicy(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ContentSecurityPolicyMiddleware>();
        }
    }
}
