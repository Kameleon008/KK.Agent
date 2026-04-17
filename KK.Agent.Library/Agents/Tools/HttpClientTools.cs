using System.ComponentModel;
using System.Net;
using System.Text;
using KK.Agent.Library.Attributes;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace KK.Agent.Library.Agents.Tools;

public class HttpClientTools
{
    private static readonly HttpClient HttpClient = new HttpClient();

    [AgentTool("Sends a POST request with raw JSON body to the specified URL. Returns status, headers, and body.")]
    public async Task<HttpRequestResult> SendPostRequestAsync(
        [Description("The target URL for the HTTP request (e.g., 'https://api.example.com/endpoint')]")] string url,
        [Description("The raw JSON string to send in the request body. Example: '{\"key\": \"value\", \"count\": 42}'")] string jsonBody)
    {
        try
        {
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            using var response = await HttpClient.PostAsync(url, content);
            string responseBody = await response.Content.ReadAsStringAsync();

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

            var result = new HttpRequestResult
            {
                StatusCode = (int)response.StatusCode,
                Body = responseBody,
                Headers = headers
            };


            return result;
        }
        catch (Exception ex)
        {
            return new HttpRequestResult
            {
                StatusCode = 0,
                Body = $"ERROR: {ex.Message}",
                Headers = new Dictionary<string, string[]> { { "error", new[] { ex.ToString() } } }
            };
        }
    }

    [AgentTool("Sends a GET request to the specified URL. Returns status, headers, and body.")]
    public async Task<HttpRequestResult> SendGetRequestAsync(
        [Description("The target URL for the HTTP request (e.g., 'https://api.example.com/endpoint')]")] string url)
    {
        try
        {
            using var response = await HttpClient.GetAsync(url);
            string responseBody = await response.Content.ReadAsStringAsync();

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

            var result = new HttpRequestResult
            {
                StatusCode = (int)response.StatusCode,
                Body = responseBody,
                Headers = headers
            };

            return result;
        }
        catch (Exception ex)
        {
            return new HttpRequestResult
            {
                StatusCode = 0,
                Body = $"ERROR: {ex.Message}",
                Headers = new Dictionary<string, string[]> { { "error", new[] { ex.ToString() } } }
            };
        }
    }

    [AgentTool("Sends a POST request with raw JSON body and includes API key authentication. Returns status, headers, and body.")]
    public async Task<HttpRequestResult> SendAuthenticatedPostRequestAsync(
        [Description("apiKey which can be required to authorize request")] string apiKey,
        [Description("The target URL for the HTTP request (e.g., 'https://api.example.com/endpoint')]")] string url,
        [Description("The raw JSON string to send in the request body. Example: '{\"key\": \"value\", \"count\": 42}'")] string jsonBody)
    {
        try
        {
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            // Add API key header if available
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                HttpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);
            }

            using var response = await HttpClient.PostAsync(url, content);
            string responseBody = await response.Content.ReadAsStringAsync();

            // Remove the API key header after use to avoid leaking it in subsequent requests
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                HttpClient.DefaultRequestHeaders.Remove("X-API-Key");
            }

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

            var result = new HttpRequestResult
            {
                StatusCode = (int)response.StatusCode,
                Body = responseBody,
                Headers = headers
            };


            return result;
        }
        catch (Exception ex)
        {
            return new HttpRequestResult
            {
                StatusCode = 0,
                Body = $"ERROR: {ex.Message}",
                Headers = new Dictionary<string, string[]> { { "error", new[] { ex.ToString() } } }
            };
        }
    }

    [AgentTool("Sends a custom HTTP request with specified method. Returns status, headers, and body.")]
    public async Task<HttpRequestResult> SendCustomRequestAsync(
        [Description("The HTTP method: GET, POST, etc.")] string method,
        [Description("The target URL for the HTTP request")] string url,
        [Description("Optional raw JSON body for requests that need it (POST, PUT, PATCH)")] string? jsonBody = null)
    {
        try
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
            string responseBody = await response.Content.ReadAsStringAsync();

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

            var result = new HttpRequestResult
            {
                StatusCode = (int)response.StatusCode,
                Body = responseBody,
                Headers = headers
            };

            return result;
        }
        catch (Exception ex)
        {
            return new HttpRequestResult
            {
                StatusCode = 0,
                Body = $"ERROR: {ex.Message}",
                Headers = new Dictionary<string, string[]> { { "error", new[] { ex.ToString() } } }
            };
        }
    }

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

    [AgentTool("Validate prompt")]
    public async Task<string> ValidatePrompt(
        [Description("REQUIRED apiKey which is used to authorize request")] string apiKey,
        [Description("Prompt, must contains placeholders {ID} and {DESCRIPTION}")] string prompt)
    {
        try
        {

            using var itemsResponse = await HttpClient.GetAsync($"https://hub.ag3nts.org/data/{apiKey}/categorize.csv");

            var itemsResult = await itemsResponse.Content.ReadAsStringAsync();

            var items = ParseCsv(itemsResult);

            var sortedList = new List<ItemToClassify>();

            sortedList.Add(items[9]);
            sortedList.Add(items[3]);
            sortedList.Add(items[8]);
            sortedList.Add(items[1]);
            sortedList.Add(items[0]);
            sortedList.Add(items[2]);
            sortedList.Add(items[6]);
            sortedList.Add(items[4]);
            sortedList.Add(items[7]);
            sortedList.Add(items[5]);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(prompt);
            Console.ResetColor();

            var responses = new List<string>();

            foreach (var item in sortedList)
            {

                var payload = new
                {
                    apikey = apiKey,
                    task = "categorize",
                    answer = new
                    {
                        prompt = prompt.Replace("{ID}", item.Id).Replace("{DESCRIPTION}", item.Description)
                    }
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var response = await HttpClient.PostAsync("https://hub.ag3nts.org/verify", content);

                var result = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.Accepted)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(result);
                    Console.ResetColor();

                    responses.Add(result);

                    continue;
                }


                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result);
                Console.ResetColor();
                return result;
            }

            return $"Products properly classified with prompt {prompt}, responses: {string.Join(", ", responses)}";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public static List<ItemToClassify> ParseCsv(string csv)
    {
        var result = new List<ItemToClassify>();

        var lines = csv.Split('\n')
            .Skip(1) // pomijamy nagłówek
            .Where(l => !string.IsNullOrWhiteSpace(l));

        foreach (var line in lines)
        {
            var parts = line.Split(',');

            if (parts.Length < 2) continue;

            var id = parts[0].Trim();
            var description = parts[1].Trim().Trim('"');

            result.Add(new ItemToClassify
            {
                Id = id,
                Description = description
            });
        }

        return result;
    }

    public class ItemToClassify
    {
        [JsonProperty("id")]
        [Description("indentificator of item")]
        public string Id { get; set; }

        [JsonProperty("description")]
        [Description("description of item")]
        public string Description { get; set; }

    }
}
