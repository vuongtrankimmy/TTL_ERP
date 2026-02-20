using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.HumanResource.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Models;

namespace TTL.HR.Shared.Pages.Administration
{
    public partial class Roles
    {
        [Inject] private IPermissionService PermissionService { get; set; } = default!;
        [Inject] private IEmployeeService EmployeeService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        private bool _isLoading = true;
        private RoleModel? _selectedRole;
        private bool _showUserPicker = false;
        private bool _showAddRoleDrawer = false;
        private bool _showRoleDetailDrawer = false;
        private bool _isEditMode = false;
        private string _editingRoleId = "";
        private bool _isDeleteModalOpen = false;
        private RoleModel? _roleToDelete;
        [Parameter, SupplyParameterFromQuery(Name = "q")] public string _employeeSearchQuery { get; set; } = "";
        private string _memberSearchQuery = "";
        private string RoleSearchTerm = "";
        private System.Timers.Timer? _roleSearchTimer;

        private List<RoleModel> _roles = new();
        private List<PermissionDto> _allPermissions = new();
        private List<EmployeePickerItem> _pickerEmployees = new();

        private List<RoleMemberDto> FilteredMembers => _selectedRole?.Members
            .Where(x => string.IsNullOrWhiteSpace(_memberSearchQuery) || 
                       x.Name.Contains(_memberSearchQuery, StringComparison.OrdinalIgnoreCase))
            .ToList() ?? new();

        private List<EmployeePickerItem> FilteredAvailableEmployees => _pickerEmployees
            .Where(x => string.IsNullOrWhiteSpace(_employeeSearchQuery) || 
                       x.Name.Contains(_employeeSearchQuery, StringComparison.OrdinalIgnoreCase) || 
                       x.Code.Contains(_employeeSearchQuery, StringComparison.OrdinalIgnoreCase))
            .ToList();

        private string _newRoleName = "";
        private string _newRoleDescription = "";
        private List<SelectedPerm> _availablePerms = new();

        protected override async Task OnParametersSetAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            _isLoading = true;
            try
            {
                _roles = await PermissionService.GetRolesAsync(RoleSearchTerm);
                _allPermissions = await PermissionService.GetAllAvailablePermissionsAsync();
                _availablePerms = _allPermissions.Select(p => new SelectedPerm(p.Name, p.Code, p.Id, p.Group)).ToList();
            }
            catch { }
            finally
            {
                _isLoading = false;
            }
        }

        private void HandleRoleSearch()
        {
            _roleSearchTimer?.Stop();
            _roleSearchTimer = new System.Timers.Timer(500);
            _roleSearchTimer.Elapsed += async (s, e) =>
            {
                _roleSearchTimer.Stop();
                await InvokeAsync(LoadDataAsync);
            };
            _roleSearchTimer.Start();
        }

        private async Task ClearRoleSearch()
        {
            RoleSearchTerm = "";
            await LoadDataAsync();
        }

        private string GetPermissionName(string idOrCode)
        {
            return _allPermissions.FirstOrDefault(p => p.Id == idOrCode || p.Code == idOrCode)?.Name ?? idOrCode;
        }

        private void openAddRole() {
            _isEditMode = false;
            _showAddRoleDrawer = true;
        }

        private void openEditRole(RoleModel role) {
            _isEditMode = true;
            _editingRoleId = role.Id;
            _newRoleName = role.Name;
            _newRoleDescription = role.Description;
            
            // Map selected perms
            _availablePerms.ForEach(x => x.IsSelected = role.Permissions.Contains(x.Id) || role.Permissions.Contains(x.Code));
            
            _showAddRoleDrawer = true;
        }

        private void closeAddRole() {
            _showAddRoleDrawer = false;
            _newRoleName = "";
            _newRoleDescription = "";
            _editingRoleId = "";
            _availablePerms.ForEach(x => x.IsSelected = false);
        }

        private async Task confirmCreateRole() {
            if (string.IsNullOrWhiteSpace(_newRoleName)) return;
            
            var selectedPermIds = _availablePerms.Where(x => x.IsSelected).Select(x => x.Id).ToList();
            var roleData = new RoleModel { 
                Name = _newRoleName, 
                Description = string.IsNullOrWhiteSpace(_newRoleDescription) ? "Mô tả vai trò" : _newRoleDescription,
                Permissions = selectedPermIds 
            };

            (bool Success, string Message) result;
            if (_isEditMode) {
                roleData.Id = _editingRoleId;
                result = await PermissionService.UpdateRoleAsync(_editingRoleId, roleData);
            } else {
                result = await PermissionService.CreateRoleAsync(roleData);
            }

            if (result.Success)
            {
                await JS.InvokeVoidAsync("toastr.success", result.Message ?? (_isEditMode ? "Cập nhật vai trò thành công" : "Tạo vai trò thành công"));
                await LoadDataAsync();
                closeAddRole();
            }
            else
            {
                await JS.InvokeVoidAsync("toastr.error", result.Message ?? (_isEditMode ? "Cập nhật vai trò thất bại" : "Tạo vai trò thất bại"));
            }
            StateHasChanged();
        }

        private async Task openRoleDetail(RoleModel role) {
            _selectedRole = role;
            _showRoleDetailDrawer = true;
            // Fetch full detail (including members) from API
            var detail = await PermissionService.GetRoleByIdAsync(role.Id);
            if (detail != null)
            {
                _selectedRole = detail;
                StateHasChanged();
            }
        }

        private void closeRoleDetail() => _showRoleDetailDrawer = false;

        private async Task openAddUser() {
            var employees = await EmployeeService.GetEmployeesAsync();
            var memberIds = _selectedRole?.Members.Select(m => m.Id).ToHashSet() ?? new HashSet<string>();
            _pickerEmployees = employees
                .Where(e => !memberIds.Contains(e.Id))
                .Select(e => new EmployeePickerItem { 
                    Id = e.Id,
                    Name = e.FullName, 
                    Code = e.Code, 
                    Position = e.PositionName, 
                    Avatar = string.IsNullOrEmpty(e.AvatarUrl) ? "assets/media/avatars/blank.png" : e.AvatarUrl
                }).ToList();
            _showUserPicker = true;
        }

        private void closeAddUser() => _showUserPicker = false;

        private async Task confirmAddUsers() {
            if (_selectedRole == null) return;

            var selectedIds = _pickerEmployees.Where(x => x.IsSelected).Select(x => x.Id).ToList();
            if (selectedIds.Any()) {
                var result = await PermissionService.AssignRolesAsync(_selectedRole.Id, selectedIds);
                if (result)
                {
                    await JS.InvokeVoidAsync("toastr.success", $"Đã thêm {selectedIds.Count} nhân viên vào vai trò");
                    _selectedRole = await PermissionService.GetRoleByIdAsync(_selectedRole.Id);
                    await LoadDataAsync();
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", "Thêm nhân viên thất bại");
                }
            }
            closeAddUser();
            StateHasChanged();
        }

        private async Task removeUser(RoleMemberDto user) {
            if (_selectedRole == null) return;
            var result = await PermissionService.UnassignRoleAsync(_selectedRole.Id, user.Id);
            if (result)
            {
                await JS.InvokeVoidAsync("toastr.success", "Đã xóa nhân viên khỏi vai trò");
                _selectedRole = await PermissionService.GetRoleByIdAsync(_selectedRole.Id);
                await LoadDataAsync();
            }
            else
            {
                await JS.InvokeVoidAsync("toastr.error", "Xóa nhân viên thất bại");
            }
            StateHasChanged();
        }

        private void promptDeleteRole(RoleModel role) {
            _roleToDelete = role;
            _isDeleteModalOpen = true;
        }

        private void closeDeleteModal() {
            _isDeleteModalOpen = false;
            _roleToDelete = null;
        }

        private async Task confirmDeleteRole() {
            if (_roleToDelete == null) return;
            var result = await PermissionService.DeleteRoleAsync(_roleToDelete.Id);
            if (result) {
                await JS.InvokeVoidAsync("toastr.success", "Đã xóa vai trò thành công");
                await LoadDataAsync();
                closeDeleteModal();
            }
            else
            {
                await JS.InvokeVoidAsync("toastr.error", "Xóa vai trò thất bại");
            }
            StateHasChanged();
        }

        public class EmployeePickerItem {
            public string Id { get; set; } = "";
            public string Name { get; set; } = "";
            public string Code { get; set; } = "";
            public string Position { get; set; } = "";
            public string Avatar { get; set; } = "";
            public bool IsSelected { get; set; } = false;
        }

        public class SelectedPerm {
            public string Name { get; set; } = string.Empty;
            public string Code { get; set; } = string.Empty;
            public string Id { get; set; } = string.Empty;
            public string Group { get; set; } = string.Empty;
            public bool IsSelected { get; set; }
            public SelectedPerm(string name, string code, string id, string group) { 
                Name = name; 
                Code = code; 
                Id = id; 
                Group = string.IsNullOrWhiteSpace(group) ? "Hệ thống" : group;
            }
        }
    }
}
