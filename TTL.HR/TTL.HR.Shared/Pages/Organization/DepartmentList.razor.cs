using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace TTL.HR.Shared.Pages.Organization
{
    public partial class DepartmentList
    {
        private bool _showEditDrawer = false;
        private bool _showDetailDrawer = false;
        private bool _showConfirmDeleteModal = false;
        private bool _isNewDepartment = true;
        private string _searchTerm = "";
        private string _statusFilter = "All";
        private bool _isLoading = true;
        private DepartmentItem _editingDept = new();
        private DepartmentItem? _selectedDept;

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            // Simulate API loading
            await System.Threading.Tasks.Task.Delay(1150);
            _isLoading = false;
        }
        private DepartmentItem? _departmentToDelete;

        private List<string> _availableIcons = new() { 
            "ki-outline ki-code", "ki-outline ki-people", "ki-outline ki-chart-line-up", 
            "ki-outline ki-gear", "ki-outline ki-shield-tick", "ki-outline ki-briefcase" 
        };

        private List<DepartmentItem> _departments = new()
        {
            new("Phòng Kỹ Thuật (Engineering)", "ENG-001", "Khối Công Nghệ (CTO)", "Nguyễn Văn Lộc", "Tech Lead", "assets/media/avatars/300-1.jpg", 42, 50, true, "ki-outline ki-code", "bg-light-primary", "text-primary", "bg-primary"),
            new("Phòng Nhân Sự (HR)", "HR-001", "Khối Vận Hành (COO)", "Lê Thị Lan", "HR Manager", null, 8, 10, true, "ki-outline ki-people", "bg-light-success", "text-success", "bg-success"),
            new("Phòng Kinh Doanh (Sales)", "SAL-001", "Khối Kinh Doanh (CBDO)", "Trần Minh Tâm", "Sales Director", null, 25, 30, true, "ki-outline ki-chart-line-up", "bg-light-warning", "text-warning", "bg-warning"),
            new("Phòng Marketing", "MKT-001", "Khối Kinh Doanh (CBDO)", "Hoàng Anh Tuấn", "Marketing Head", null, 12, 15, true, "ki-outline ki-megaphone", "bg-light-info", "text-info", "bg-info"),
            new("Ban Pháp Chế", "LEG-001", "Ban Giám Đốc", "Phạm Như Quỳnh", "Legal Counsel", null, 3, 4, true, "ki-outline ki-shield-tick", "bg-light-danger", "text-danger", "bg-danger")
        };

        private IEnumerable<DepartmentItem> FilteredDepartments => _departments
            .Where(d => string.IsNullOrEmpty(_searchTerm) || d.Name.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) || d.Code.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase))
            .Where(d => _statusFilter == "All" || (d.IsActive && _statusFilter == "Active") || (!d.IsActive && _statusFilter == "Inactive"));

        private void resetFilters() { _searchTerm = ""; _statusFilter = "All"; }

        private void openAddDepartment() {
            _isNewDepartment = true;
            _editingDept = new DepartmentItem { IsActive = true, Icon = "ki-outline ki-briefcase", IconBg = "bg-light-primary", IconColor = "text-primary", ProgressColor = "bg-primary" };
            _showEditDrawer = true;
        }

        private void editDepartment(DepartmentItem dept) {
            _isNewDepartment = false;
            _editingDept = dept; // In real app, we should clone
            _showEditDrawer = true;
        }

        private void viewDetails(DepartmentItem dept) {
            _selectedDept = dept;
            _showDetailDrawer = true;
        }

        private void closeEditDrawer() => _showEditDrawer = false;
        private void closeDetailDrawer() => _showDetailDrawer = false;

        private void saveDepartment() {
            if (_isNewDepartment) {
                _departments.Insert(0, _editingDept);
            }
            closeEditDrawer();
        }

        private void requestDeleteDepartment(DepartmentItem dept) {
            _departmentToDelete = dept;
            _showConfirmDeleteModal = true;
        }

        private void closeDeleteModal() {
            _showConfirmDeleteModal = false;
            _departmentToDelete = null;
        }

        private void confirmDeleteDepartment() {
            if (_departmentToDelete != null) {
                _departments.Remove(_departmentToDelete);
                closeDeleteModal();
            }
        }

        private class DepartmentItem
        {
            public string Name { get; set; } = "";
            public string Code { get; set; } = "";
            public string Block { get; set; } = "";
            public string? ManagerName { get; set; }
            public string? ManagerTitle { get; set; }
            public string? ManagerAvatar { get; set; }
            public int EmployeeCount { get; set; }
            public int Capacity { get; set; } = 10;
            public bool IsActive { get; set; } = true;
            public string Icon { get; set; } = "ki-outline ki-briefcase";
            public string IconBg { get; set; } = "bg-light-primary";
            public string IconColor { get; set; } = "text-primary";
            public string ProgressColor { get; set; } = "bg-primary";

            public DepartmentItem() { }
            public DepartmentItem(string name, string code, string block, string? manager, string? title, string? avatar, int count, int capacity, bool active, string icon, string bg, string color, string prog) {
                Name = name; Code = code; Block = block; ManagerName = manager; ManagerTitle = title; ManagerAvatar = avatar; 
                EmployeeCount = count; Capacity = capacity; IsActive = active; Icon = icon; IconBg = bg; IconColor = color; ProgressColor = prog;
            }
        }
    }
}
