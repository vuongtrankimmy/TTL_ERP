using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TTL.HR.Application.Modules.Common.Models
{
    public class LookupModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int LookupID { get; set; }
        public string? Type { get; set; }
        public int Order { get; set; }
        public string? Module { get; set; }
    }

    public class CountryModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string PhoneCode { get; set; } = string.Empty;
    }

    public class ApiResponse<T>
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
        
        [JsonPropertyName("data")]
        public T? Data { get; set; }
        
        [JsonPropertyName("errors")]
        public List<string>? Errors { get; set; }
    }

    public class PagedResult<T>
    {
        [JsonPropertyName("items")]
        public List<T> Items { get; set; } = new();
        
        [JsonPropertyName("pageIndex")]
        public int PageIndex { get; set; }
        
        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }
        
        [JsonPropertyName("totalCount")]
        public long TotalCount { get; set; }
        
        [JsonPropertyName("totalPages")]
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
