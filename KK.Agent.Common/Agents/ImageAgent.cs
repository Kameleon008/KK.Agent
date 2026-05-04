using KK.Agent.Library.AgentEngine;
using KK.Agent.Library.Clients.OpenApi;
using KK.Agent.Library.Tools;

namespace KK.Agent.Library.Agents
{
    public class ImageAgent(OpenApiClient client, ToolsProvider tools, AgentLogger logger)
        : AgentBase(client, tools, logger)
    {
        protected override string SystemPrompt { get; set; } = File.ReadAllText($"./Agents/{nameof(ImageAgent)}.md");

        protected override string AgentId { get; set; } = nameof(ImageAgent);

        private static readonly HttpClient HttpClient = new HttpClient();

        public async Task<string> FetchImageAsBase64Async(string url)
        {
            try
            {
                using var response = await HttpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return $"Failed to fetch image from {url}. Status code: {(int)response.StatusCode}";
                }

                var contentType = response.Content.Headers.ContentType?.MediaType ?? string.Empty;

                // Check if the content is an image
                if (!contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                {
                    return $"Content from {url} is not an image. MIME type: {contentType}";
                }

                var imageBytes = await response.Content.ReadAsByteArrayAsync();
                var base64Image = Convert.ToBase64String(imageBytes);

                // Return with data URI prefix for easy use in HTML/markdown
                return $"data:{contentType};base64,{base64Image}";
            }
            catch (Exception ex)
            {
                return $"Error fetching image from {url}: {ex.Message}";
            }
        }

    }
}
