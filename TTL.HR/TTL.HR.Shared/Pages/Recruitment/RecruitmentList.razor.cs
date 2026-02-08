using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace TTL.HR.Shared.Pages.Recruitment
{
    public partial class RecruitmentList
    {
        private bool _isLoading = true;

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            // Simulate API loading
            await System.Threading.Tasks.Task.Delay(1300);
            _isLoading = false;
        }

        private List<JobItem> _jobs = new()
        {
            new() { Id = 1, Title = "Senior .NET Developer", Code = "DEV-001", Department = "Kỹ thuật", Type = "Toàn thời gian", HiringCount = 2, Deadline = new DateTime(2026, 3, 15), ApplicantsCount = 12, Status = "Đang tuyển", StatusClass = "badge-light-success" },
            new() { Id = 2, Title = "HR Specialist", Code = "HR-002", Department = "Nhân sự", Type = "Toàn thời gian", HiringCount = 1, Deadline = new DateTime(2026, 2, 28), ApplicantsCount = 5, Status = "Đang tuyển", StatusClass = "badge-light-success" },
            new() { Id = 3, Title = "Sales Executive", Code = "SALE-003", Department = "Kinh doanh", Type = "Toàn thời gian", HiringCount = 5, Deadline = new DateTime(2026, 3, 10), ApplicantsCount = 20, Status = "Tạm dừng", StatusClass = "badge-light-warning" },
            new() { Id = 4, Title = "Marketing Intern", Code = "MKT-004", Department = "Marketing", Type = "Thực tập", HiringCount = 3, Deadline = new DateTime(2026, 2, 20), ApplicantsCount = 8, Status = "Đã đóng", StatusClass = "badge-light-danger" }
        };

        public class JobItem
        {
            public int Id { get; set; }
            public string Title { get; set; } = "";
            public string Code { get; set; } = "";
            public string Department { get; set; } = "";
            public string Type { get; set; } = "";
            public int HiringCount { get; set; }
            public DateTime Deadline { get; set; }
            public int ApplicantsCount { get; set; }
            public string Status { get; set; } = "";
            public string StatusClass { get; set; } = "";
        }
    }
}
