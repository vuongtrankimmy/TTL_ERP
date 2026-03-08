using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Recruitment;
using TTL.HR.Application.Modules.Recruitment.Models;

namespace TTL.HR.Shared.Pages.Recruitment
{
    public partial class RecruitmentDetails
    {
        [Parameter] public string Id { get; set; } = string.Empty;
        [Inject] public IRecruitmentApplication RecruitmentApp { get; set; } = default!;
        [Inject] public NavigationManager Navigation { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;

        protected JobDetail? Job { get; set; }
        protected List<ApplicantItem> Applicants { get; set; } = new();
        protected List<ApplicantItem> FilteredApplicants => string.IsNullOrWhiteSpace(SearchTerm) 
            ? Applicants 
            : Applicants.Where(a => a.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) || a.Email.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
        
        protected string SearchTerm { get; set; } = "";
        protected bool _isLoading = true;

        // Modal State
        protected bool IsAddApplicantModalOpen = false;
        protected ApplicantRequest NewApplicant = new();
        protected bool IsDeleteModalOpen = false;
        protected ApplicantItem? ApplicantToDelete;
        protected bool IsProfileModalOpen = false;
        protected ApplicantItem? SelectedApplicant;
        protected bool IsScheduleModalOpen = false;
        protected InterviewScheduleModel ScheduleModel = new();
        protected bool IsRejectionModalOpen = false;
        protected string RejectionReason = "";
        protected ApplicantItem? ApplicantToReject;
        protected bool IsSuccessModalOpen = false;
        protected string SuccessNote = "";
        protected ApplicantItem? ApplicantToSuccess;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            _isLoading = true;
            try 
            {
                Job = await RecruitmentApp.GetJobDetailsAsync(Id);
                var applicantList = await RecruitmentApp.GetApplicantsAsync(Id);
                Applicants = applicantList.ToList();
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

        protected void OpenAddApplicantModal()
        {
            NewApplicant = new ApplicantRequest { AppliedDate = DateTime.Today };
            IsAddApplicantModalOpen = true;
        }

        protected void CloseAddApplicantModal() => IsAddApplicantModalOpen = false;

        protected async Task SaveApplicant()
        {
            var success = await RecruitmentApp.AddApplicantAsync(Id, NewApplicant);
            if (success)
            {
                await LoadData();
                CloseAddApplicantModal();
                await JS.InvokeVoidAsync("toastr.success", "Đã thêm ứng viên mới thành công!");
            }
            else
            {
                await JS.InvokeVoidAsync("toastr.error", "Có lỗi xảy ra khi thêm ứng viên.");
            }
        }

        protected async Task UpdateStatus(ApplicantItem applicant, string newStatus, int? newStatusId = null, string? note = null)
        {
            if (newStatus == "Từ chối" || newStatusId == 106)
            {
                ApplicantToReject = applicant;
                RejectionReason = "";
                IsRejectionModalOpen = true;
                return;
            }

            if (newStatus == "Đã tuyển" || newStatusId == 105)
            {
                ApplicantToSuccess = applicant;
                SuccessNote = "";
                IsSuccessModalOpen = true;
                return;
            }

            await ApplyStatusChange(applicant, newStatus, newStatusId, note);
        }

        protected async Task ApplyStatusChange(ApplicantItem applicant, string? newStatus, int? newStatusId = null, string? note = null)
        {
            var success = await RecruitmentApp.UpdateApplicantStatusAsync(applicant.Id.ToString(), newStatus, newStatusId, note);
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

        protected void CloseRejectionModal() => IsRejectionModalOpen = false;
        protected async Task SaveRejection(string reason)
        {
            if (ApplicantToReject != null)
            {
                await ApplyStatusChange(ApplicantToReject, "Rejected", 106, $"Lý do loại: {reason}");
                IsRejectionModalOpen = false;
            }
        }

        protected void CloseSuccessModal() => IsSuccessModalOpen = false;
        protected async Task SaveSuccess(string note)
        {
            if (ApplicantToSuccess != null)
            {
                await ApplyStatusChange(ApplicantToSuccess, "Hired", 105, $"Ghi chú tuyển dụng: {note}");
                IsSuccessModalOpen = false;
            }
        }

        protected void HandleEditJob() => Navigation.NavigateTo($"/recruitment/add?id={Id}");

        protected void PromptDeleteApplicant(ApplicantItem applicant)
        {
            ApplicantToDelete = applicant;
            IsDeleteModalOpen = true;
        }

        protected void CloseDeleteModal()
        {
            IsDeleteModalOpen = false;
            ApplicantToDelete = null;
        }

        protected async Task ConfirmDeleteApplicant()
        {
            if (ApplicantToDelete != null)
            {
                var success = await RecruitmentApp.DeleteApplicantAsync(ApplicantToDelete.Id.ToString());
                if (success)
                {
                    await LoadData();
                    CloseDeleteModal();
                    await JS.InvokeVoidAsync("toastr.success", "Đã xóa hồ sơ ứng viên.");
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", "Có lỗi xảy ra khi xóa hồ sơ.");
                }
            }
        }

        protected void ViewProfile(ApplicantItem applicant)
        {
            SelectedApplicant = applicant;
            IsProfileModalOpen = true;
        }

        protected void CloseProfileModal()
        {
            IsProfileModalOpen = false;
            SelectedApplicant = null;
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
        }

        protected void CloseScheduleModal() => IsScheduleModalOpen = false;

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
    }
}
