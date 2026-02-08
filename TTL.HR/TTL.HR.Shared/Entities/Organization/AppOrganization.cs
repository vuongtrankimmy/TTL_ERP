using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TTL.HR.Shared.Entities.Base;

namespace TTL.HR.Shared.Entities.Organization
{
    public class Department : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string ParentId { get; set; } // For hierarchical structure
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string ManagerId { get; set; } // Head of Department (EmployeeId)

        public int Level { get; set; } // 0: Top, 1, 2...
        public string Path { get; set; } // Materialized Path: /RootId/ChildId/
    }

    public class Position : BaseEntity
    {
        public string Title { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal BaseSalaryRangeMin { get; set; }
        public decimal BaseSalaryRangeMax { get; set; }
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string DepartmentId { get; set; } // Optional: link position to specific dept
    }
}
