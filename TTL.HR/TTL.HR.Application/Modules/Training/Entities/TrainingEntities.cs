using System;
using System.Collections.Generic;
using TTL.HR.Application.Modules.Common.Entities;

namespace TTL.HR.Application.Modules.Training.Entities
{
    public class Course : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DurationHours { get; set; }
        public string TrainerName { get; set; } = string.Empty;
        public bool IsMandatory { get; set; }
        public string MaterialUrl { get; set; } = string.Empty;
        public CourseStatus Status { get; set; } = CourseStatus.Active;
    }

    public class TrainingRecord : BaseEntity
    {
        public string EmployeeId { get; set; } = string.Empty;
        
        public string CourseId { get; set; } = string.Empty;

        public DateTime EnrolledDate { get; set; }
        public DateTime? CompletionDate { get; set; }

        public TrainingStatus RecordStatus { get; set; }
        public double? Score { get; set; }
        public string CertificateUrl { get; set; } = string.Empty;
        public string Feedback { get; set; } = string.Empty;
    }

    public enum CourseStatus { Draft, Active, Inactive }
    public enum TrainingStatus { Enrolled, InProgress, Completed, Failed, Cancelled }
}
