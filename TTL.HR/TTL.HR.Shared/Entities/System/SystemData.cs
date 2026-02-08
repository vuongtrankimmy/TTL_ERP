using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TTL.HR.Shared.Entities.Base;

namespace TTL.HR.Shared.Entities.System
{
    public class SystemConfig : BaseEntity
    {
        public string CompanyName { get; set; }
        public string TaxCode { get; set; }
        public string Headquarters { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public string LogoUrl { get; set; }
        
        // HR defaults
        public int DefaultAnnualLeaveDays { get; set; } = 12;
        public int ProbationMonths { get; set; } = 2;
        public TimeSpan StandardCheckIn { get; set; } = new TimeSpan(8, 0, 0);
        public TimeSpan StandardCheckOut { get; set; } = new TimeSpan(17, 30, 0);
    }

    public class AuditLog : BaseEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }
        public string Username { get; set; }
        
        public string Action { get; set; } // "CREATE", "UPDATE", "DELETE", "LOGIN"
        public string EntityName { get; set; } // "Employee", "Contract"
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string EntityId { get; set; }
        
        public string OldValueJson { get; set; }
        public string NewValueJson { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
    }
}
