using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
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

        private bool _isLoading = true;
        private RoleModel? _selectedRole;
        private bool _showUserPicker = false;
        private bool _showAddRoleDrawer = false;
        private bool _showRoleDetailDrawer = false;
        private string _memberSearchQuery = "";
        private string _employeeSearchQuery = "";

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
        private List<SelectedPerm> _availablePerms = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            _isLoading = true;
            try
            {
                _roles = await PermissionService.GetRolesAsync();
                _allPermissions = await PermissionService.GetAllAvailablePermissionsAsync();
                _availablePerms = _allPermissions.Select(p => new SelectedPerm(p.Name, p.Code)).ToList();
            }
            catch { }
            finally
            {
                _isLoading = false;
            }
        }

        private string GetPermissionName(string code)
        {
            return _allPermissions.FirstOrDefault(p => p.Code == code)?.Name ?? code;
        }

        private void openAddRole() => _showAddRoleDrawer = true;
        private void closeAddRole() {
            _showAddRoleDrawer = false;
            _newRoleName = "";
            _availablePerms.ForEach(x => x.IsSelected = false);
        }

        private async Task confirmCreateRole() {
            if (string.IsNullOrWhiteSpace(_newRoleName)) return;
            
            var selectedPermCodes = _availablePerms.Where(x => x.IsSelected).Select(x => x.Code).ToList();
            var newRole = new RoleModel { 
                Name = _newRoleName, 
                Description = "Tạo mới từ hệ thống",
                Permissions = selectedPermCodes 
            };

            var result = await PermissionService.CreateRoleAsync(newRole);
            if (result)
            {
                await LoadDataAsync();
                closeAddRole();
            }
        }

        private void openRoleDetail(RoleModel role) {
            _selectedRole = role;
            _showRoleDetailDrawer = true;
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
                    Position = e.Role, 
                    Avatar = e.Avatar ?? "assets/media/avatars/blank.png" 
                }).ToList();
            _showUserPicker = true;
        }

        private void closeAddUser() => _showUserPicker = false;

        private async Task confirmAddUsers() {
            if (_selectedRole == null) return;

            var selected = _pickerEmployees.Where(x => x.IsSelected).ToList();
            bool anySuccess = false;
            foreach(var s in selected) {
                var result = await PermissionService.AssignRoleAsync(_selectedRole.Id, s.Id);
                if (result) anySuccess = true;
            }

            if (anySuccess)
            {
                _selectedRole = await PermissionService.GetRoleByIdAsync(_selectedRole.Id);
                await LoadDataAsync();
            }
            closeAddUser();
        }

        private async Task removeUser(RoleMemberDto user) {
            if (_selectedRole == null) return;
            var result = await PermissionService.UnassignRoleAsync(_selectedRole.Id, user.Id);
            if (result)
            {
                _selectedRole = await PermissionService.GetRoleByIdAsync(_selectedRole.Id);
                await LoadDataAsync();
            }
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
            public bool IsSelected { get; set; }
            public SelectedPerm(string name, string code) { Name = name; Code = code; }
        }
    }
}
