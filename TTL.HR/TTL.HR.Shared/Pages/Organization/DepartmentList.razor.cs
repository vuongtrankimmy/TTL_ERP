using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using TTL.HR.Application.Modules.Organization.Interfaces;
using TTL.HR.Application.Modules.Organization.Models;

namespace TTL.HR.Shared.Pages.Organization
{
    public partial class DepartmentList
    {
        [Inject] private IDepartmentService DepartmentService { get; set; } = default!;

        private bool _showEditDrawer = false;
        private bool _showDetailDrawer = false;
        private bool _showConfirmDeleteModal = false;
        private bool _isNewDepartment = true;
        private string _searchTerm = "";
        private string _statusFilter = "All";
        private bool _isLoading = true;
        private DepartmentItem _editingDept = new();
        private DepartmentItem? _selectedDept;
        private DepartmentItem? _departmentToDelete;

        private List<string> _availableIcons = new() { 
            "ki-outline ki-code", "ki-outline ki-people", "ki-outline ki-chart-line-up", 
            "ki-outline ki-gear", "ki-outline ki-shield-tick", "ki-outline ki-briefcase",
            "ki-outline ki-abstract-26", "ki-outline ki-abstract-41"
        };

        private List<DepartmentItem> _departments = new();

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            await LoadDepartments();
        }

        private async System.Threading.Tasks.Task LoadDepartments()
        {
            _isLoading = true;
            try
            {
                var departments = await DepartmentService.GetDepartmentsAsync();
                _departments = departments.Select(d => new DepartmentItem
                {
                    Id = d.Id,
                    Name = d.Name,
                    Code = d.Code,
                    Block = d.Description ?? "", // Mapping Description to Block for now
                    ManagerName = d.ManagerName,
                    ManagerTitle = d.ManagerTitle,
                    ManagerAvatar = d.ManagerAvatar,
                    EmployeeCount = d.EmployeeCount,
                    IsActive = d.IsActive,
                    Capacity = 10, // Default or fetch if available
                    // Set default UI properties or map if stored
                    Icon = "ki-outline ki-briefcase",
                    IconBg = "bg-light-primary",
                    IconColor = "text-primary",
                    ProgressColor = "bg-primary"
                }).ToList();
            }
            catch (Exception ex)
            {
                // Handle error
                Console.WriteLine($"Error loading departments: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

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
            _editingDept = new DepartmentItem
            {
                Id = dept.Id,
                Name = dept.Name,
                Code = dept.Code,
                Block = dept.Block,
                ManagerName = dept.ManagerName,
                ManagerTitle = dept.ManagerTitle,
                ManagerAvatar = dept.ManagerAvatar,
                EmployeeCount = dept.EmployeeCount,
                Capacity = dept.Capacity,
                IsActive = dept.IsActive,
                Icon = dept.Icon,
                IconBg = dept.IconBg,
                IconColor = dept.IconColor,
                ProgressColor = dept.ProgressColor
            };
            _showEditDrawer = true;
        }

        private void viewDetails(DepartmentItem dept) {
            _selectedDept = dept;
            _showDetailDrawer = true;
        }

        private void closeEditDrawer() => _showEditDrawer = false;
        private void closeDetailDrawer() => _showDetailDrawer = false;

        private async System.Threading.Tasks.Task saveDepartment() {
            _isLoading = true;
            try 
            {
                if (_isNewDepartment) {
                    var createRequest = new CreateDepartmentRequest
                    {
                        Name = _editingDept.Name,
                        Code = _editingDept.Code,
                        Description = _editingDept.Block, // Mapping Block to Description
                        ParentId = null // Add UI for ParentId if needed
                    };
                    await DepartmentService.CreateDepartmentAsync(createRequest);
                }
                else
                {
                    var updateRequest = new UpdateDepartmentRequest
                    {
                        Name = _editingDept.Name,
                        Code = _editingDept.Code,
                        Description = _editingDept.Block,
                        IsActive = _editingDept.IsActive
                    };
                    await DepartmentService.UpdateDepartmentAsync(_editingDept.Id, updateRequest);
                }
                await LoadDepartments();
                closeEditDrawer();
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"Error saving department: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private void requestDeleteDepartment(DepartmentItem dept) {
            _departmentToDelete = dept;
            _showConfirmDeleteModal = true;
        }

        private void closeDeleteModal() {
            _showConfirmDeleteModal = false;
            _departmentToDelete = null;
        }

        private async System.Threading.Tasks.Task confirmDeleteDepartment() {
            if (_departmentToDelete != null) {
                try
                {
                    await DepartmentService.DeleteDepartmentAsync(_departmentToDelete.Id);
                    await LoadDepartments();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting department: {ex.Message}");
                }
                finally
                {
                    closeDeleteModal();
                }
            }
        }

        private class DepartmentItem
        {
            public string Id { get; set; } = string.Empty;
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
