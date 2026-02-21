using System.Collections.Generic;

namespace TTL.HR.Application.Modules.Attendance.Models
{
    public class ImportAttendanceResultModel
    {
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<ImportPreviewItemModel> PreviewItems { get; set; } = new();
        public List<string> FileHeaders { get; set; } = new();
    }

    public class ImportPreviewItemModel
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public string Status { get; set; } = "Normal";
        public int LateMinutes { get; set; }
        public int EarlyLeaveMinutes { get; set; }
    }
}
