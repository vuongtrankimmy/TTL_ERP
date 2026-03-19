using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TTL.HR.Application.Modules.Common.Models
{
    public class LookupModel
    {
        /// <summary>String Id (e.g. MongoDB _id hoặc GUID). Dùng khi API trả về string.</summary>
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        private string _code = string.Empty;
        public string Code 
        { 
            get => _code; 
            set 
            { 
                _code = value; 
                if (int.TryParse(value, out int id)) LookupID = id; 
            } 
        }

        /// <summary>Numeric integer ID từ API (tỉnh/huyện/phường). Dùng ưu tiên khi > 0.</summary>
        public int LookupID { get; set; }

        public string? Type { get; set; }
        public int Order { get; set; }
        public string? Module { get; set; }

        /// <summary>Trả về ID hiệu quả: nếu LookupID > 0 thì dùng LookupID.ToString(), ngược lại dùng Id string.</summary>
        [JsonIgnore]
        public string EffectiveId => LookupID > 0 ? LookupID.ToString() : Id;
    }

    public class CountryModel
    {
        public string Id { get; set; } = string.Empty;

        /// <summary>Numeric integer ID của quốc gia từ API.</summary>
        public int IntId { get; set; }

        public string Name { get; set; } = string.Empty;
        private string _code = string.Empty;
        public string Code 
        { 
            get => _code; 
            set 
            { 
                _code = value; 
                if (int.TryParse(value, out int id)) IntId = id; 
            } 
        }
        public string PhoneCode { get; set; } = string.Empty;

        /// <summary>Trả về ID hiệu quả: nếu IntId > 0 thì dùng IntId.ToString(), ngược lại dùng Id.</summary>
        [JsonIgnore]
        public string EffectiveId => IntId > 0 ? IntId.ToString() : Id;
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

        [JsonPropertyName("stackTrace")]
        public string? StackTrace { get; set; }
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
