using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using TTL.HR.Shared.Models;

namespace TTL.HR.Shared.Pages.Recruitment
{
    public partial class RecruitmentList
    {
        [Inject] public NavigationManager Navigation { get; set; }
        private bool _isLoading = true;
        private bool IsDeleteModalOpen = false;
        private JobDetail? ItemToDelete;

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            // Simulate API loading
            await System.Threading.Tasks.Task.Delay(1000);
            _isLoading = false;
        }

        private void HandleEdit(int id)
        {
            Navigation.NavigateTo($"/recruitment/add?id={id}");
        }

        private void PromptDelete(JobDetail job)
        {
            ItemToDelete = job;
            IsDeleteModalOpen = true;
        }

        private void CloseDeleteModal()
        {
            IsDeleteModalOpen = false;
            ItemToDelete = null;
        }

        private void ConfirmDelete()
        {
            if (ItemToDelete != null)
            {
                _jobs.Remove(ItemToDelete);
                CloseDeleteModal();
            }
        }

        private List<JobDetail> _jobs = new()
        {
            new() { Id = 1, Title = "Senior .NET Developer", Code = "DEV-001", Department = "Kỹ thuật", Type = "Toàn thời gian", HiringCount = 2, Deadline = new DateTime(2026, 3, 15), ApplicantsCount = 12, Status = "Đang tuyển", StatusClass = "badge-light-success" },
            new() { Id = 2, Title = "HR Specialist", Code = "HR-002", Department = "Nhân sự", Type = "Toàn thời gian", HiringCount = 1, Deadline = new DateTime(2026, 2, 28), ApplicantsCount = 5, Status = "Đang tuyển", StatusClass = "badge-light-success" },
            new() { Id = 3, Title = "Sales Executive", Code = "SALE-003", Department = "Kinh doanh", Type = "Toàn thời gian", HiringCount = 5, Deadline = new DateTime(2026, 3, 10), ApplicantsCount = 20, Status = "Tạm dừng", StatusClass = "badge-light-warning" },
            new() { Id = 4, Title = "Marketing Intern", Code = "MKT-004", Department = "Marketing", Type = "Thực tập", HiringCount = 3, Deadline = new DateTime(2026, 2, 20), ApplicantsCount = 8, Status = "Đã đóng", StatusClass = "badge-light-danger" },
            new() { Id = 5, Title = "Senior UI/UX Designer", Code = "DEV-005", Department = "Kỹ thuật", Type = "Toàn thời gian", HiringCount = 1, Deadline = new DateTime(2026, 4, 1), ApplicantsCount = 15, Status = "Đang tuyển", StatusClass = "badge-light-success" },
            new() { Id = 6, Title = "DevOps Engineer", Code = "OPS-006", Department = "Kỹ thuật", Type = "Toàn thời gian", HiringCount = 1, Deadline = new DateTime(2026, 3, 30), ApplicantsCount = 7, Status = "Đang tuyển", StatusClass = "badge-light-success" },
            new() { Id = 7, Title = "Mobile Developer (React Native)", Code = "MOB-007", Department = "Kỹ thuật", Type = "Toàn thời gian", HiringCount = 2, Deadline = new DateTime(2026, 3, 20), ApplicantsCount = 10, Status = "Đang tuyển", StatusClass = "badge-light-success" },
            new() { Id = 8, Title = "Accounting Manager", Code = "ACC-008", Department = "Kế toán", Type = "Toàn thời gian", HiringCount = 1, Deadline = new DateTime(2026, 3, 5), ApplicantsCount = 4, Status = "Tạm dừng", StatusClass = "badge-light-warning" },
            new() { Id = 9, Title = "Business Analyst", Code = "BA-009", Department = "Kỹ thuật", Type = "Toàn thời gian", HiringCount = 2, Deadline = new DateTime(2026, 3, 12), ApplicantsCount = 18, Status = "Đang tuyển", StatusClass = "badge-light-success" },
            new() { Id = 10, Title = "Content Creator", Code = "MKT-010", Department = "Marketing", Type = "Toàn thời gian", HiringCount = 1, Deadline = new DateTime(2026, 2, 25), ApplicantsCount = 25, Status = "Đã đóng", StatusClass = "badge-light-danger" }
        };
    }
}
