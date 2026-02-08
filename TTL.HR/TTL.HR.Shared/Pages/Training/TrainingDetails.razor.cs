using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace TTL.HR.Shared.Pages.Training
{
    public partial class TrainingDetails
    {
        [Parameter] public int Id { get; set; }

        private CourseDetail? Course { get; set; }
        private List<AttendeeItem> Attendees { get; set; } = new();

        private bool _isLoading = true;

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            // Simulate API loading
            await System.Threading.Tasks.Task.Delay(1400);
            _isLoading = false;

            // Simulate fetching data based on Id
            Course = new CourseDetail
            {
                Id = Id,
                Title = "Kỹ năng giao tiếp nâng cao",
                Code = "COM-2024",
                Trainer = "Nguyễn Văn A",
                Type = "Nội bộ",
                StartDate = new DateTime(2026, 3, 10),
                Duration = "2 ngày",
                Location = "Phòng họp 1",
                ParticipantsCount = 25,
                MaxParticipants = 30,
                Status = "Sắp diễn ra"
            };

            Attendees = new List<AttendeeItem>
            {
                new() { Name = "Nguyễn Thị X", Email = "thix@example.com", Avatar = "assets/media/avatars/300-10.jpg", Department = "Kinh doanh", Status = "Đã đăng ký", StatusBadge = "badge-light-primary", Score = "-" },
                new() { Name = "Trần Văn Y", Email = "vany@example.com", Avatar = "assets/media/avatars/300-11.jpg", Department = "Kỹ thuật", Status = "Hoàn thành", StatusBadge = "badge-light-success", Score = "8.5" },
                new() { Name = "Lê Thị Z", Email = "thiz@example.com", Avatar = "assets/media/avatars/300-12.jpg", Department = "Nhân sự", Status = "Vắng mặt", StatusBadge = "badge-light-danger", Score = "0" }
            };
        }

        public class CourseDetail
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
        }

        public class AttendeeItem
        {
            public string Name { get; set; } = "";
            public string Email { get; set; } = "";
            public string Avatar { get; set; } = "";
            public string Department { get; set; } = "";
            public string Status { get; set; } = "";
            public string StatusBadge { get; set; } = "";
            public string Score { get; set; } = "";
        }
    }
}
