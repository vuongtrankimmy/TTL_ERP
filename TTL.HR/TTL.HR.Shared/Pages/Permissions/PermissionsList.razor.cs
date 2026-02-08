using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using TTL.HR.Shared.Models;

namespace TTL.HR.Shared.Pages.Permissions
{
    public partial class PermissionsList
    {
        private bool _showRoleDrawer = false;
        private bool _showDetailDrawer = false;
        private PermissionCategory? _selectedCategory;

        private void openAddRole() => _showRoleDrawer = true;
        private void closeRoleDrawer() => _showRoleDrawer = false;

        private void openPermissionDetail(PermissionCategory cat) {
            _selectedCategory = cat;
            _showDetailDrawer = true;
        }
        private void closeDetailDrawer() => _showDetailDrawer = false;

        private List<PermissionCategory> _permissions = new()
        {
            new("Quản lý Nhân sự", "Hồ sơ, Sơ đồ tổ chức, Danh sách nhân viên", "ki-people", "bg-light-primary", "text-primary", new bool[] { true, true, true, false, false }, new() { new("Xem danh sách", "hr.view"), new("Thêm mới nhân viên", "hr.create"), new("Sửa thông tin hồ sơ", "hr.edit"), new("Xóa nhân sự", "hr.delete"), new("Xuất báo cáo Excel", "hr.export") }),
            new("Bảng lương & Thuế", "Tính lương, Chốt lương, Thuế TNCN & BHXH", "ki-wallet", "bg-light-success", "text-success", new bool[] { true, true, false, false, false }, new() { new("Xem bảng lương", "payroll.view"), new("Tính toán lương tháng", "payroll.calc"), new("Chốt sổ lương", "payroll.close"), new("Gửi phiếu lương", "payroll.send") }),
            new("Chấm công & Ca làm", "Dữ liệu máy chấm công, Phân ca, Chốt công", "ki-timer", "bg-light-warning", "text-warning", new bool[] { true, true, true, true, false }, new() { new("Xem dữ liệu công", "att.view"), new("Phân ca làm việc", "att.schedule"), new("Chốt vân tay", "att.sync"), new("Sửa giờ công", "att.edit") }),
            new("Quản lý Nghỉ phép", "Duyệt đơn, Cấu hình loại nghỉ, Quỹ phép", "ki-calendar", "bg-light-info", "text-info", new bool[] { true, true, true, false, false }, new() { new("Duyệt đơn nghỉ", "leave.approve"), new("Cấu hình quỹ phép", "leave.config"), new("Xem lịch nghỉ toàn công ty", "leave.calendar") }),
            new("Phụ cấp & Phúc lợi", "Cấu hình phụ cấp, Danh sách phúc lợi năm", "ki-shop", "bg-light-danger", "text-danger", new bool[] { true, true, false, false, false }, new() { new("Thêm phụ cấp mới", "ben.create"), new("Gán phúc lợi nhân viên", "ben.assign"), new("Quản lý bảo hiểm PVI", "ben.insurance") }),
            new("Hợp đồng lao động", "Ký mới, Gia hạn, Chấm dứt phụ lục hợp đồng", "ki-file-added", "bg-light-primary", "text-primary", new bool[] { true, true, false, false, false }, new() { new("Tạo hợp đồng", "contract.create"), new("Gia hạn tự động", "contract.renew"), new("In hợp đồng PDF", "contract.print") }),
            new("Tuyển dụng & Onboarding", "Kế hoạch tuyển, Phỏng vấn, Quy trình thử việc", "ki-user-tick", "bg-light-success", "text-success", new bool[] { true, true, true, false, false }, new() { new("Đăng tin tuyển dụng", "rec.post"), new("Đánh giá ứng viên", "rec.eval"), new("Chuyển trạng thái Onboarding", "rec.onboard") }),
            new("Đào tạo & Phát triển", "Khóa học nội bộ, Đánh giá năng lực, Chứng chỉ", "ki-book-open", "bg-light-info", "text-info", new bool[] { true, true, true, true, false }, new() { new("Tạo khóa đào tạo", "edu.create"), new("Cấp chứng chỉ", "edu.cert"), new("Đánh giá sau đào tạo", "edu.eval") }),
            new("Báo cáo & Dashboard", "Truy cập báo cáo tổng hợp, Biểu đồ thống kê", "ki-graph-3", "bg-light-warning", "text-warning", new bool[] { true, true, true, true, false }, new() { new("Xem dashboard HR", "report.dashboard"), new("Báo cáo biến động nhân sự", "report.turnover"), new("Báo cáo chi phí lương", "report.cost") }),
            new("Cấu hình Hệ thống", "Phân quyền, Log hệ thống, API, Thông tin công ty", "ki-setting-2", "bg-light-dark", "text-dark", new bool[] { true, false, false, false, false }, new() { new("Cấu hình phân quyền", "sys.perm"), new("Xem log hệ thống", "sys.log"), new("Cài đặt API kết nối", "sys.api") })
        };

        private class PermissionCategory
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Icon { get; set; }
            public string IconBg { get; set; }
            public string IconColor { get; set; }
            public bool[] AccessDefaults { get; set; }
            public List<SubPermission> SubPermissions { get; set; }
            
            public PermissionCategory(string name, string desc, string icon, string bg, string color, bool[] defaults, List<SubPermission> subs) {
                Name = name; Description = desc; Icon = icon; IconBg = bg; IconColor = color; AccessDefaults = defaults; SubPermissions = subs;
            }
        }

        private class SubPermission
        {
            public string Action { get; set; }
            public string Key { get; set; }
            public bool DefaultValue { get; set; } = true;
            public SubPermission(string action, string key) { Action = action; Key = key; }
        }
    }
}
