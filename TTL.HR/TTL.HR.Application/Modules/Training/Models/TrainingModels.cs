using System;

namespace TTL.HR.Application.Modules.Training.Models
{
    public class CourseModel
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string TrainerName { get; set; } = "";
        public DateTime StartDate { get; set; }
        public int DurationHours { get; set; }
        public string Status { get; set; } = "";
    }

    public class ParticipantModel
    {
        public string EmployeeId { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Department { get; set; } = "";
        public string Position { get; set; } = "";
        public string AvatarUrl { get; set; } = "";
        public int Progress { get; set; }
        public string Status { get; set; } = "";
        public DateTime? EnrollmentDate { get; set; }
        public DateTime? CompletionDate { get; set; }
    }
}
