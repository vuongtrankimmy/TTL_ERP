using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Recruitment;
using TTL.HR.Application.Modules.Recruitment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TTL.HR.Shared.Pages.Recruitment
{
    public partial class RecruitmentKanban
    {
        [Inject] public IRecruitmentApplication RecruitmentApp { get; set; } = default!;
        [Inject] public NavigationManager Navigation { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;

        protected string SearchTerm { get; set; } = "";
        protected string? SelectedJobId { get; set; }

        protected List<KanbanColumn> KanbanBoard = new();
        protected List<JobDetail> Jobs = new();
        protected List<ApplicantItem> AllApplicants = new();
        protected bool _isLoading = true;
        
        // Modal States
        protected bool IsProfileModalOpen = false;
        protected bool IsScheduleModalOpen = false;
        protected bool IsRejectionModalOpen = false;
        protected bool IsSuccessModalOpen = false;

        protected ApplicantItem? SelectedApplicant;
        protected ApplicantItem? ApplicantToReject;
        protected ApplicantItem? ApplicantToSuccess;
        protected InterviewScheduleModel ScheduleModel = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            _isLoading = true;
            try
            {
                var jobsList = await RecruitmentApp.GetActiveJobPostingsAsync();
                Jobs = jobsList.ToList();

                // To load all applicants for Kanban, we might need a separate API or loop through jobs
                // For now, let's assume we can fetch all related applicants or if a job is selected, fetch for that job
                if (!string.IsNullOrEmpty(SelectedJobId))
                {
                    var applicantList = await RecruitmentApp.GetApplicantsAsync(SelectedJobId);
                    AllApplicants = applicantList.ToList();
                }
                else
                {
                    // If no job selected, maybe load a default set or empty
                    AllApplicants = new List<ApplicantItem>();
                    // Proactively load applicants for all active jobs if possible, but keep it efficient
                    foreach(var job in Jobs.Take(5)) // Limit for performance
                    {
                         var appList = await RecruitmentApp.GetApplicantsAsync(job.Id);
                         AllApplicants.AddRange(appList);
                    }
                }
                
                UpdateKanbanView();
            }
            catch (Exception)
            {
                // Error handling
            }
            finally
            {
                _isLoading = false;
            }
        }

        protected void UpdateKanbanView()
        {
            var filtered = AllApplicants
                .Where(a => string.IsNullOrEmpty(SearchTerm) || a.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                .Where(a => string.IsNullOrEmpty(SelectedJobId) || a.JobPostingId == SelectedJobId)
                .ToList();

            KanbanBoard = new List<KanbanColumn>
            {
                new() { Title = "Tiếp nhận", Status = "Applied", ColorClass = "info", Applicants = filtered.Where(a => a.StatusId == 101 || a.Status == "Tiếp nhận" || a.Status == "Applied").ToList() },
                new() { Title = "Sơ loại", Status = "Screening", ColorClass = "primary", Applicants = filtered.Where(a => a.StatusId == 102 || a.Status == "Sơ loại" || a.Status == "Screening").ToList() },
                new() { Title = "Phỏng vấn", Status = "Interview", ColorClass = "warning", Applicants = filtered.Where(a => a.StatusId == 103 || a.Status == "Phỏng vấn" || a.Status == "Interview").ToList() },
                new() { Title = "Đề nghị", Status = "Offered", ColorClass = "info", Applicants = filtered.Where(a => a.StatusId == 104 || a.Status == "Đề nghị" || a.Status == "Offered").ToList() },
                new() { Title = "Kết thúc", Status = "Final", ColorClass = "success", Applicants = filtered.Where(a => a.StatusId == 105 || a.StatusId == 106 || a.Status == "Đã tuyển" || a.Status == "Hired" || a.Status == "Từ chối" || a.Status == "Rejected").ToList() }
            };
            StateHasChanged();
        }

        protected void OnSearch(ChangeEventArgs e)
        {
            SearchTerm = e.Value?.ToString() ?? "";
            UpdateKanbanView();
        }

        protected async Task OnJobSelected(ChangeEventArgs e)
        {
            SelectedJobId = e.Value?.ToString();
            await LoadData();
        }

        protected async Task ApplyStatusChange(ApplicantItem applicant, string? newStatus, int? newStatusId = null, string? note = null)
        {
            var success = await RecruitmentApp.UpdateApplicantStatusAsync(applicant.Id, newStatus, newStatusId, note);
            if (success)
            {
                await LoadData();
                await JS.InvokeVoidAsync("toastr.success", "Đã cập nhật trạng thái hồ sơ!");
            }
            else
            {
                await JS.InvokeVoidAsync("toastr.error", "Có lỗi xảy ra khi cập nhật trạng thái.");
            }
        }

        protected void ViewProfile(ApplicantItem applicant)
        {
            SelectedApplicant = applicant;
            IsProfileModalOpen = true;
        }

        protected void OpenScheduleModal(ApplicantItem applicant)
        {
            var firstInterview = applicant.Interviews.FirstOrDefault();
            SelectedApplicant = applicant;
            ScheduleModel = new InterviewScheduleModel
            {
                ApplicantId = applicant.Id,
                ApplicantName = applicant.Name,
                InterviewDate = DateOnly.FromDateTime(firstInterview?.ScheduledAt ?? DateTime.Today.AddDays(1)),
                InterviewTime = TimeOnly.FromDateTime(firstInterview?.ScheduledAt ?? DateTime.Today.AddHours(9)),
                Interviewer = firstInterview?.InterViewerName ?? "Phan Thanh Tùng",
                Location = firstInterview?.Location ?? "Văn phòng Tầng 5 - TTL Building"
            };
            IsScheduleModalOpen = true;
            IsProfileModalOpen = false;
        }

        protected async Task SaveInterviewSchedule(InterviewScheduleModel model)
        {
            if (SelectedApplicant != null)
            {
                model.ScheduledAt = model.InterviewDate.ToDateTime(model.InterviewTime);
                var success = await RecruitmentApp.ScheduleInterviewAsync(SelectedApplicant.Id, model);
                if (success)
                {
                    await LoadData();
                    IsScheduleModalOpen = false;
                    await JS.InvokeVoidAsync("toastr.success", "Đã cập nhật lịch phỏng vấn thành công!");
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", "Có lỗi xảy ra khi đặt lịch phỏng vấn.");
                }
            }
        }

        protected async Task SaveRejection(string reason)
        {
            if (ApplicantToReject != null)
            {
                await ApplyStatusChange(ApplicantToReject, "Rejected", 106, $"Lý do loại: {reason}");
                IsRejectionModalOpen = false;
            }
        }

        protected async Task SaveSuccess(string note)
        {
            if (ApplicantToSuccess != null)
            {
                await ApplyStatusChange(ApplicantToSuccess, "Hired", 105, $"Ghi chú tuyển dụng: {note}");
                IsSuccessModalOpen = false;
            }
        }

        public class KanbanColumn
        {
            public string Title { get; set; } = "";
            public string Status { get; set; } = "";
            public string ColorClass { get; set; } = "";
            public List<ApplicantItem> Applicants { get; set; } = new();
        }

        public class JobItem
        {
            public int Id { get; set; }
            public string Title { get; set; } = "";
        }
    }
}
