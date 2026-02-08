using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TTL.HR.Shared.Entities.Base;

namespace TTL.HR.Shared.Entities.Recruitment
{
    public class JobPosting : BaseEntity
    {
        public string Title { get; set; }
        public string Code { get; set; } // DEV-001
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string DepartmentId { get; set; }
        public string DepartmentName { get; set; } // Denormalized for display

        public string Description { get; set; } // HTML/MarkDown
        public string Requirements { get; set; }
        
        public int Quantity { get; set; }
        public int FilledQuantity { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public JobStatus Status { get; set; } = JobStatus.Draft;
        public string[] Skills { get; set; } // [".NET", "SQL", "Blazor"]

        [BsonRepresentation(BsonType.ObjectId)]
        public string AssigneeId { get; set; } // Recruiter in charge
    }

    public class Candidate : BaseEntity
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ResumeUrl { get; set; } // Link to CV file
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string JobPostingId { get; set; }

        public CandidateStatus Status { get; set; } = CandidateStatus.New;
        public double? InterviewScore { get; set; }
        public string Notes { get; set; }
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
