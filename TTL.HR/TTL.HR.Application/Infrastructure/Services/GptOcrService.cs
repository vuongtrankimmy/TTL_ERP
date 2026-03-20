using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TTL.HR.Application.Modules.HumanResource.Models;

namespace TTL.HR.Application.Infrastructure.Services
{
    /**
     * GptOcrService - Intelligence Tier (Vision Edition)
     * High-Precision Diagnostics for OpenAI Vision (with Payload tracing)
     */
    public class GptOcrService
    {
        private readonly HttpClient _httpClient;
        private readonly string? _apiKey;
        private readonly string _apiUrl = "https://api.openai.com/v1/chat/completions";

        public GptOcrService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            // Set timeout longer for large image uploads (e.g., 60s)
            _httpClient.Timeout = TimeSpan.FromSeconds(60);
            _apiKey = configuration["OpenAI:ApiKey"];
        }

        public async Task<CccdData?> ExtractWithGptVisionAsync(byte[] imageBytes)
        {
            if (string.IsNullOrEmpty(_apiKey) || imageBytes == null) return null;

            try
            {
                // LOG PAYLOAD SIZE
                Console.WriteLine($"[GPT VISION TRACE] Preparing payload. Image Size: {imageBytes.Length / 1024.0:F2} KB");

                string base64Image = Convert.ToBase64String(imageBytes);
                
                var prompt = @"
                Analyze this Vietnamese ID Card (CCCD) image and extract EVERY data field. 
                Format dates as dd/MM/yyyy.
                Gender must be 'Nam' or 'Nữ'.
                Ensure 'Hometown' and 'Address' are complete and clean (correct typos in place names).
                Return ONLY a valid JSON object with: 
                IdCard, Name (UPPERCASE), DOB, Gender, Hometown, Address, Nationality (Default: Việt Nam).";

                var requestBody = new
                {
                    model = "gpt-4o-mini",
                    messages = new object[]
                    {
                        new {
                            role = "user",
                            content = new object[]
                            {
                                new { type = "text", text = prompt },
                                new { type = "image_url", image_url = new { url = $"data:image/jpeg;base64,{base64Image}" } }
                            }
                        }
                    },
                    response_format = new { type = "json_object" },
                    max_tokens = 500
                };

                Console.WriteLine($"[GPT VISION TRACE] Sending to OpenAI...");

                var request = new HttpRequestMessage(HttpMethod.Post, _apiUrl);
                request.Headers.Add("Authorization", $"Bearer {_apiKey}");
                request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                using var response = await _httpClient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[GPT VISION TRACE] Success! Received data hub.");
                    using var doc = JsonDocument.Parse(responseBody);
                    var content = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                    
                    if (!string.IsNullOrEmpty(content))
                    {
                        return JsonSerializer.Deserialize<CccdData>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                }
                else
                {
                    Console.WriteLine($"[GPT VISION ERROR] Status: {response.StatusCode} ({response.ReasonPhrase})");
                    Console.WriteLine($"[GPT VISION ERROR] Detail: {responseBody}");
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GPT VISION EXCEPTION] {ex.Message}");
                if (ex.InnerException != null) Console.WriteLine($"[GPT VISION EXCEPTION] Inner: {ex.InnerException.Message}");
                return null;
            }
        }
    }
}
