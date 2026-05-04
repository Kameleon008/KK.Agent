using System.ComponentModel;
using System.Net;
using System.Text;
using KK.Agent.Library.Attributes;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace KK.Agent.Library.Agents.Tools;

public class HttpClientTools(IServiceProvider provider)
{
    private static readonly HttpClient HttpClient = new HttpClient();

    [AgentTool("Sends a POST request with raw JSON body to the specified URL. Returns status, headers, and body.")]
    public async Task<HttpRequestResult> SendPostRequestAsync(
        [Description("The target URL for the HTTP request (e.g., 'https://api.example.com/endpoint')]")] string url,
        [Description("The raw JSON string to send in the request body. Example: '{\"key\": \"value\", \"count\": 42}'")] string jsonBody)
    {
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        using var response = await HttpClient.PostAsync(url, content);

        return await ProcessResponseAsync(response);
    }

    [AgentTool("Sends a GET request to the specified URL. Returns status, headers, and body.")]
    public async Task<HttpRequestResult> SendGetRequestAsync(
        [Description("The target URL for the HTTP request (e.g., 'https://api.example.com/endpoint')]")] string url)
    {
        using var response = await HttpClient.GetAsync(url);

        return await ProcessResponseAsync(response);
    }

    [AgentTool("Sends a custom HTTP request with specified method. Returns status, headers, and body.")]
    public async Task<HttpRequestResult> SendCustomRequestAsync(
        [Description("The HTTP method: GET, POST, etc.")] string method,
        [Description("The target URL for the HTTP request")] string url,
        [Description("Optional raw JSON body for requests that need it (POST, PUT, PATCH)")] string? jsonBody = null)
    {
        HttpRequestMessage request;

        if (string.Equals(method, "GET", StringComparison.OrdinalIgnoreCase))
        {
            request = new HttpRequestMessage(HttpMethod.Get, url);
        }
        else if (string.Equals(method, "POST", StringComparison.OrdinalIgnoreCase))
        {
            var content = string.IsNullOrEmpty(jsonBody)
                ? new StringContent("", Encoding.UTF8, "application/json")
                : new StringContent(jsonBody!, Encoding.UTF8, "application/json");
            request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
        }
        else
        {
            var content = string.IsNullOrEmpty(jsonBody)
                ? new StringContent("", Encoding.UTF8, "application/json")
                : new StringContent(jsonBody!, Encoding.UTF8, "application/json");
            request = new HttpRequestMessage(new HttpMethod(method), url) { Content = content };
        }

        using var response = await HttpClient.SendAsync(request);

        return await ProcessResponseAsync(response);
    }

    /// <summary>
    /// Common method to process HTTP response and create HttpRequestResult.
    /// Extracts headers, status code, and body from the response.
    /// </summary>
    private static async Task<HttpRequestResult> ProcessResponseAsync(HttpResponseMessage response)
    {
        try
        {
            var responseBody = await response.Content.ReadAsStringAsync();

            // Extract headers from response
            var headers = new Dictionary<string, string[]>();
            foreach (var header in response.Headers)
            {
                headers[header.Key] = header.Value.ToArray();
            }

            // Add any content-specific headers
            if (response.Content.Headers != null)
            {
                foreach (var header in response.Content.Headers)
                {
                    headers[header.Key] = header.Value.ToArray();
                }
            }

            return new HttpRequestResult
            {
                StatusCode = (int)response.StatusCode,
                Body = responseBody,
                Headers = headers
            };
        }
        catch (Exception ex)
        {
            return CreateErrorResponse(ex);
        }
    }

    /// <summary>
    /// Creates an error response in case of exception.
    /// </summary>
    private static HttpRequestResult CreateErrorResponse(Exception ex) => new()
    {
        StatusCode = 0,
        Body = $"ERROR: {ex.Message}",
        Headers = new Dictionary<string, string[]> { { "error", new[] { ex.ToString() } } }
    };

    public class HttpRequestResult
    {
        /// <summary>
        /// The HTTP status code of the response.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Whether the request was successful (2xx status code).
        /// </summary>
        public bool IsSuccess => StatusCode >= 200 && StatusCode < 300;

        /// <summary>
        /// The response body content.
        /// </summary>
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// All response headers as a dictionary.
        /// </summary>
        public Dictionary<string, string[]> Headers { get; set; } = new();

        /// <summary>
        /// Converts the result to a JSON string.
        /// </summary>
        public override string ToString() => JsonSerializer.Serialize(this);
    }
}
