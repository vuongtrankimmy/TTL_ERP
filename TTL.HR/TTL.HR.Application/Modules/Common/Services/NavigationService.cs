using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Application.Modules.Common.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IAuthService _authService;

        public NavigationService(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<bool> UserHasPermissionAsync(string? permission)
        {
            if (string.IsNullOrEmpty(permission)) return true;
            
            var user = await _authService.GetCurrentUserAsync();
            if (user == null) return false;

            // 1. SuperAdmin & IT Support bypass for technical settings
            if (user.Role == "SuperAdmin" || user.Role == "Super Administrator" || user.Role == "Admin") 
            {
                return true;
            }

            if (user.Role == "IT Support" || user.Role == "ITSupport")
            {
                // IT Support can access settings and logs but NOT payroll
                if (permission.Contains("Payroll") || permission.Contains("Salary")) return false;
                if (permission.Contains("Settings") || permission.Contains("Audit") || permission.Contains("Administration")) return true;
                return true; 
            }

            // 2. HR Manager Permissions
            if (user.Role == "HR Manager" || user.Role == "HRManager")
            {
                // Full access to HR, Payroll, Recruitment, etc.
                if (permission.Contains("Administration")) return false; // Block system level config
                return true;
            }

            // 3. Dept Manager Permissions
            if (user.Role == "Dept Manager" || user.Role == "DeptManager")
            {
                // Can view/approve for their department
                if (permission.Contains("Payroll") || permission.Contains("Settings") || permission.Contains("Contract")) return false;
                if (permission.Contains("LeaveRequests.Approve") || permission.Contains("ShiftRequests.View")) return true;
                if (permission.Contains("Employees.View")) return true;
                return true;
            }

            // 4. Employee (Self-Service)
            if (user.Role == "Employee" || user.Role == "User")
            {
                // Restricted to self-service items
                if (permission.Contains("Create") || permission.Contains("Edit") || permission.Contains("Delete") || permission.Contains("Approve"))
                {
                    // Allow creating requests but not profiles/configs
                    if (permission.Contains("LeaveRequests.Create") || permission.Contains("ShiftRequests.Create")) return true;
                    return false;
                }
                
                // Allow viewing specific items
                if (permission.Contains("Employees.View") || permission.Contains("Contracts.View")) return false; // Cannot view other employees
                if (permission.Contains("Payroll.ViewSlip")) return true;
                
                return permission.Contains("View");
            }

            return false;
        }

        public async Task<List<NavItem>> GetMenuItemsAsync()
        {
            var allItems = new List<NavItem>
            {
                new NavItem { Title = "Menu_Dashboards", Icon = "ki-outline ki-element-11", Href = "/dashboard" },
                
                new NavItem { Title = "Section_OrgAndPersonnel", IsSection = true },
                new NavItem { 
                    Title = "Menu_CompanyStructure", 
                    Icon = "ki-outline ki-address-book", 
                    SubItems = new List<NavItem>
                    {
                        new NavItem { Title = "Menu_OrgChart", Href = "/organization/structure" },
                        new NavItem { Title = "Menu_Departments", Href = "/organization/departments", Permission = "Permissions.Administration.Settings" },
                        new NavItem { Title = "Menu_Positions", Href = "/organization/positions", Permission = "Permissions.Administration.Settings" }
                    }
                },
                new NavItem { 
                    Title = "Menu_EmployeeManagement", 
                    Icon = "ki-outline ki-people", 
                    SubItems = new List<NavItem>
                    {
                        new NavItem { Title = "Menu_EmployeeList", Href = "/employees", Permission = "Permissions.Employees.View" },
                        new NavItem { Title = "Menu_Onboarding", Href = "/employees/add", Permission = "Permissions.Employees.Create" },
                        new NavItem { Title = "Menu_Contracts", Href = "/contracts", Permission = "Permissions.Contracts.View" }
                    }
                },
                
                new NavItem { Title = "Section_HROperations", IsSection = true },
                new NavItem { 
                    Title = "Menu_Attendance", 
                    Icon = "ki-outline ki-timer", 
                    SubItems = new List<NavItem>
                    {
                        new NavItem { Title = "Menu_MonthlyTimesheet", Href = "/attendance/timesheet", Permission = "Permissions.Attendance.View" },
                        new NavItem { Title = "Menu_AttendanceData", Href = "/attendance", Permission = "Permissions.Attendance.View" },
                        new NavItem { Title = "Menu_ImportData", Href = "/attendance/import", Permission = "Permissions.Attendance.Edit" }
                    }
                },
                new NavItem
                {
                    Title = "Menu_ShiftManagement",
                    Icon = "ki-outline ki-calendar-8",
                    SubItems = new List<NavItem>
                    {
                        new NavItem { Title = "Menu_ShiftCatalog", Href = "/attendance/shifts", Icon = "ki-outline ki-category", Permission = "Permissions.Administration.Settings" },
                        new NavItem { Title = "Menu_WorkSchedule", Href = "/attendance/schedule", Icon = "ki-outline ki-calendar-edit", Permission = "Permissions.Attendance.Edit" },
                        new NavItem { Title = "Menu_ShiftApproval", Href = "/attendance/approval", Icon = "ki-outline ki-check-square", Permission = "Permissions.ShiftRequests.View" }
                    }
                },
                new NavItem { 
                    Title = "Menu_LeaveManagement", 
                    Icon = "ki-outline ki-calendar-8", 
                    SubItems = new List<NavItem>
                    {
                        new NavItem { Title = "Menu_LeaveRequests", Href = "/leave/requests", Permission = "Permissions.LeaveRequests.View" },
                        new NavItem { Title = "Menu_BusinessTrip", Href = "/attendance/leave-trip", Permission = "Permissions.LeaveRequests.View" }
                    }
                },
                new NavItem { 
                    Title = "Menu_PayrollAndBenefits", 
                    Icon = "ki-outline ki-wallet", 
                    SubItems = new List<NavItem>
                    {
                        new NavItem { Title = "Menu_PayrollPeriods", Href = "/payroll/periods", Permission = "Permissions.Payroll.View" },
                        new NavItem { Title = "Menu_Benefits", Href = "/benefits", Permission = "Permissions.Benefits.View" }
                    }
                },
                
                new NavItem { Title = "Section_RecruitmentAndTraining", IsSection = true },
                new NavItem { Title = "Menu_Recruitment", Icon = "ki-outline ki-user-tick", Href = "/recruitment", Permission = "Permissions.Recruitment.View" },
                new NavItem { Title = "Menu_Training", Icon = "ki-outline ki-book-open", Href = "/training", Permission = "Permissions.Training.View" },
                
                new NavItem { Title = "Section_AssetsAndEquipment", IsSection = true },
                new NavItem { 
                    Title = "Menu_AssetManagement", 
                    Icon = "ki-outline ki-monitor-mobile", 
                    SubItems = new List<NavItem>
                    {
                        new NavItem { Title = "Menu_AssetInventory", Href = "/assets/inventory", Permission = "Permissions.Assets.View" },
                        new NavItem { Title = "Menu_AssetAllocation", Href = "/assets/allocation", Permission = "Permissions.Assets.View" }
                    }
                },
                
                new NavItem { Title = "Section_System", IsSection = true, Permission = "Permissions.Administration.Settings" },
                new NavItem { 
                    Title = "Menu_AdminAndConfig", 
                    Icon = "ki-outline ki-setting-2", 
                    Permission = "Permissions.Administration.Settings",
                    SubItems = new List<NavItem>
                    {
                        new NavItem { Title = "Menu_RolesAndGroups", Href = "/administration/roles" },
                        new NavItem { Title = "Menu_Permissions", Href = "/permissions" },
                        new NavItem { Title = "Menu_GeneralSettings", Href = "/settings/general" },
                        new NavItem { Title = "Menu_Banks", Href = "/settings/banks" },
                        new NavItem { Title = "Menu_PayrollConfig", Href = "/settings/payroll-config" }
                    }
                },
                
                new NavItem { Title = "Section_Support", IsSection = true },
                new NavItem { Title = "Menu_HRToolkit", Icon = "ki-outline ki-calculator", Href = "/tools/hr-toolkit" },
                new NavItem { Title = "Menu_UserGuide", Icon = "ki-outline ki-book-open", Href = "/system/guide" }
            };

            var filteredMenu = new List<NavItem>();

            foreach (var item in allItems)
            {
                if (await UserHasPermissionAsync(item.Permission))
                {
                    var newItem = new NavItem
                    {
                        Title = item.Title,
                        Icon = item.Icon,
                        Href = item.Href,
                        Permission = item.Permission,
                        IsSection = item.IsSection
                    };

                    foreach (var subItem in item.SubItems)
                    {
                        if (await UserHasPermissionAsync(subItem.Permission))
                        {
                            newItem.SubItems.Add(subItem);
                        }
                    }

                    // If it's a section or has subitems (or is a leaf item), add it
                    if (item.IsSection || newItem.HasSubItems || !string.IsNullOrEmpty(newItem.Href))
                    {
                        filteredMenu.Add(newItem);
                    }
                }
            }

            return filteredMenu;
        }

    }
}
