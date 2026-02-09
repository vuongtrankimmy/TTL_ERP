using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TTL.HR.Shared.Entities.Base;

namespace TTL.HR.Shared.Entities.Payroll
{
    // Bảng cấu trúc lương (Phần tử lương)
    public class SalaryComponent : BaseEntity
    {
        public string Name { get; set; } = string.Empty; // Lương cơ bản, Phụ cấp xăng xe, BHXH...
        public string Code { get; set; } = string.Empty; // SAL-BASE, ALL-FUEL...
        public ComponentType Type { get; set; } // EARNING (Thu nhập) or DEDUCTION (Khấu trừ)
        public bool IsFixed { get; set; } // Cố định hàng tháng hay biến động?
        public bool IsTaxable { get; set; } // Chịu thuế TNCN không?
    }

    // Thời kỳ lương (Tháng/Năm)
    public class PayrollPeriod : BaseEntity
    {
        public string Name { get; set; } = string.Empty; // "Bảng lương Tháng 01/2026"
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool IsClosed { get; set; } // Đã đóng sổ chưa?
    }

    // Phiếu lương chi tiết của từng nhân viên
    public class SalarySlip : BaseEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string EmployeeId { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        public string PeriodId { get; set; } = string.Empty;

        public decimal TotalEarning { get; set; }
        public decimal TotalDeduction { get; set; }
        public decimal NetSalary { get; set; } // Thực lĩnh

        public List<SalaryDetail> Details { get; set; } = new();

        public SlipStatus Status { get; set; } = SlipStatus.Draft;
    }

    public class SalaryDetail
    {
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public ComponentType Type { get; set; }
    }

    public enum ComponentType
    {
        Earning, // Thu nhập
        Deduction, // Khấu trừ
        CompanyCost // Chi phí công ty (BHXH đóng thay NV)
    }

    public enum SlipStatus
    {
        Draft,
        Sent, // Đã gửi email cho NV check
        Confirmed, // NV xác nhận đúng
        Paid // Đã chuyển khoản
    }
}
