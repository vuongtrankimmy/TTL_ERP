using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace TTL.HR.Shared.Pages.User.Components
{
    public partial class UserLeaveHistory
    {
        private List<LeaveHistoryItem> _leaveHistory = new()
        {
            new() { Type = "Nghỉ phép năm", TypeColor = "badge-light-primary", DateRange = "10/02 - 12/02/2026", Duration = "3 ngày", Reason = "Giải quyết việc gia đình cá nhân quan trọng.", Status = "Đã phê duyệt", StatusColor = "badge-light-success" },
            new() { Type = "Nghỉ ốm", TypeColor = "badge-light-danger", DateRange = "05/02 - 06/02/2026", Duration = "2 ngày", Reason = "Bị sốt xuất huyết, có giấy bác sĩ.", Status = "Chờ phê duyệt", StatusColor = "badge-light-warning" },
            new() { Type = "Đi công tác", TypeColor = "badge-light-info", DateRange = "15/02 - 20/02/2026", Duration = "6 ngày", Reason = "Hỗ trợ triển khai dự án miền Nam.", Status = "Đã phê duyệt", StatusColor = "badge-light-success" },
            new() { Type = "Làm việc từ xa", TypeColor = "badge-light-dark", DateRange = "07/02/2026", Duration = "1 ngày", Reason = "Xử lý hồ sơ bảo mật tại nhà.", Status = "Đã phê duyệt", StatusColor = "badge-light-success" },
            new() { Type = "Nghỉ không lương", TypeColor = "badge-light-secondary", DateRange = "12/02 - 14/02/2026", Duration = "3 ngày", Reason = "Về quê có việc hiếu hỷ.", Status = "Đã từ chối", StatusColor = "badge-light-danger" },
            new() { Type = "Nghỉ thai sản", TypeColor = "badge-light-warning", DateRange = "01/02 - 01/08/2026", Duration = "6 tháng", Reason = "Nghỉ thai sản theo quy định.", Status = "Đã phê duyệt", StatusColor = "badge-light-success" },
            new() { Type = "Đi đào tạo", TypeColor = "badge-light-primary", DateRange = "22/02 - 25/02/2026", Duration = "4 ngày", Reason = "Khóa học Cloud Computing.", Status = "Chờ phê duyệt", StatusColor = "badge-light-warning" },
            new() { Type = "Nghỉ kết hôn", TypeColor = "badge-light-success", DateRange = "20/02 - 23/02/2026", Duration = "4 ngày", Reason = "Nghỉ kết hôn bản thân.", Status = "Đã phê duyệt", StatusColor = "badge-light-success" },
            new() { Type = "Nghỉ việc riêng", TypeColor = "badge-light-info", DateRange = "09/02/2026", Duration = "1 ngày", Reason = "Thủ tục hành chính.", Status = "Chờ phê duyệt", StatusColor = "badge-light-warning" },
            new() { Type = "Đi muộn", TypeColor = "badge-light-secondary", DateRange = "04/02/2026", Duration = "1 giờ", Reason = "Xe hỏng dọc đường.", Status = "Đã phê duyệt", StatusColor = "badge-light-success" }
        };

        public class LeaveHistoryItem
        {
            public string Type { get; set; } = "";
            public string TypeColor { get; set; } = "";
            public string DateRange { get; set; } = "";
            public string Duration { get; set; } = "";
            public string Reason { get; set; } = "";
            public string Status { get; set; } = "";
            public string StatusColor { get; set; } = "";
        }
    }
}
