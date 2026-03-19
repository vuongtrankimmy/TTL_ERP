using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Shared.Pages.Permissions
#pragma warning disable CS0169, CS0414
{
    public partial class PermissionsList
    {
        [Inject] private IPermissionService PermissionService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        private bool _isLoading = true;
        private bool _showRoleDrawer = false;
        private bool _showDetailDrawer = false;
        private PermissionCategory? _selectedCategory;
        private List<RoleModel> _roles = new();
        private List<PermissionCategory> _permissions = new();
        private int _allPermissionsCount = 0;
        private int _totalUsersWithRoles = 0;

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            _isLoading = true;
            try
            {
                var allRoles = await PermissionService.GetRolesAsync();
                _roles = allRoles.Take(5).ToList(); // Limit columns to 5 for layout stability
                
                var apiPermissions = await PermissionService.GetPermissionsAsync();
                _allPermissionsCount = apiPermissions.Count;
                _totalUsersWithRoles = allRoles.Sum(r => r.UsersCount);

                _permissions = apiPermissions
                    .GroupBy(p => p.Group)
                    .Select(g => new PermissionCategory(
                        g.Key,
                        $"{g.Count()} quyền hạn khả dụng",
                        GetGroupIcon(g.Key),
                        GetGroupBg(g.Key),
                        GetGroupColor(g.Key),
                        CalculateAccessDefaults(g.ToList()),
                        g.Select(p => new SubPermission(p.Name, p.Id, p.AssignedRoles.Select(ar => ar.Id).ToList())).ToList()
                    )).ToList();
            }
            catch (Exception)
            {
                // Handle error
            }
            finally
            {
                _isLoading = false;
            }
        }

        private bool[] CalculateAccessDefaults(List<PermissionWithRolesDto> groupPerms)
        {
            var results = new bool[_roles.Count];
            for (int i = 0; i < _roles.Count; i++)
            {
                var roleId = _roles[i].Id;
                // A category is "checked" for a role if it has ANY permission assigned to it
                results[i] = groupPerms.Any(p => p.AssignedRoles.Any(ar => ar.Id == roleId));
            }
            return results;
        }

        private string GetGroupIcon(string group) => group switch
        {
            "Nhân sự" => "ki-outline ki-profile-user",
            "Tính lương" => "ki-outline ki-wallet",
            "Chấm công" => "ki-outline ki-timer",
            "Nghỉ phép" => "ki-outline ki-calendar-tick",
            "Tuyển dụng" => "ki-outline ki-user-tick",
            "Đào tạo" => "ki-outline ki-teacher",
            "Hệ thống" => "ki-outline ki-setting-2",
            "Tổ chức" => "ki-outline ki-office-bag",
            "Tài sản" => "ki-outline ki-package",
            "Phúc lợi" => "ki-outline ki-heart",
            _ => "ki-outline ki-shield"
        };

        private string GetGroupBg(string group) => group switch
        {
            "Nhân sự" => "bg-light-primary",
            "Tính lương" => "bg-light-success",
            "Chấm công" => "bg-light-warning",
            "Nghỉ phép" => "bg-light-info",
            "Tuyển dụng" => "bg-light-danger",
            "Đào tạo" => "bg-light-primary",
            "Hệ thống" => "bg-light-dark",
            "Tổ chức" => "bg-light-warning",
            "Tài sản" => "bg-light-success",
            "Phúc lợi" => "bg-light-danger",
            _ => "bg-light-primary"
        };

        private string GetGroupColor(string group) => group switch
        {
            "Nhân sự" => "text-primary",
            "Tính lương" => "text-success",
            "Chấm công" => "text-warning",
            "Nghỉ phép" => "text-info",
            "Tuyển dụng" => "text-danger",
            "Đào tạo" => "text-primary",
            "Hệ thống" => "text-dark",
            "Tổ chức" => "text-warning",
            "Tài sản" => "text-success",
            "Phúc lợi" => "text-danger",
            _ => "text-primary"
        };

        private void openAddRole() => _showRoleDrawer = true;
        private void closeRoleDrawer() => _showRoleDrawer = false;

        private void openPermissionDetail(PermissionCategory cat) {
            _selectedCategory = cat;
            _showDetailDrawer = true;
        }
        private void closeDetailDrawer() => _showDetailDrawer = false;

        private void ToggleCategory(PermissionCategory cat, int roleIndex)
        {
            var role = _roles[roleIndex];
            var isCurrentlyEnabled = cat.AccessDefaults[roleIndex];
            
            foreach (var sub in cat.SubPermissions)
            {
                if (isCurrentlyEnabled)
                    role.Permissions.Remove(sub.Key);
                else if (!role.Permissions.Contains(sub.Key))
                    role.Permissions.Add(sub.Key);
                
                // Update the SubPermission's internal state too
                if (isCurrentlyEnabled)
                    sub.AssignedRoleIds.Remove(role.Id);
                else if (!sub.AssignedRoleIds.Contains(role.Id))
                    sub.AssignedRoleIds.Add(role.Id);
            }
            
            cat.AccessDefaults[roleIndex] = !isCurrentlyEnabled;
            StateHasChanged();
        }

        private void ToggleSinglePermission(SubPermission sub, int roleIndex)
        {
            var role = _roles[roleIndex];
            if (sub.AssignedRoleIds.Contains(role.Id))
            {
                sub.AssignedRoleIds.Remove(role.Id);
                role.Permissions.Remove(sub.Key);
            }
            else
            {
                sub.AssignedRoleIds.Add(role.Id);
                role.Permissions.Add(sub.Key);
            }

            // Update parent category's AccessDefaults for the matrix view
            // Re-calculate if ANY sub-permission in this category is enabled for this role
            if (_selectedCategory != null)
            {
                _selectedCategory.AccessDefaults[roleIndex] = _selectedCategory.SubPermissions.Any(p => p.AssignedRoleIds.Contains(role.Id));
            }
            StateHasChanged();
        }

        private async Task SaveAllChanges()
        {
            _isLoading = true;
            try
            {
                bool allSuccess = true;
                foreach (var role in _roles)
                {
                    var result = await PermissionService.UpdateRoleAsync(role.Id, role);
                    if (!result.Success) allSuccess = false;
                }
                
                if (allSuccess)
                    await JS.InvokeVoidAsync("toastr.success", "Đã lưu tất cả thay đổi phân quyền");
                else
                    await JS.InvokeVoidAsync("toastr.warning", "Một số thay đổi có thể chưa được lưu. Vui lòng kiểm tra lại.");
            }
            catch (Exception)
            {
                await JS.InvokeVoidAsync("toastr.error", "Lưu thay đổi thất bại. Vui lòng thử lại.");
            }
            finally
            {
                await LoadDataAsync();
                StateHasChanged();
            }
        }

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
            public List<string> AssignedRoleIds { get; set; }
            public bool DefaultValue { get; set; } = false;
            public bool IsEnabled(string roleId) => AssignedRoleIds.Contains(roleId);

            public SubPermission(string action, string key, List<string> roleIds) { 
                Action = action; Key = key; AssignedRoleIds = roleIds;
            }
        }
    }
}
