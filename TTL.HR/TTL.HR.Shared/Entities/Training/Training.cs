using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TTL.HR.Shared.Entities.Base;

namespace TTL.HR.Shared.Entities.Training
{
    // Khóa đào tạo (Catalogue)
    public class Course : BaseEntity
    {
        public string Title { get; set; } = string.Empty; // Kỹ năng mềm, Excel nâng cao...
        public string Code { get; set; } = string.Empty; // SOFT-01, EXCEL-PRO...
        public string Description { get; set; } = string.Empty;
        public int DurationHours { get; set; }
        
        public string TrainerName { get; set; } = string.Empty; // Giảng viên nội bộ hay thuê ngoài
        public bool IsMandatory { get; set; } // Có bắt buộc không?
        
        // Tài liệu khóa học (link)
        public string MaterialUrl { get; set; } = string.Empty;
        
        public CourseStatus Status { get; set; } = CourseStatus.Active;
    }

    // Hồ sơ tham gia của NV
    public class TrainingRecord : BaseEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string EmployeeId { get; set; } = string.Empty;
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string CourseId { get; set; } = string.Empty;

        public DateTime EnrolledDate { get; set; }
        public DateTime? CompletionDate { get; set; }

        public TrainingStatus RecordStatus { get; set; }
        public double? Score { get; set; } // Điểm số (nếu có thi)
        public string CertificateUrl { get; set; } = string.Empty; // Chứng chỉ hoàn thành (PDF)
        public string Feedback { get; set; } = string.Empty; // NV đánh giá khóa học
    }

    public enum CourseStatus
    {
        Draft,
        Active,
        Inactive
    }

    public enum TrainingStatus
    {
        Enrolled,
        InProgress,
        Completed,
        Failed,
        Cancelled
    }
}
