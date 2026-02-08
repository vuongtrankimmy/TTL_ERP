using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace TTL.HR.Shared.Pages.Organization
{
    public partial class PositionList
    {
        private bool _showEditDrawer = false;
        private bool _showDetailDrawer = false;
        private bool _showConfirmDeleteModal = false;
        private bool _isNewPosition = true;
        
        private string _searchTerm = "";
        private string _levelFilter = "All";
        private bool _isLoading = true;

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            // Simulate API loading
            await System.Threading.Tasks.Task.Delay(1350);
            _isLoading = false;
        }

        
        private PositionItem _editingPos = new();
        private PositionItem? _selectedPos;
        private PositionItem? _positionToDelete;
        
        private List<string> _availableIcons = new() {
            "ki-outline ki-briefcase", "ki-outline ki-award", "ki-outline ki-person-badge",
            "ki-outline ki-palette", "ki-outline ki-megaphone", "ki-outline ki-shield-check",
            "ki-outline ki-bank", "ki-outline ki-graph-up"
        };

        private List<PositionItem> _positions = new()
        {
            new("Kỹ sư phần mềm cao cấp", "Senior Software Engineer", "Công nghệ thông tin", "Level 4", "badge-light-primary", 12, "25-35M", "ki-outline ki-briefcase", "bg-light-info", "text-info") {
                JobDescription = "Phát triển hệ thống ERP doanh nghiệp\nThiết kế microservices architecture\nCode review và hướng dẫn junior",
                Requirements = "Tốt nghiệp ĐH chuyên ngành CNTT\nTrên 5 năm kinh nghiệm .NET Core/React\nCó kinh nghiệm triển khai Cloud Azure/AWS",
                Benefits = "Lương thưởng tháng 13, 14\nBảo hiểm sức khỏe PVI Premium\nDu lịch hàng năm cùng công ty"
            },
            new("Trưởng phòng kinh doanh", "Sales Manager", "Kinh doanh", "Manager", "badge-light-danger", 3, "20-30M", "ki-outline ki-award", "bg-light-warning", "text-warning") {
                JobDescription = "Xây dựng chiến lược kinh doanh quý/năm\nQuản lý và đào tạo đội ngũ sales 20 người\nTìm kiếm đối tác chiến lược",
                Requirements = "Tốt nghiệp ĐH các khối ngành kinh tế\nKỹ năng đàm phán và thuyết phục tốt\nÍt nhất 3 năm kinh nghiệm quản lý",
                Benefits = "Thưởng KPI doanh số không giới hạn\nXe đưa đón công tác\nPhụ cấp điện thoại, tiếp khách"
            },
            new("Nhân viên hành chính", "Administrative Officer", "Hành chính - Nhân sự", "Level 1", "badge-light-success", 5, "10-15M", "ki-outline ki-person-badge", "bg-light-primary", "text-primary") {
                JobDescription = "Quản lý văn phòng phẩm và tài sản\nTiếp đón khách và quản lý tổng đài\nHỗ trợ tổ chức sự kiện nội bộ",
                Requirements = "Tốt nghiệp Cao đẳng/Đại học\nNgoại hình sáng, giao tiếp tốt\nSử dụng thành thạo Office",
                Benefits = "Môi trường trẻ trung năng động\nPhụ cấp ăn trưa\nĐào tạo kỹ năng mềm"
            },
            new("Thiết kế UI/UX", "UI/UX Designer", "Thiết kế", "Level 3", "badge-light-primary", 4, "18-25M", "ki-outline ki-palette", "bg-light-success", "text-success"),
            new("Chuyên viên Marketing", "Marketing Specialist", "Marketing", "Level 2", "badge-light-primary", 8, "12-18M", "ki-outline ki-megaphone", "bg-light-danger", "text-danger"),
            new("Chuyên viên Pháp chế", "Legal Assistant", "Ban Giám Đốc", "Level 3", "badge-light-primary", 2, "20-28M", "ki-outline ki-shield-check", "bg-light-primary", "text-primary"),
            new("Kế toán tổng hợp", "General Accountant", "Tài chính - Kế toán", "Level 2", "badge-light-primary", 4, "15-22M", "ki-outline ki-bank", "bg-light-info", "text-info")
        };

        private IEnumerable<PositionItem> FilteredPositions => _positions
            .Where(x => string.IsNullOrEmpty(_searchTerm) || x.NameVN.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) || x.NameEN.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase))
            .Where(x => _levelFilter == "All" || x.LevelName.Contains(_levelFilter));

        private void resetFilters() { _searchTerm = ""; _levelFilter = "All"; }

        private void openAddPosition() {
            _isNewPosition = true;
            _editingPos = new PositionItem { LevelName = "Level 1", LevelBadge = "badge-light-primary", Icon = "ki-outline ki-briefcase", IconBg = "bg-light-primary", IconColor = "text-primary" };
            _showEditDrawer = true;
            _showDetailDrawer = false;
        }

        private void editPosition(PositionItem pos) {
            _isNewPosition = false;
            _editingPos = pos;
            _showEditDrawer = true;
            _showDetailDrawer = false;
        }

        private void viewDetails(PositionItem pos) {
            _selectedPos = pos;
            _showDetailDrawer = true;
            _showEditDrawer = false;
        }

        private void closeEditDrawer() => _showEditDrawer = false;
        private void closeDetailDrawer() => _showDetailDrawer = false;

        private void savePosition() {
            if (_isNewPosition) {
                _positions.Insert(0, _editingPos);
            }
            closeEditDrawer();
        }

        private void requestDeletePosition(PositionItem pos) {
            _positionToDelete = pos;
            _showConfirmDeleteModal = true;
        }

        private void closeDeleteModal() {
            _showConfirmDeleteModal = false;
            _positionToDelete = null;
        }

        private void confirmDeletePosition() {
            if (_positionToDelete != null) {
                _positions.Remove(_positionToDelete);
                closeDeleteModal();
            }
        }

        public class PositionItem
        {
            public string NameVN { get; set; } = "";
            public string NameEN { get; set; } = "";
            public string Group { get; set; } = "";
            public string LevelName { get; set; } = "";
            public string LevelBadge { get; set; } = "badge-light-primary";
            public int ActiveEmployees { get; set; }
            public string SalaryRange { get; set; } = "Thỏa thuận";
            public string JobDescription { get; set; } = "";
            public string Requirements { get; set; } = "";
            public string Benefits { get; set; } = "";
            public string Icon { get; set; } = "ki-outline ki-briefcase";
            public string IconBg { get; set; } = "bg-light-primary";
            public string IconColor { get; set; } = "text-primary";

            public PositionItem() { }
            public PositionItem(string vn, string en, string group, string level, string badge, int count, string salary, string icon, string bg, string color) {
                NameVN = vn; NameEN = en; Group = group; LevelName = level; LevelBadge = badge; ActiveEmployees = count; SalaryRange = salary;
                Icon = icon; IconBg = bg; IconColor = color;
            }
        }
    }
}
