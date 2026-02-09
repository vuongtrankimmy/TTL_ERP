using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TTL.HR.Shared.Entities.Base;

namespace TTL.HR.Shared.Entities.System
{
    public class SystemConfig : BaseEntity
    {
        public string CompanyName { get; set; } = string.Empty;
        public string TaxCode { get; set; } = string.Empty;
        public string Headquarters { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        
        // HR defaults
        public int DefaultAnnualLeaveDays { get; set; } = 12;
        public int ProbationMonths { get; set; } = 2;
        public TimeSpan StandardCheckIn { get; set; } = new TimeSpan(8, 0, 0);
        public TimeSpan StandardCheckOut { get; set; } = new TimeSpan(17, 30, 0);
    }

    public class AuditLog : BaseEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        
        public string Action { get; set; } = string.Empty; // "CREATE", "UPDATE", "DELETE", "LOGIN"
        public string EntityName { get; set; } = string.Empty; // "Employee", "Contract"
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string EntityId { get; set; } = string.Empty;
        
        public string OldValueJson { get; set; } = string.Empty;
        public string NewValueJson { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
    }
}
