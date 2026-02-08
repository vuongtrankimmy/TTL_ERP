using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TTL.HR.Shared.Entities.Base;

namespace TTL.HR.Shared.Entities.Contracts
{
    // Replaces TemplateItem
    public class ContractTemplate : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; } // QT-NS-01/2026
        public string Description { get; set; }
        public ContractType Type { get; set; }
        
        public string ContentHtml { get; set; } // For editor preview
        public string FilePath { get; set; } // Path to .docx template file
        
        public TemplateStatus Status { get; set; } = TemplateStatus.Draft;
        public string Icon { get; set; } = "bi bi-file-earmark-text";
        public string Color { get; set; } = "primary"; // UI color
    }
    
    public class EmployeeContract : BaseEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string EmployeeId { get; set; }
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string TemplateId { get; set; } // Derived from which template

        public string ContractNumber { get; set; } // 2026/LĐ-001
        
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; } // Null if permanent

        public decimal BaseSalary { get; set; }
        public decimal AllowanceTotal { get; set; }
        
        public ContractStatus Status { get; set; } = ContractStatus.Draft;
        public string SignedFileUrl { get; set; } // Scan of signed copy
    }

    public enum ContractType
    {
        Probation, // Thử việc
        Official_12M, // 1 Năm
        Official_36M, // 3 Năm
        Indefinite, // Không xác định thời hạn
        Collaborator, // Cộng tác viên
        Training, // Đào tạo
        NDA // Bảo mật
    }

    public enum TemplateStatus
    {
        Draft,
        Active,
        Archived
    }

    public enum ContractStatus
    {
        Draft,
        SentToEmployee,
        Signed,
        Expired,
        Terminated
    }
}
