using Microsoft.AspNetCore.Mvc;

namespace KK.Agent.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost("stream-chat")]
        public async Task StreamChat([FromBody] string prompt, CancellationToken ct)
        {
            // 1. Ustawienie nagłówków dla Streamingu
            Response.ContentType = "text/plain"; // Lub "text/event-stream" dla SSE

            var client = new ChatClient("gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

            // 2. Pobranie strumienia z OpenAI
            AsyncCollectionResult<StreamingChatCompletionUpdate> updates =
                client.CompleteChatStreamingAsync(prompt, cancellationToken: ct);

            // 3. Przekazywanie kawałków (chunks) do klienta Twojego API
            await foreach (var update in updates)
            {
                foreach (var contentPart in update.ContentUpdate)
                {
                    if (!string.IsNullOrEmpty(contentPart.Text))
                    {
                        // Pisanie bezpośrednio do strumienia odpowiedzi
                        await Response.WriteAsync(contentPart.Text, ct);
                        await Response.Body.FlushAsync(ct); // Wymuszenie wysłania "kawałka"
                    }
                }
            }
        }
    }
}
