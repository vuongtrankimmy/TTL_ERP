using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TTL.HR.Shared.Entities.Base;

namespace TTL.HR.Shared.Entities.System
{
    // Quyền hạn nhỏ nhất (Ví dụ: "employee.create", "payroll.view")
    public class Permission : BaseEntity
    {
        public string Name { get; set; } = string.Empty; // "Xem danh sách nhân viên"
        public string Code { get; set; } = string.Empty; // "EMPLOYEE_VIEW"
        public string Module { get; set; } = string.Empty; // "HR", "PAYROLL", "SYSTEM"
        public string Description { get; set; } = string.Empty;
    }

    // Nhóm quyền (Ví dụ: "HR Manager", "Admin")
    public class Role : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty; // "HR_MGR"
        public string Description { get; set; } = string.Empty;
        public bool IsSystemRole { get; set; } // Role mặc định không thể xóa (vd: Admin)

        // Danh sách Permission ID được gán cho Role này
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> PermissionIds { get; set; } = new List<string>();
    }
}
