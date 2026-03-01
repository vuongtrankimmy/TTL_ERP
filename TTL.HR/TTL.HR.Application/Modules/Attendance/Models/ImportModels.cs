using System.Collections.Generic;

namespace TTL.HR.Application.Modules.Attendance.Models
{
    public class ImportAttendanceResultModel
    {
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public int DuplicateCount { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<ImportPreviewItemModel> PreviewItems { get; set; } = new();
        public List<ImportRawItemModel> RawItems { get; set; } = new();
        public List<string> FileHeaders { get; set; } = new();
    }

    public class ImportRawItemModel
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public Dictionary<string, string> RowData { get; set; } = new();
    }

    public class ImportPreviewItemModel
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public string ShiftId { get; set; } = string.Empty;
        public string ShiftName { get; set; } = string.Empty;
        public string ShiftCode { get; set; } = string.Empty;
        public string Status { get; set; } = "Normal";
        public int LateMinutes { get; set; }
        public int EarlyLeaveMinutes { get; set; }
        public double WorkingHours { get; set; }
        public double OvertimeHours { get; set; }
        public string Note { get; set; } = string.Empty;
        public string? Method { get; set; }
        public string? DeviceName { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Confidence { get; set; }
        public Dictionary<string, string> ExtraData { get; set; } = new();
    }
}
