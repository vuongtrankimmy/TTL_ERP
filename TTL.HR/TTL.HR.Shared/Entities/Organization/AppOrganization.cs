using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TTL.HR.Shared.Entities.Base;

namespace TTL.HR.Shared.Entities.Organization
{
    public class Department : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string ParentId { get; set; } = string.Empty; // For hierarchical structure
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string ManagerId { get; set; } = string.Empty; // Head of Department (EmployeeId)

        public int Level { get; set; } // 0: Top, 1, 2...
        public string Path { get; set; } = string.Empty; // Materialized Path: /RootId/ChildId/
    }

    public class Position : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BaseSalaryRangeMin { get; set; }
        public decimal BaseSalaryRangeMax { get; set; }
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string DepartmentId { get; set; } = string.Empty; // Optional: link position to specific dept
    }
}
