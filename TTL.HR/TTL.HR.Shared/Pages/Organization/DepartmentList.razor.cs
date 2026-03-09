using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Organization.Interfaces;
using TTL.HR.Application.Modules.Organization.Models;
using TTL.HR.Application.Modules.HumanResource.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Models;

namespace TTL.HR.Shared.Pages.Organization
{
    public partial class DepartmentList
    {
        [Inject] private IDepartmentService DepartmentService { get; set; } = default!;
        [Inject] private IEmployeeService EmployeeService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        private bool _showEditDrawer = false;
        private bool _showDetailDrawer = false;
        private bool _showConfirmDeleteModal = false;
        private bool _isNewDepartment = true;
        private string _searchTerm = "";
        private string _statusFilter = "All";
        private bool _isLoading = true;
        private bool _loadError = false;
        private string _errorMessage = "";
        private int currentPage = 1;
        private int pageSize = 10;
        private DepartmentItem _editingDept = new();
        private DepartmentItem? _selectedDept;
        private DepartmentItem? _departmentToDelete;

        private List<string> _availableIcons = new() { 
            "ki-outline ki-code", "ki-outline ki-people", "ki-outline ki-chart-line-up", 
            "ki-outline ki-gear", "ki-outline ki-shield-tick", "ki-outline ki-briefcase",
            "ki-outline ki-abstract-26", "ki-outline ki-abstract-41"
        };

        private List<DepartmentItem> _departments = new();
        private List<EmployeeDto> _employees = new();

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            await Task.WhenAll(LoadDepartments(), LoadEmployees());
        }

        private async Task LoadEmployees()
        {
            try
            {
                var result = await EmployeeService.GetEmployeesPaginatedAsync(1, 10000);
                _employees = result?.Items ?? new List<EmployeeDto>();
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                Console.WriteLine($"Error loading employees: {ex.Message}");
            }
        }

        private async System.Threading.Tasks.Task LoadDepartments()
        {
            _isLoading = true;
            _loadError = false;
            try
            {
                var departments = await DepartmentService.GetDepartmentsAsync();
                if (departments == null)
                {
                    _loadError = true;
                    return;
                }
                
                _departments = departments.Select(d => new DepartmentItem
                {
                    Id = d.Id,
                    Name = d.Name,
                    Code = d.Code,
                    Block = d.Description ?? "",
                    ManagerId = d.ManagerId,
                    ManagerName = d.ManagerName,
                    ManagerTitle = d.ManagerTitle,
                    ManagerAvatar = d.ManagerAvatar,
                    EmployeeCount = d.EmployeeCount,
                    IsActive = d.IsActive,
                    Capacity = d.Capacity > 0 ? d.Capacity : 10,
                    SortOrder = d.SortOrder,
                    ParentId = d.ParentId,
                    Icon = "ki-outline ki-briefcase",
                    IconBg = "bg-light-primary",
                    IconColor = "text-primary",
                    ProgressColor = "bg-primary"
                }).ToList();
            }
            catch (Exception ex)
            {
                _loadError = true;
                _errorMessage = ex.Message;
                Console.WriteLine($"Error loading departments: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private IEnumerable<DepartmentItem> FilteredDepartments => _departments
            .Where(d => string.IsNullOrEmpty(_searchTerm) || 
                       (d.Name?.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) || 
                       (d.Code?.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) ?? false))
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
                ManagerId = dept.ManagerId,
                ManagerName = dept.ManagerName,
                ManagerTitle = dept.ManagerTitle,
                ManagerAvatar = dept.ManagerAvatar,
                EmployeeCount = dept.EmployeeCount,
                Capacity = dept.Capacity,
                SortOrder = dept.SortOrder,
                ParentId = dept.ParentId,
                IsActive = dept.IsActive,
                Icon = dept.Icon,
                IconBg = dept.IconBg,
                IconColor = dept.IconColor,
                ProgressColor = dept.ProgressColor
            };
            _showEditDrawer = true;
        }

        private async System.Threading.Tasks.Task viewDetails(DepartmentItem dept) {
            _selectedDept = dept;
            _showDetailDrawer = true;
            
            try 
            {
                var details = await DepartmentService.GetDepartmentDetailAsync(dept.Id);
                if (details != null)
                {
                    _selectedDept.ActiveMembers = details.ActiveMembers;
                    _selectedDept.PendingReviews = details.PendingReviews;
                    _selectedDept.Members = details.Members;
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading department details: {ex.Message}");
            }
        }

        private void closeEditDrawer() => _showEditDrawer = false;
        private void closeDetailDrawer() => _showDetailDrawer = false;

        private async System.Threading.Tasks.Task saveDepartment() {
            _isLoading = true;
            try 
            {
                bool success = false;
                if (_isNewDepartment) {
                    var createRequest = new CreateDepartmentRequest
                    {
                        Name = _editingDept.Name,
                        Code = _editingDept.Code,
                        Description = _editingDept.Block,
                        ManagerId = _editingDept.ManagerId,
                        Capacity = _editingDept.Capacity,
                        SortOrder = _editingDept.SortOrder,
                        IsActive = _editingDept.IsActive,
                        ParentId = _editingDept.ParentId 
                    };
                    var result = await DepartmentService.CreateDepartmentAsync(createRequest);
                    success = result != null;
                    if (success) await JS.InvokeVoidAsync("toastr.success", "Tạo phòng ban thành công");
                    else await JS.InvokeVoidAsync("toastr.error", "Lỗi: Không thể tạo phòng ban. Vui lòng kiểm tra lại mã hoặc quyền hạn.");
                }
                else
                {
                    var updateRequest = new UpdateDepartmentRequest
                    {
                        Id = _editingDept.Id,
                        Name = _editingDept.Name,
                        Code = _editingDept.Code,
                        Description = _editingDept.Block,
                        ManagerId = _editingDept.ManagerId,
                        Capacity = _editingDept.Capacity,
                        SortOrder = _editingDept.SortOrder,
                        IsActive = _editingDept.IsActive,
                        ParentId = _editingDept.ParentId
                    };
                    var result = await DepartmentService.UpdateDepartmentAsync(_editingDept.Id, updateRequest);
                    success = result != null;
                    if (success) await JS.InvokeVoidAsync("toastr.success", "Cập nhật phòng ban thành công");
                }

                if (success)
                {
                    await LoadDepartments();
                    closeEditDrawer();
                }
                else
                {
                    await JS.InvokeVoidAsync("Swal.fire", new
                    {
                        title = "Không thể lưu dữ liệu",
                        text = "Máy chủ từ chối yêu cầu. Vui lòng kiểm tra lại thông tin nhập vào (Mã phòng ban có thể đã tồn tại).",
                        icon = "error",
                        confirmButtonText = "Đã hiểu"
                    });
                }
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("Swal.fire", new
                {
                    title = "Lỗi hệ thống nghiêm trọng",
                    html = $@"<div class='text-start'>
                                <p><b>Thông điệp:</b> {ex.Message}</p>
                                <hr/>
                                <p><b>Chi tiết kỹ thuật (Stack Trace):</b></p>
                                <pre class='bg-light p-3 border rounded' style='font-size: 10px; max-height: 200px; overflow-y: auto;'>{ex.StackTrace}</pre>
                             </div>",
                    icon = "error",
                    width = "600px",
                    confirmButtonText = "Đóng"
                });
                Console.WriteLine($"Error saving department: {ex}");
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
                    await JS.InvokeVoidAsync("toastr.success", "Xóa phòng ban thành công");
                    await LoadDepartments();
                }
                catch (Exception ex)
                {
                    await JS.InvokeVoidAsync("Swal.fire", new
                    {
                        title = "Lỗi khi xóa",
                        html = $@"<div class='text-start'>
                                    <p><b>Thông điệp:</b> {ex.Message}</p>
                                    <pre class='bg-light p-2 border' style='font-size: 10px;'>{ex.StackTrace}</pre>
                                 </div>",
                        icon = "error",
                        confirmButtonText = "Đóng"
                    });
                    Console.WriteLine($"Error deleting department: {ex.Message}");
                }
                finally
                {
                    closeDeleteModal();
                }
            }
        }

        // --- Assign Employees Modal Logic ---
        private bool _showAssignEmployeesDrawer = false;
        private List<string> _initialSelectedEmployeeIds = new();
        private List<EmployeeDto> _assignableEmployees = new();

        private void openAssignEmployeesModal()
        {
            if (_selectedDept == null) return;
            // Get all employees that have not resigned
            _assignableEmployees = _employees
                .Where(e => e.StatusName != "Đã nghỉ việc")
                .ToList();
            
            _initialSelectedEmployeeIds = _assignableEmployees
                .Where(e => e.DepartmentId == _selectedDept.Id)
                .Select(e => e.Id)
                .ToList();
                
            _showAssignEmployeesDrawer = true;
        }

        private async System.Threading.Tasks.Task saveEmployeeAssignments(List<string> selectedEmployeeIds)
        {
            if (_selectedDept == null) return;

            if (!selectedEmployeeIds.Any())
            {
                await JS.InvokeVoidAsync("toastr.warning", "Vui lòng chọn ít nhất 1 nhân viên");
                return;
            }

            _isLoading = true;
            try
            {
                // the new EmployeeSelectionDrawer passes the updated list
                var success = await DepartmentService.AssignEmployeesAsync(_selectedDept.Id, selectedEmployeeIds);
                if (success)
                {
                    await JS.InvokeVoidAsync("toastr.success", "Gán nhân sự vào phòng ban thành công.");
                    // Refresh current department details
                    await viewDetails(_selectedDept);
                    // Refresh tables list entirely
                    await LoadDepartments(); 
                    await LoadEmployees(); // because their departments updated
                    _showAssignEmployeesDrawer = false;
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", "Có lỗi xảy ra khi gán nhân sự.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error assigning employees: {ex.Message}");
                await JS.InvokeVoidAsync("Swal.fire", new
                {
                    title = "Lỗi gán nhân sự",
                    html = $@"<div class='text-start'>
                                <p>{ex.Message}</p>
                                <pre class='bg-light p-2 border' style='font-size: 10px;'>{ex.StackTrace}</pre>
                             </div>",
                    icon = "error"
                });
            }
            finally
            {
                _isLoading = false;
            }
        }

        private class DepartmentItem
        {
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = "";
            public string Code { get; set; } = "";
            public string Block { get; set; } = "";
            public string? ManagerId { get; set; }
            public string? ManagerName { get; set; }
            public string? ManagerTitle { get; set; }
            public string? ManagerAvatar { get; set; }
            public int EmployeeCount { get; set; }
            public int Capacity { get; set; } = 10;
            public int SortOrder { get; set; }
            public string? ParentId { get; set; }
            public int ActiveMembers { get; set; }
            public int PendingReviews { get; set; }
            public List<DepartmentMemberModel> Members { get; set; } = new();
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
