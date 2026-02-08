using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using TTL.HR.Shared.Models;

namespace TTL.HR.Shared.Pages.User.Components
{
    public partial class UserAttendanceHistory
    {
        [Parameter] public int UserId { get; set; }

        private bool _showExplanationDrawer = false;
        private AttendanceRecord? _selectedRecordForExplanation;

        private List<AttendanceRecord> _attendanceRecords = new List<AttendanceRecord>();

        protected override void OnParametersSet()
        {
            // Mock data logic based on UserId (simulated)
            GenerateMockData(UserId);
        }

        private void GenerateMockData(int userId)
        {
            // Reset and generate fresh mock data
            _attendanceRecords.Clear();
            var random = new Random(userId); // Use userId as seed for consistent random data per user

            DateTime startDate = new DateTime(2026, 2, 1);
            DateTime endDate = new DateTime(2026, 2, 28);

            for (DateTime date = endDate; date >= startDate; date = date.AddDays(-1))
            {
                var record = new AttendanceRecord
                {
                    Date = date,
                    DayOfWeek = date.ToString("dddd", new System.Globalization.CultureInfo("vi-VN")),
                    CheckIn = "08:00",
                    CheckOut = "17:30",
                    TotalWork = "8h 00m",
                    Status = "Đúng giờ",
                    StatusClass = "badge-light-success",
                    CheckInColor = "text-gray-800",
                    CheckOutColor = "text-gray-800",
                    TotalWorkColor = "text-success"
                };

                // Randomize some statuses
                if (date.DayOfWeek == DayOfWeek.Sunday)
                {
                    record.DayOfWeek = "Chủ Nhật";
                    record.CheckIn = "--:--";
                    record.CheckOut = "--:--";
                    record.TotalWork = "0h 00m";
                    record.Status = "Ngày nghỉ";
                    record.StatusClass = "badge-light-secondary";
                    record.CheckInColor = "text-muted fst-italic";
                    record.CheckOutColor = "text-muted fst-italic";
                    record.TotalWorkColor = "text-muted";
                }
                else if (random.Next(0, 10) == 1) // 10% chance of late
                {
                    record.CheckIn = "08:45";
                    record.TotalWork = "7h 15m";
                    record.Status = "Đi muộn";
                    record.StatusClass = "badge-light-warning";
                    record.CheckInColor = "text-danger";
                    record.TotalWorkColor = "text-warning";
                }
                else if (random.Next(0, 15) == 2) // ~6% chance of leave
                {
                        record.CheckIn = "P";
                        record.CheckOut = "P";
                        record.TotalWork = "8h 00m";
                        record.Status = "Nghỉ phép";
                        record.StatusClass = "badge-light-info";
                        record.CheckInColor = "text-info";
                        record.CheckOutColor = "text-info";
                }

                _attendanceRecords.Add(record);
            }
        }

        private void OpenExplanation(AttendanceRecord record)
        {
            _selectedRecordForExplanation = record;
            _showExplanationDrawer = true;
        }

        private void CloseExplanation()
        {
            _showExplanationDrawer = false;
            _selectedRecordForExplanation = null;
        }

        public class AttendanceRecord
        {
            public DateTime Date { get; set; }
            public string DayOfWeek { get; set; } = "";
            public string CheckIn { get; set; } = "";
            public string CheckOut { get; set; } = "";
            public string TotalWork { get; set; } = "";
            public string Status { get; set; } = "";
            public string StatusClass { get; set; } = "";
            public string CheckInColor { get; set; } = "";
            public string CheckOutColor { get; set; } = "";
            public string TotalWorkColor { get; set; } = "";
        }
    }
}
