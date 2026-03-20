using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Models;

namespace TTL.HR.Application.Infrastructure.Services
{
    /**
     * FptOcrService - Specialized Vietnamese AI (FPT Smart Cloud)
     * High-Precision OCR for Vietnam ID Cards (CCCD)
     */
    public class FptOcrService : IOcrService
    {
        private readonly HttpClient _httpClient;
        private readonly string? _apiKey;
        private readonly string _apiUrl = "https://api.fpt.ai/vision/idr/vnm/";

        public FptOcrService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(60);
            _apiKey = configuration["FptAi:ApiKey"];
        }

        public string? GetApiKey() => _apiKey;

        public async Task<CccdData?> ProcessCccdAsync(byte[] imageBytes, string fileName)
        {
            if (string.IsNullOrEmpty(_apiKey) || imageBytes == null) return null;

            try
            {
                using var content = new MultipartFormDataContent();
                var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                content.Add(imageContent, "image", fileName);

                var request = new HttpRequestMessage(HttpMethod.Post, _apiUrl);
                request.Headers.Add("api-key", _apiKey);
                request.Content = content;

                var response = await _httpClient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(responseBody);
                    var dataArray = doc.RootElement.GetProperty("data");
                    
                    if (dataArray.GetArrayLength() > 0)
                    {
                        var data = dataArray[0];
                        return new CccdData
                        {
                            IdCard = GetProp(data, "id"),
                            Name = GetProp(data, "name"),
                            DOB = DateTime.TryParseExact(GetProp(data, "dob"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var dob) ? dob : null,
                            Gender = GetProp(data, "sex") == "Nam" ? "Nam" : "Nữ",
                            Hometown = GetProp(data, "home"),
                            Address = GetProp(data, "address"),
                            Nationality = "Việt Nam"
                        };
                    }
                }
                else
                {
                    Console.WriteLine($"[FPT AI ERROR] Status: {response.StatusCode} | Detail: {responseBody}");
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FPT AI EXCEPTION] {ex.Message}");
                return null;
            }
        }

        private string GetProp(JsonElement el, string name)
        {
            try { return el.GetProperty(name).GetString() ?? ""; } catch { return ""; }
        }
    }
}
