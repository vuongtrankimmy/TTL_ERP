using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TTL.HR.Shared.Entities.Base;

namespace TTL.HR.Shared.Entities.Recruitment
{
    public class JobPosting : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty; // DEV-001
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string DepartmentId { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty; // Denormalized for display

        public string Description { get; set; } = string.Empty; // HTML/MarkDown
        public string Requirements { get; set; } = string.Empty;
        
        public int Quantity { get; set; }
        public int FilledQuantity { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public JobStatus Status { get; set; } = JobStatus.Draft;
        public string[] Skills { get; set; } = Array.Empty<string>(); // [".NET", "SQL", "Blazor"]

        [BsonRepresentation(BsonType.ObjectId)]
        public string AssigneeId { get; set; } = string.Empty; // Recruiter in charge
    }

    public class Candidate : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string ResumeUrl { get; set; } = string.Empty; // Link to CV file
        
        [BsonRepresentation(BsonType.ObjectId)]
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
