using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Models;

namespace TTL.HR.Application.Infrastructure.Services
{
    /**
     * VnptOcrService - Specialized Vietnamese AI (VNPT Smart Vision)
     * High-Precision OCR for Vietnam ID Cards (CCCD)
     */
    public class VnptOcrService : IOcrService
    {
        private readonly HttpClient _httpClient;
        private readonly string? _token;
        private readonly string _apiUrl = "https://api-ocr.vnpt.ai/v1/ocr/id-card";

        public VnptOcrService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(60);
            _token = configuration["VnptAi:Token"];
        }

        public string? GetToken() => _token;

        public async Task<CccdData?> ProcessCccdAsync(byte[] imageBytes, string fileName)
        {
            if (string.IsNullOrEmpty(_token) || imageBytes == null) return null;

            try
            {
                string base64Image = Convert.ToBase64String(imageBytes);
                var requestBody = new { image = base64Image };

                var request = new HttpRequestMessage(HttpMethod.Post, _apiUrl);
                request.Headers.Add("Token", _token);
                // request.Headers.Add("X-Client-Id", _clientId); // Optional if using clientId
                request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                using var response = await _httpClient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(responseBody);
                    // VNPT AI usually returns { "status": 0, "object": { "id_card": ..., "name": ... } }
                    if (doc.RootElement.GetProperty("status").GetInt32() == 0)
                    {
                        var obj = doc.RootElement.GetProperty("object");
                        return new CccdData
                        {
                            IdCard = GetProp(obj, "id_card"),
                            Name = GetProp(obj, "name"),
                            DOB = DateTime.TryParseExact(GetProp(obj, "dob"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var dob) ? dob : null,
                            Gender = GetProp(obj, "sex") == "Nam" ? "Nam" : "Nữ",
                            Hometown = GetProp(obj, "hometown"),
                            Address = GetProp(obj, "address"),
                            Nationality = "Việt Nam"
                        };
                    }
                }
                else
                {
                    Console.WriteLine($"[VNPT AI ERROR] Status: {response.StatusCode} | Detail: {responseBody}");
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[VNPT AI EXCEPTION] {ex.Message}");
                return null;
            }
        }

        private string GetProp(JsonElement el, string name)
        {
            try { return el.GetProperty(name).GetString() ?? ""; } catch { return ""; }
        }
    }
}
