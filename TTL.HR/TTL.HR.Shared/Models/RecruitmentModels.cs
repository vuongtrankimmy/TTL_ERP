using System;
using System.Collections.Generic;

namespace TTL.HR.Shared.Models
{
    public class ApplicantItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Avatar { get; set; } = "";
        public int JobId { get; set; }
        public string JobTitle { get; set; } = "";
        public DateTime AppliedDate { get; set; }
        public string Status { get; set; } = "";
        public string StatusBadge { get; set; } = "";
        public int DaysInStage { get; set; }

        public bool HasInterviewScheduled { get; set; }
        public DateOnly? InterviewDate { get; set; }
        public TimeOnly? InterviewTime { get; set; }
        public string? Interviewer { get; set; }
        public string? Location { get; set; }

        public string? RejectionReason { get; set; }
        public string? HiringNote { get; set; }
        public List<StatusHistory> History { get; set; } = new();
    }

    public class StatusHistory
    {
        public DateTime Date { get; set; }
        public string Status { get; set; } = "";
        public string? Note { get; set; }
    }

    public class InterviewScheduleModel
    {
        public int ApplicantId { get; set; }
        public string ApplicantName { get; set; } = "";
        public DateOnly InterviewDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        public TimeOnly InterviewTime { get; set; } = new TimeOnly(9, 0);
        public string Location { get; set; } = "Văn phòng Tầng 5 - TTL Building";
        public string Interviewer { get; set; } = "Phan Thanh Tùng";
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
        public string Status { get; set; } = "";
        public string StatusClass { get; set; } = "";
        
        // Fields from JobFormModel
        public string Description { get; set; } = "";
        public string Requirements { get; set; } = "";
        public string Benefits { get; set; } = "";
    }

    public class ApplicantRequest
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public DateTime AppliedDate { get; set; } = DateTime.Today;
    }
}
