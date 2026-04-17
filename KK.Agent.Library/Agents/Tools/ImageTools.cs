using System.ComponentModel;
using KK.Agent.Library.Attributes;

namespace KK.Agent.Library.Agents.Tools
{
    public class ImageTools
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        [AgentTool("Fetches an image from the specified URL and returns it as a base64 string. Only works with image content types.")]
        public async Task<string?> FetchImageAsBase64Async(
            [Description("The URL of the image to fetch (e.g., 'https://example.com/image.png')]")] string url)
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
