using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace TTL.HR.Shared.Pages.Recruitment
{
    public partial class RecruitmentDetails
    {
        [Parameter] public int Id { get; set; }

        private JobDetail? Job { get; set; }
        private List<ApplicantItem> Applicants { get; set; } = new();
        private bool _isLoading = true;

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            // Simulate API loading
            await System.Threading.Tasks.Task.Delay(1400);
            _isLoading = false;

            // Simulate fetching data based on Id
            Job = new JobDetail
            {
                Id = Id,
                Title = "Senior .NET Developer",
                Code = "DEV-001",
                Department = "Kỹ thuật",
                Type = "Toàn thời gian",
                HiringCount = 2,
                ApplicantsCount = 12,
                Deadline = new DateTime(2026, 3, 15)
            };

            Applicants = new List<ApplicantItem>
            {
                new() { Name = "Nguyễn Văn A", Email = "vana@example.com", Avatar = "assets/media/avatars/300-1.jpg", AppliedDate = new DateTime(2026, 2, 10), Status = "Phỏng vấn", StatusBadge = "badge-light-warning" },
                new() { Name = "Trần Thị B", Email = "thib@example.com", Avatar = "assets/media/avatars/300-2.jpg", AppliedDate = new DateTime(2026, 2, 8), Status = "Đã duyệt", StatusBadge = "badge-light-success" },
                new() { Name = "Lê Văn C", Email = "vanc@example.com", Avatar = "assets/media/avatars/300-3.jpg", AppliedDate = new DateTime(2026, 2, 5), Status = "Từ chối", StatusBadge = "badge-light-danger" }
            };
        }

        public class JobDetail
        {
            public int Id { get; set; }
            public string Title { get; set; } = "";
            public string Code { get; set; } = "";
            public string Department { get; set; } = "";
            public string Type { get; set; } = "";
            public int HiringCount { get; set; }
            public int ApplicantsCount { get; set; }
            public DateTime Deadline { get; set; }
        }

        public class ApplicantItem
        {
            public string Name { get; set; } = "";
            public string Email { get; set; } = "";
            public string Avatar { get; set; } = "";
            public DateTime AppliedDate { get; set; }
            public string Status { get; set; } = "";
            public string StatusBadge { get; set; } = "";
        }
    }
}
