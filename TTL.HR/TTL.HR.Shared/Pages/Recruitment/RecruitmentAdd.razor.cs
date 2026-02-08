using System;
using Microsoft.AspNetCore.Components;
using TTL.HR.Shared.Models;

namespace TTL.HR.Shared.Pages.Recruitment
{
    public partial class RecruitmentAdd
    {
        [Parameter, SupplyParameterFromQuery]
        public int? Id { get; set; }

        private JobDetail Model = new();
        private bool _isEdit => Id.HasValue;
        private bool _isLoading = false;

        [Inject] public NavigationManager Navigation { get; set; }

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            if (_isEdit)
            {
                _isLoading = true;
                await System.Threading.Tasks.Task.Delay(500);
                
                Model = new JobDetail
                {
                    Id = Id.Value,
                    Title = "Senior .NET Developer",
                    Code = "DEV-001",
                    Department = "IT",
                    Type = "Full-time",
                    HiringCount = 2,
                    Deadline = new DateTime(2026, 3, 15),
                    Description = "Phát triển và duy trì các hệ thống backend sử dụng .NET Core.",
                    Requirements = "Ít nhất 2 năm kinh nghiệm.",
                    Benefits = "Lương hấp dẫn."
                };
                _isLoading = false;
            }
            else
            {
                Model = new JobDetail { Deadline = DateTime.Today.AddMonths(1) };
            }
        }

        private void SaveRecruitment()
        {
            Navigation.NavigateTo("/recruitment");
        }
    }
}
