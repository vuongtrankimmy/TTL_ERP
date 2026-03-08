using System;
using System.Collections.Generic;

namespace TTL.HR.Application.Modules.Recruitment.Models
{
    public class ApplicantItem
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public string JobPostingId { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int? StatusId { get; set; }
        public string Status { get; set; } = "Applied";
        public int? GenderId { get; set; }
        public string Gender { get; set; } = string.Empty;
        
        // Aliases for UI
        public string Name => FullName;
        public DateTime AppliedDate => CreatedAt;
        public string? StatusColor { get; set; }
        public string StatusBadge => $"badge-light-{GetStatusColor()}";

        private string GetStatusColor()
        {
            if (!string.IsNullOrEmpty(StatusColor)) return StatusColor;

            return StatusId switch
            {
                101 => "info",      // Applied
                102 => "primary",   // Screening
                103 => "warning",   // Interview
                104 => "success",   // Offered
                105 => "success",   // Hired
                106 => "danger",    // Rejected
                _ => "secondary"
            };
        }
        
        public int DaysInStage { get; set; }
        public bool HasInterviewScheduled { get; set; }

        // Interview Details
        public DateTime? InterviewDate { get; set; }
        public DateTime? InterviewTime { get; set; }
        public string Interviewer { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string RejectionReason { get; set; } = string.Empty;
        public string HiringNote { get; set; } = string.Empty;

        public List<InterviewDto> Interviews { get; set; } = new();
        public List<CandidateHistoryDto> History { get; set; } = new();
    }

    public class InterviewDto
    {
        public string Id { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public string Location { get; set; } = string.Empty;
        public string InterViewerName { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class CandidateHistoryDto
    {
        public int? FromStatusId { get; set; }
        public string FromStatus { get; set; } = string.Empty;
        public int? ToStatusId { get; set; }
        public string ToStatus { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }
        public string ChangedByName { get; set; } = string.Empty;

        // Aliases for UI
        public DateTime Date => ChangedAt;
        public string Status => ToStatus;
        public string Note => Comment;
    }

    public class InterviewScheduleModel
    {
        public string ApplicantId { get; set; } = string.Empty;
        public string ApplicantName { get; set; } = string.Empty;
        public string InterviewerId { get; set; } = "SYSTEM";
        public DateTime ScheduledAt { get; set; } = DateTime.Today.AddDays(1).AddHours(9);
        
        // For UI binding if needed, or keep for simplicity
        public DateOnly InterviewDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        public TimeOnly InterviewTime { get; set; } = new TimeOnly(9, 0);
        public string Location { get; set; } = "Văn phòng Tầng 5 - TTL Building";
        public string Interviewer { get; set; } = "Phan Thanh Tùng";
    }

    public class JobDetail
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        
        // Aliases for UI compatibility or update UI to use backend names
        public string Department => DepartmentName;
        public int HiringCount => Quantity;
        public int ApplicantsCount => AppliedCount;
        public DateTime Deadline => EndDate;

        public int Quantity { get; set; }
        public int FilledQuantity { get; set; }
        public int AppliedCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public int? StatusId { get; set; }
        public string Status { get; set; } = "Draft";
        public string? StatusColor { get; set; }

        public string StatusClass
        {
            get
            {
                if (!string.IsNullOrEmpty(StatusColor)) return $"badge-light-{StatusColor}";

                return StatusId switch
                {
                    82 => "badge-light-secondary", // Draft
                    81 => "badge-light-primary",   // Published
                    84 => "badge-light-warning",   // OnHold
                    83 => "badge-light-danger",    // Closed
                    85 => "badge-light-success",   // Completed
                    _ => "badge-light-secondary"
                };
            }
        }
        
        public string Description { get; set; } = string.Empty;
        public string Requirements { get; set; } = string.Empty;
        public string Type { get; set; } = "Full-time";
        public string Benefits { get; set; } = string.Empty;
        public List<string> Skills { get; set; } = new();
    }

    public class ApplicantRequest
    {
        public string JobPostingId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int? GenderId { get; set; }
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-25);
        public string CvUrl { get; set; } = string.Empty;
        public double ExperienceYears { get; set; }
        public string? Notes { get; set; }
        
        public DateTime AppliedDate { get; set; } = DateTime.Today;
        
        // Alias for UI compatibility
        public string Name { get => FullName; set => FullName = value; }
    }
}
