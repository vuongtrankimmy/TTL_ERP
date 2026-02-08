using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TTL.HR.Shared.Pages.Recruitment
{
    public partial class RecruitmentKanban
    {
        [Inject] public NavigationManager Navigation { get; set; }
        [Inject] public IJSRuntime JS { get; set; }

        protected string SearchTerm { get; set; } = "";
        protected int? SelectedJobId { get; set; }

        protected List<KanbanColumn> KanbanBoard = new();
        protected List<JobItem> Jobs = new();
        protected List<ApplicantItem> AllApplicants = new();
        
        // Modal States
        protected bool IsProfileModalOpen = false;
        protected bool IsScheduleModalOpen = false;
        protected bool IsRejectionModalOpen = false;
        protected bool IsSuccessModalOpen = false;

        protected ApplicantItem? SelectedApplicant;
        protected ApplicantItem? ApplicantToReject;
        protected ApplicantItem? ApplicantToSuccess;
        protected InterviewScheduleModel ScheduleModel = new();

        protected override void OnInitialized()
        {
            Jobs = new List<JobItem>
            {
                new() { Id = 1, Title = "Senior .NET Developer" },
                new() { Id = 2, Title = "HR Specialist" },
                new() { Id = 3, Title = "Sales Executive" },
                new() { Id = 4, Title = "Marketing Intern" },
                new() { Id = 5, Title = "Senior UI/UX Designer" }
            };

            LoadMockData();
            UpdateKanbanView();
        }

        private void LoadMockData()
        {
            AllApplicants = new List<ApplicantItem>
            {
                new() { Id = 1, Name = "Nguyễn Văn Anh", Email = "anh.nv@gmail.com", Avatar = "assets/media/avatars/300-1.jpg", JobId = 1, JobTitle = "Senior .NET Developer", Status = "Phỏng vấn", AppliedDate = new DateTime(2026, 2, 1), DaysInStage = 2, HasInterviewScheduled = true, InterviewDate = new DateOnly(2026, 2, 12), InterviewTime = new TimeOnly(14, 30), History = new List<StatusHistory> { new() { Status = "Tiếp nhận", Date = new DateTime(2026, 2, 1) } } },
                new() { Id = 2, Name = "Trần Thị Bình", Email = "binh.tt@outlook.com", Avatar = "assets/media/avatars/300-2.jpg", JobId = 1, JobTitle = "Senior .NET Developer", Status = "Tiếp nhận", AppliedDate = new DateTime(2026, 2, 5), DaysInStage = 1, History = new List<StatusHistory> { new() { Status = "Tiếp nhận", Date = new DateTime(2026, 2, 5) } } },
                new() { Id = 3, Name = "Lê Văn Cường", Email = "cuong.lv@gmail.com", Avatar = "assets/media/avatars/300-3.jpg", JobId = 2, JobTitle = "HR Specialist", Status = "Tiếp nhận", AppliedDate = new DateTime(2026, 2, 3), DaysInStage = 3, History = new List<StatusHistory> { new() { Status = "Tiếp nhận", Date = new DateTime(2026, 2, 3) } } },
                new() { Id = 4, Name = "Phạm Thị Dung", Email = "dung.pt@gmail.com", Avatar = "assets/media/avatars/300-4.jpg", JobId = 3, JobTitle = "Sales Executive", Status = "Đề nghị", AppliedDate = new DateTime(2026, 1, 28), DaysInStage = 5, History = new List<StatusHistory> { new() { Status = "Tiếp nhận", Date = new DateTime(2026, 1, 28) }, new() { Status = "Phỏng vấn", Date = new DateTime(2026, 2, 2) }, new() { Status = "Đề nghị", Date = new DateTime(2026, 2, 6) } } },
                new() { Id = 5, Name = "Hoàng Văn Em", Email = "em.hv@gmail.com", Avatar = "assets/media/avatars/300-5.jpg", JobId = 5, JobTitle = "Senior UI/UX Designer", Status = "Đã tuyển", AppliedDate = new DateTime(2026, 1, 15), DaysInStage = 10, HiringNote = "Bắt đầu làm việc từ 01/3/2026", History = new List<StatusHistory> { new() { Status = "Tiếp nhận", Date = new DateTime(2026, 1, 15) }, new() { Status = "Đã tuyển", Date = new DateTime(2026, 2, 7) } } },
                new() { Id = 6, Name = "Vũ Thị Giang", Email = "giang.vt@gmail.com", Avatar = "assets/media/avatars/300-6.jpg", JobId = 1, JobTitle = "Senior .NET Developer", Status = "Từ chối", AppliedDate = new DateTime(2026, 2, 2), DaysInStage = 4, RejectionReason = "Không đạt bài test kỹ thuật", History = new List<StatusHistory> { new() { Status = "Tiếp nhận", Date = new DateTime(2026, 2, 2) }, new() { Status = "Từ chối", Date = new DateTime(2026, 2, 8) } } },
                new() { Id = 7, Name = "Đặng Văn Hùng", Email = "hung.dv@gmail.com", Avatar = "assets/media/avatars/300-7.jpg", JobId = 4, JobTitle = "Marketing Intern", Status = "Phỏng vấn", AppliedDate = new DateTime(2026, 2, 6), DaysInStage = 1, History = new List<StatusHistory> { new() { Status = "Tiếp nhận", Date = new DateTime(2026, 2, 6) } } },
                new() { Id = 8, Name = "Bùi Thị Kim", Email = "kim.bt@gmail.com", Avatar = "assets/media/avatars/300-8.jpg", JobId = 2, JobTitle = "HR Specialist", Status = "Tiếp nhận", AppliedDate = new DateTime(2026, 2, 7), DaysInStage = 1, History = new List<StatusHistory> { new() { Status = "Tiếp nhận", Date = new DateTime(2026, 2, 7) } } },
                new() { Id = 9, Name = "Ngô Văn Long", Email = "long.nv@gmail.com", Avatar = "assets/media/avatars/300-9.jpg", JobId = 3, JobTitle = "Sales Executive", Status = "Đề nghị", AppliedDate = new DateTime(2026, 2, 2), DaysInStage = 4, History = new List<StatusHistory> { new() { Status = "Phỏng vấn", Date = new DateTime(2026, 2, 5) } } },
                new() { Id = 10, Name = "Lý Thị Mai", Email = "mai.lt@gmail.com", Avatar = "assets/media/avatars/300-10.jpg", JobId = 5, JobTitle = "Senior UI/UX Designer", Status = "Phỏng vấn", AppliedDate = new DateTime(2026, 2, 4), DaysInStage = 3, History = new List<StatusHistory> { new() { Status = "Tiếp nhận", Date = new DateTime(2026, 2, 4) } } },
                new() { Id = 11, Name = "Phan Văn Nam", Email = "nam.pv@gmail.com", Avatar = "assets/media/avatars/300-11.jpg", JobId = 1, JobTitle = "Senior .NET Developer", Status = "Tiếp nhận", AppliedDate = new DateTime(2026, 2, 8), DaysInStage = 0, History = new List<StatusHistory> { new() { Status = "Tiếp nhận", Date = new DateTime(2026, 2, 8) } } },
                new() { Id = 12, Name = "Đỗ Thị Ngọc", Email = "ngoc.dt@gmail.com", Avatar = "assets/media/avatars/300-12.jpg", JobId = 4, JobTitle = "Marketing Intern", Status = "Tiếp nhận", AppliedDate = new DateTime(2026, 2, 8), DaysInStage = 0, History = new List<StatusHistory> { new() { Status = "Tiếp nhận", Date = new DateTime(2026, 2, 8) } } }
            };
        }

        protected void UpdateKanbanView()
        {
            var filtered = AllApplicants
                .Where(a => string.IsNullOrEmpty(SearchTerm) || a.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                .Where(a => !SelectedJobId.HasValue || a.JobId == SelectedJobId.Value)
                .ToList();

            KanbanBoard = new List<KanbanColumn>
            {
                new() { Title = "Tiếp nhận", Status = "Tiếp nhận", ColorClass = "primary", Applicants = filtered.Where(a => a.Status == "Tiếp nhận").ToList() },
                new() { Title = "Phỏng vấn", Status = "Phỏng vấn", ColorClass = "warning", Applicants = filtered.Where(a => a.Status == "Phỏng vấn").ToList() },
                new() { Title = "Đề nghị", Status = "Đề nghị", ColorClass = "info", Applicants = filtered.Where(a => a.Status == "Đề nghị").ToList() },
                new() { Title = "Kết thúc", Status = "Kết thúc", ColorClass = "success", Applicants = filtered.Where(a => a.Status == "Đã tuyển" || a.Status == "Từ chối").ToList() }
            };
            StateHasChanged();
        }

        protected void OnSearch(ChangeEventArgs e)
        {
            SearchTerm = e.Value?.ToString() ?? "";
            UpdateKanbanView();
        }

        protected void OnJobSelected(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int id)) SelectedJobId = id;
            else SelectedJobId = null;
            UpdateKanbanView();
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
                UpdateKanbanView();
            }
        }

        protected void ViewProfile(ApplicantItem applicant)
        {
            SelectedApplicant = applicant;
            IsProfileModalOpen = true;
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
            IsProfileModalOpen = false;
        }

        protected async Task SaveInterviewSchedule(InterviewScheduleModel model)
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
                
                // Hiển thị thông báo
                await JS.InvokeVoidAsync("toastr.success", "Đã cập nhật lịch phỏng vấn thành công!");
            }
        }

        protected async Task SaveRejection(string reason)
        {
            if (ApplicantToReject != null)
            {
                ApplicantToReject.RejectionReason = reason;
                ApplyStatusChange(ApplicantToReject, "Từ chối", $"Lý do loại: {reason}");
                IsRejectionModalOpen = false;
                await JS.InvokeVoidAsync("toastr.success", "Đã cập nhật trạng thái hồ sơ!");
            }
        }

        protected async Task SaveSuccess(string note)
        {
            if (ApplicantToSuccess != null)
            {
                ApplicantToSuccess.HiringNote = note;
                ApplyStatusChange(ApplicantToSuccess, "Đã tuyển", $"Ghi chú tuyển dụng: {note}");
                IsSuccessModalOpen = false;
                await JS.InvokeVoidAsync("toastr.success", "Chúc mừng! Bạn đã hoàn tất tuyển dụng ứng viên.");
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
