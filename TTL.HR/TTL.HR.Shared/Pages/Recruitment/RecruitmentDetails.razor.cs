using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Shared.Models;

namespace TTL.HR.Shared.Pages.Recruitment
{
    public partial class RecruitmentDetails
    {
        [Parameter] public int Id { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }
        [Inject] public IJSRuntime JS { get; set; }

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

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            await System.Threading.Tasks.Task.Delay(500);
            _isLoading = false;

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
                new() { Id = 1, Name = "Nguyễn Văn Anh", Email = "anh.nv@gmail.com", Avatar = "assets/media/avatars/300-1.jpg", AppliedDate = new DateTime(2026, 2, 1), Status = "Phỏng vấn", StatusBadge = "badge-light-warning", History = new List<StatusHistory> { new() { Status = "Tiếp nhận", Date = new DateTime(2026, 2, 1) }, new() { Status = "Phỏng vấn", Date = new DateTime(2026, 2, 5), Note = "Đã hẹn phỏng vấn vòng 1" } } },
                new() { Id = 2, Name = "Trần Thị Bình", Email = "binh.tt@outlook.com", Avatar = "assets/media/avatars/300-2.jpg", AppliedDate = new DateTime(2026, 2, 2), Status = "Tiếp nhận", StatusBadge = "badge-light-primary", History = new List<StatusHistory> { new() { Status = "Tiếp nhận", Date = new DateTime(2026, 2, 2) } } },
                new() { Id = 3, Name = "Phạm Văn Chung", Email = "chung.pv@gmail.com", Avatar = "assets/media/avatars/300-3.jpg", AppliedDate = new DateTime(2026, 1, 25), Status = "Đề nghị", StatusBadge = "badge-light-info", History = new List<StatusHistory> { new() { Status = "Đề nghị", Date = new DateTime(2026, 2, 7) } } },
                new() { Id = 4, Name = "Lê Thị Diệu", Email = "dieu.lt@gmail.com", Avatar = "assets/media/avatars/300-4.jpg", AppliedDate = new DateTime(2026, 2, 3), Status = "Từ chối", StatusBadge = "badge-light-danger", RejectionReason = "Kinh nghiệm chưa phù hợp", History = new List<StatusHistory> { new() { Status = "Từ chối", Date = new DateTime(2026, 2, 6) } } },
                new() { Id = 5, Name = "Hoàng Văn Hải", Email = "hai.hv@gmail.com", Avatar = "assets/media/avatars/300-5.jpg", AppliedDate = new DateTime(2026, 2, 4), Status = "Đã tuyển", StatusBadge = "badge-light-success", HiringNote = "Nhận việc từ 15/02", History = new List<StatusHistory> { new() { Status = "Đã tuyển", Date = new DateTime(2026, 2, 8) } } },
                new() { Id = 6, Name = "Ngô Thị Hoa", Email = "hoa.nt@gmail.com", Avatar = "assets/media/avatars/300-6.jpg", AppliedDate = new DateTime(2026, 2, 5), Status = "Tiếp nhận", StatusBadge = "badge-light-primary" },
                new() { Id = 7, Name = "Đặng Văn Khoa", Email = "khoa.dv@gmail.com", Avatar = "assets/media/avatars/300-7.jpg", AppliedDate = new DateTime(2026, 2, 6), Status = "Phỏng vấn", StatusBadge = "badge-light-warning" },
                new() { Id = 8, Name = "Bùi Văn Nam", Email = "nam.bv@gmail.com", Avatar = "assets/media/avatars/300-8.jpg", AppliedDate = new DateTime(2026, 2, 7), Status = "Tiếp nhận", StatusBadge = "badge-light-primary" },
                new() { Id = 9, Name = "Lý Văn Phát", Email = "phat.lv@gmail.com", Avatar = "assets/media/avatars/300-9.jpg", AppliedDate = new DateTime(2026, 2, 8), Status = "Phỏng vấn", StatusBadge = "badge-light-warning" },
                new() { Id = 10, Name = "Vũ Văn Quân", Email = "quan.vv@gmail.com", Avatar = "assets/media/avatars/300-10.jpg", AppliedDate = new DateTime(2026, 2, 8), Status = "Tiếp nhận", StatusBadge = "badge-light-primary" },
                new() { Id = 11, Name = "Trịnh Văn Sơn", Email = "son.tv@gmail.com", Avatar = "assets/media/avatars/300-11.jpg", AppliedDate = new DateTime(2026, 2, 8), Status = "Tiếp nhận", StatusBadge = "badge-light-primary" }
            };
        }

        protected void OpenAddApplicantModal()
        {
            NewApplicant = new ApplicantRequest { AppliedDate = DateTime.Today };
            IsAddApplicantModalOpen = true;
        }

        protected void CloseAddApplicantModal() => IsAddApplicantModalOpen = false;

        protected async System.Threading.Tasks.Task SaveApplicant()
        {
            Applicants.Insert(0, new ApplicantItem
            {
                Id = Applicants.Any() ? Applicants.Max(a => a.Id) + 1 : 1,
                Name = NewApplicant.Name,
                Email = NewApplicant.Email,
                Avatar = "assets/media/avatars/300-12.jpg",
                AppliedDate = NewApplicant.AppliedDate,
                Status = "Tiếp nhận",
                StatusBadge = "badge-light-primary"
            });
            if (Job != null) Job.ApplicantsCount++;
            CloseAddApplicantModal();
            await JS.InvokeVoidAsync("toastr.success", "Đã thêm ứng viên mới thành công!");
        }

        protected void UpdateStatus(ApplicantItem applicant, string newStatus, string? note = null)
        {
            if (newStatus == "Từ chối")
            {
                ApplicantToReject = applicant;
                RejectionReason = "";
                IsRejectionModalOpen = true;
                return;
            }

            if (newStatus == "Đã tuyển")
            {
                ApplicantToSuccess = applicant;
                SuccessNote = "";
                IsSuccessModalOpen = true;
                return;
            }

            ApplyStatusChange(applicant, newStatus, note);
        }

        protected void ApplyStatusChange(ApplicantItem applicant, string newStatus, string? note = null)
        {
            if (applicant.Status != newStatus || !string.IsNullOrEmpty(note))
            {
                if (applicant.Status != newStatus)
                {
                    applicant.Status = newStatus;
                    applicant.StatusBadge = newStatus switch
                    {
                        "Tiếp nhận" => "badge-light-primary",
                        "Phỏng vấn" => "badge-light-warning",
                        "Đã tuyển" => "badge-light-success",
                        "Từ chối" => "badge-light-danger",
                        _ => "badge-light-secondary"
                    };
                }

                applicant.History.Add(new StatusHistory { Status = newStatus, Date = DateTime.Now, Note = note });
                StateHasChanged();
            }
        }

        protected void CloseRejectionModal() => IsRejectionModalOpen = false;
        protected async System.Threading.Tasks.Task SaveRejection(string reason)
        {
            if (ApplicantToReject != null)
            {
                ApplicantToReject.RejectionReason = reason;
                ApplyStatusChange(ApplicantToReject, "Từ chối", $"Lý do loại: {reason}");
                IsRejectionModalOpen = false;
                await JS.InvokeVoidAsync("toastr.success", "Đã cập nhật trạng thái hồ sơ!");
            }
        }

        protected void CloseSuccessModal() => IsSuccessModalOpen = false;
        protected async System.Threading.Tasks.Task SaveSuccess(string note)
        {
            if (ApplicantToSuccess != null)
            {
                ApplicantToSuccess.HiringNote = note;
                ApplyStatusChange(ApplicantToSuccess, "Đã tuyển", $"Ghi chú tuyển dụng: {note}");
                IsSuccessModalOpen = false;
                await JS.InvokeVoidAsync("toastr.success", "Chúc mừng! Bạn đã hoàn tất tuyển dụng ứng viên.");
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

        protected async System.Threading.Tasks.Task ConfirmDeleteApplicant()
        {
            if (ApplicantToDelete != null)
            {
                Applicants.Remove(ApplicantToDelete);
                if (Job != null) Job.ApplicantsCount--;
                CloseDeleteModal();
                await JS.InvokeVoidAsync("toastr.success", "Đã xóa hồ sơ ứng viên.");
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
            SelectedApplicant = applicant;
            ScheduleModel = new InterviewScheduleModel
            {
                ApplicantId = applicant.Id,
                ApplicantName = applicant.Name,
                InterviewDate = applicant.InterviewDate ?? DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                InterviewTime = applicant.InterviewTime ?? new TimeOnly(9, 0),
                Interviewer = applicant.Interviewer ?? "Phan Thanh Tùng",
                Location = applicant.Location ?? "Văn phòng Tầng 5 - TTL Building"
            };
            IsScheduleModalOpen = true;
        }

        protected void CloseScheduleModal() => IsScheduleModalOpen = false;

        protected async System.Threading.Tasks.Task SaveInterviewSchedule(InterviewScheduleModel model)
        {
            if (SelectedApplicant != null)
            {
                string note = SelectedApplicant.HasInterviewScheduled ? "Cập nhật lịch phỏng vấn" : $"Đã hẹn phỏng vấn: {model.InterviewDate:dd/MM} lúc {model.InterviewTime}";
                
                // Cập nhật thông tin vào applicant hiện tại
                SelectedApplicant.InterviewDate = model.InterviewDate;
                SelectedApplicant.InterviewTime = model.InterviewTime;
                SelectedApplicant.Interviewer = model.Interviewer;
                SelectedApplicant.Location = model.Location;
                SelectedApplicant.HasInterviewScheduled = true;

                ApplyStatusChange(SelectedApplicant, "Phỏng vấn", note);
                
                IsScheduleModalOpen = false;
                await JS.InvokeVoidAsync("toastr.success", "Đã cập nhật lịch phỏng vấn thành công!");
            }
        }
    }
}
