using System;
using System.Collections.Generic;
using TTL.HR.Application.Modules.Common.Entities; // Updated

namespace TTL.HR.Application.Modules.Recruitment.Entities
{
    public class JobPosting : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        
        public string DepartmentId { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public string Requirements { get; set; } = string.Empty;
        
        public int Quantity { get; set; }
        public int FilledQuantity { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public JobStatus Status { get; set; } = JobStatus.Draft;
        public string[] Skills { get; set; } = Array.Empty<string>();

        public string AssigneeId { get; set; } = string.Empty;
    }

    public class Candidate : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string ResumeUrl { get; set; } = string.Empty;
        
        public string JobPostingId { get; set; } = string.Empty;

        public CandidateStatus Status { get; set; } = CandidateStatus.New;
        public double? InterviewScore { get; set; }
        public string Notes { get; set; } = string.Empty;
    }

    public enum JobStatus
    {
        Draft,
        Published,
        Closed,
        OnHold
    }

    public enum CandidateStatus
    {
        New,
        Screening,
        InterviewScheduled,
        Offered,
        Hired,
        Rejected
    }
}
