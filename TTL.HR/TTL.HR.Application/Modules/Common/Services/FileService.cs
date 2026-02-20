using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Json;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Application.Modules.Common.Services
{
    public class FileService : IFileService
    {
        private readonly HttpClient _httpClient;

        public FileService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> UploadFileAsync(IBrowserFile file, string folder = "general")
        {
            try
            {
                var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024)); // 10MB
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                
                content.Add(fileContent, "file", file.Name);

                var response = await _httpClient.PostAsync($"core/files/upload?folder={folder}", content);

                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<UploadResponse>>();
                    var url = result?.Data?.FileUrl;
                    if (!string.IsNullOrEmpty(url) && url.StartsWith("/"))
                    {
                        url = _httpClient.BaseAddress + url.TrimStart('/');
                    }
                    return url;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"FileUpload Error: {ex.Message}");
            }
            return null;
        }

        private class UploadResponse
        {
            public string FileName { get; set; } = string.Empty;
            public string FileUrl { get; set; } = string.Empty;
            public string FileSize { get; set; } = string.Empty;
            public string FileType { get; set; } = string.Empty;
        }
    }
}
