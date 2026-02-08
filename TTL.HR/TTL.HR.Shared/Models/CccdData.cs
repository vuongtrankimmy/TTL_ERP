using System;

namespace TTL.HR.Shared.Models
{
    public class CccdData
    {
        public string Name { get; set; }
        public DateTime? DOB { get; set; }
        public string Gender { get; set; }
        public string IdCard { get; set; }
        public string Hometown { get; set; }
        public string Address { get; set; }
        public string Nationality { get; set; } = "Việt Nam";
        public string Ethnicity { get; set; } = "Kinh";
        public string Religion { get; set; } = "Không";
        public string IssueDate { get; set; }
        public string IssuePlace { get; set; }
    }
}
