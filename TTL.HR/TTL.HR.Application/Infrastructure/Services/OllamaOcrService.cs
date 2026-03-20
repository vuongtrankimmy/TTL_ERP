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
     * OllamaOcrService - The Standalone Fortress
     * 100% Local Vision Inference (LLaVA / Moondream)
     * Zero-Cost, Pure Offline Hardware-Accelerated Data Extraction
     */
    public class OllamaOcrService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "http://localhost:11434/api/generate";
        private readonly string _model = "llava";

        public OllamaOcrService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(120); // Local LLMs take time
            _model = configuration["Ollama:Model"] ?? "llava";
            var customUrl = configuration["Ollama:ApiUrl"];
            if (!string.IsNullOrEmpty(customUrl)) _apiUrl = customUrl;
        }

        public async Task<CccdData?> ExtractWithOllamaAsync(byte[] imageBytes)
        {
            if (imageBytes == null) return null;

            try
            {
                Console.WriteLine($"[OLLAMA VISION TRACE] Launching {_model} Core. Payload: {imageBytes.Length / 1024.0:F2} KB");

                string base64Image = Convert.ToBase64String(imageBytes);
                
                var prompt = @"Analyze this image of a Vietnamese ID card. 
                Extract: 01.ID_NUMBER, 02.NAME (UPPERCASE), 03.DOB, 04.GENDER (Nam/Nữ), 05.HOMETOWN, 06.ADDRESS. 
                FORMAT: Return ONLY a valid JSON object.";

                var requestBody = new
                {
                    model = _model,
                    prompt = prompt,
                    images = new string[] { base64Image },
                    stream = false,
                    format = "json"
                };

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                using var response = await _httpClient.PostAsync(_apiUrl, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[OLLAMA VISION TRACE] Success! Local model has finished patterning.");
                    using var doc = JsonDocument.Parse(responseBody);
                    var responseText = doc.RootElement.GetProperty("response").GetString();
                    
                    if (!string.IsNullOrEmpty(responseText))
                    {
                        return JsonSerializer.Deserialize<CccdData>(responseText, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                }
                else
                {
                    Console.WriteLine($"[OLLAMA VISION ERROR] Local API Unreachable | Status: {response.StatusCode}");
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OLLAMA VISION EXCEPTION] {ex.Message} | Ensure Ollama is running (`ollama serve`).");
                return null;
            }
        }
    }
}
