using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TTL.HR.Shared.Entities.Base;

namespace TTL.HR.Shared.Entities.Assets
{
    // Danh mục tài sản
    public class CompanyAsset : BaseEntity
    {
        public string Name { get; set; } // Laptop Dell XPS 15
        public string Code { get; set; } // ASSET-IT-001
        public string SerialNumber { get; set; }
        public AssetType Type { get; set; } // Laptop, Monitor, Chair
        public DateTime PurchaseDate { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal DepreciationPerYear { get; set; } // Khấu hao
        
        public AssetStatus Status { get; set; } = AssetStatus.Available;
        public string CurrentlyAssignedTo { get; set; } // Nếu đang dùng thì ai giữ?
    }

    // Lịch sử cấp phát/thu hồi
    public class AssetAllocation : BaseEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string AssetId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string EmployeeId { get; set; }

        public DateTime HandoverDate { get; set; } // Ngày bàn giao
        public string HandoverCondition { get; set; } // Tình trạng: Mới 100%

        public DateTime? ReturnDate { get; set; } // Ngày trả lại (khi nghỉ việc)
        public string ReturnCondition { get; set; } // Tình trạng: Trầy xước nhẹ

        public string Remarks { get; set; }
    }

    public enum AssetType
    {
        Computer, // Laptop, Desktop
        Accessory, // Chuột, phím
        Furniture, // Bàn ghế
        Vehicle, // Xe công ty
        Uniform // Đồng phục
    }

    public enum AssetStatus
    {
        Available, // Trong kho
        Assigned, // Đang sử dụng
        Repairing, // Đang sửa chữa
        Dispose, // Thanh lý
        Lost // Mất
    }
}
