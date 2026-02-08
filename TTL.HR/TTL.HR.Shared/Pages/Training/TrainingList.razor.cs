using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace TTL.HR.Shared.Pages.Training
{
    public partial class TrainingList
    {
        private bool _isLoading = true;

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            // Simulate API loading
            await System.Threading.Tasks.Task.Delay(1300);
            _isLoading = false;
        }

        private List<CourseItem> _courses = new()
        {
            new() { Id = 1, Title = "Kỹ năng giao tiếp nâng cao", Code = "COM-2024", Trainer = "Nguyễn Văn A", Type = "Nội bộ", StartDate = new DateTime(2026, 3, 10), Duration = "2 ngày", Location = "Phòng họp 1", ParticipantsCount = 25, MaxParticipants = 30, Status = "Sắp diễn ra", StatusClass = "badge-light-primary" },
            new() { Id = 2, Title = "Quản trị dự án Agile", Code = "PM-AGILE", Trainer = "Trung tâm ABC", Type = "Bên ngoài", StartDate = new DateTime(2026, 3, 20), Duration = "3 ngày", Location = "Trung tâm ABC", ParticipantsCount = 10, MaxParticipants = 15, Status = "Đang đăng ký", StatusClass = "badge-light-success" },
            new() { Id = 3, Title = "An toàn thông tin cơ bản", Code = "SEC-001", Trainer = "Lê Văn C", Type = "Online", StartDate = new DateTime(2026, 2, 15), Duration = "4 giờ", Location = "Zoom", ParticipantsCount = 45, MaxParticipants = 50, Status = "Đã hoàn thành", StatusClass = "badge-light-info" }
        };

        public class CourseItem
        {
            public int Id { get; set; }
            public string Title { get; set; } = "";
            public string Code { get; set; } = "";
            public string Trainer { get; set; } = "";
            public string Type { get; set; } = "";
            public DateTime StartDate { get; set; }
            public string Duration { get; set; } = "";
            public string Location { get; set; } = "";
            public int ParticipantsCount { get; set; }
            public int MaxParticipants { get; set; }
            public string Status { get; set; } = "";
            public string StatusClass { get; set; } = "";
        }
    }
}
