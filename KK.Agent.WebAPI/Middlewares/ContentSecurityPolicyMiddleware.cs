namespace KK.Agent.WebAPI.Middlewares
{
    public class ContentSecurityPolicyMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var policy = "default-src 'self' data:; " +
                         "connect-src 'self' https://localhost:7084 https://localhost:3000 ws: wss:; " +
                         "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
                         "style-src 'self' 'unsafe-inline';";

            context.Response.Headers["Content-Security-Policy"] = policy;
            await next(context);
        }
    }
}
