using System;
using System.Collections.Generic;

namespace TTL.HR.Application.Modules.Training.Models
{
    public class CourseModel
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Code { get; set; } = "";
        public string Description { get; set; } = "";
        public string Category { get; set; } = "General";
        public string Level { get; set; } = "Beginner";
        public string TrainerName { get; set; } = "";
        public decimal DurationHours { get; set; }
        public int MaxParticipants { get; set; }
        public bool IsMandatory { get; set; }
        public string Location { get; set; } = "";
        public string? MaterialUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string Status { get; set; } = "Draft";
        public List<string> Syllabus { get; set; } = new();
        public List<string> Attachments { get; set; } = new();
        public int EnrolledCount { get; set; }
        public List<ParticipantModel> EnrolledEmployees { get; set; } = new();
        public List<string> EnrolledEmployeeIds { get; set; } = new();
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ParticipantModel
    {
        public string Id { get; set; } = "";
        public string EmployeeId { get; set; } = "";
        
        [System.Text.Json.Serialization.JsonPropertyName("EmployeeName")]
        public string FullName { get; set; } = "";
        
        [System.Text.Json.Serialization.JsonPropertyName("EmployeeCode")]
        public string Code { get; set; } = "";
        
        [System.Text.Json.Serialization.JsonPropertyName("DepartmentName")]
        public string Department { get; set; } = "";
        
        [System.Text.Json.Serialization.JsonPropertyName("PositionName")]
        public string Position { get; set; } = "";
        
        public string AvatarUrl { get; set; } = "";
        public int Progress { get; set; }
        public int? StatusId { get; set; }
        public string Status { get; set; } = "Enrolled"; // Enrolled, InProgress, Completed, Failed
        
        [System.Text.Json.Serialization.JsonPropertyName("EnrolledDate")]
        public DateTime EnrollmentDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string? Note { get; set; }
    }

    public class TrainingAnalyticsModel
    {
        public int TotalCourses { get; set; }
        public int ActiveCourses { get; set; }
        public int TotalEnrollments { get; set; }
        public int CompletedEnrollments { get; set; }
        public double CompletionRate { get; set; }
        public List<CourseStatsModel> TopCourses { get; set; } = new();
        public List<MonthlyTrainingTrendModel> EnrollmentTrends { get; set; } = new();
    }

    public class CourseStatsModel
    {
        public string CourseName { get; set; } = "";
        public int EnrolledCount { get; set; }
        public double AvgScore { get; set; }
    }

    public class MonthlyTrainingTrendModel
    {
        public string Month { get; set; } = "";
        public int Enrolled { get; set; }
        public int Completed { get; set; }
    }
}
