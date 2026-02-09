using System;

namespace TTL.HR.Shared.Models
{
    public class CccdData
    {
        public string Name { get; set; } = string.Empty;
        public DateTime? DOB { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string IdCard { get; set; } = string.Empty;
        public string Hometown { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Nationality { get; set; } = "Việt Nam";
        public string Ethnicity { get; set; } = "Kinh";
        public string Religion { get; set; } = "Không";
        public string IssueDate { get; set; } = string.Empty;
        public string IssuePlace { get; set; } = string.Empty;
    }
}
