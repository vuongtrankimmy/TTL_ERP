using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace TTL.HR.Shared.Components.Training
{
    public partial class TrainingRegistrationPopup
    {
        [Inject] public TTL.HR.Application.Modules.HumanResource.Interfaces.IEmployeeService EmployeeService { get; set; } = default!;
        [Parameter] public bool IsVisible { get; set; }
        [Parameter] public string CourseTitle { get; set; } = "";
        [Parameter] public List<string> AlreadyEnrolledEmployeeIds { get; set; } = new();
        [Parameter] public EventCallback<List<EmployeeSelectViewModel>> OnConfirm { get; set; }
        [Parameter] public EventCallback OnCancel { get; set; }

        private bool _isLoading = false;

        private string SearchTerm = "";
        private List<EmployeeSelectViewModel> AllEmployees = new();

        private bool _previousVisible = false;

        protected override void OnParametersSet()
        {
            if (IsVisible && !_previousVisible)
            {
                // Reset states when popup opens
                SearchTerm = "";
                var enrolledSet = AlreadyEnrolledEmployeeIds?.ToHashSet() ?? new HashSet<string>();
                foreach (var emp in AllEmployees)
                {
                    emp.IsAlreadyEnrolled = enrolledSet.Contains(emp.Id);
                    emp.IsSelected = emp.IsAlreadyEnrolled;
                }
            }
            _previousVisible = IsVisible;
        }

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            try
            {
                var employees = await EmployeeService.GetEmployeesAsync();
                if (employees != null)
                {
                    var enrolledSet = AlreadyEnrolledEmployeeIds?.ToHashSet() ?? new HashSet<string>();
                    AllEmployees = employees.Select(e => new EmployeeSelectViewModel
                    {
                        Id = e.Id,
                        FullName = e.FullName,
                        Code = e.Code,
                        Department = e.DepartmentName ?? "N/A",
                        Status = e.StatusName ?? "Active",
                        IsAlreadyEnrolled = enrolledSet.Contains(e.Id),
                        IsSelected = enrolledSet.Contains(e.Id)
                    }).ToList();
                }
            }
            catch (Exception)
            {
                // Silently fail for now, or could inject toastr
            }
            finally
            {
                _isLoading = false;
            }
        }

        private IEnumerable<EmployeeSelectViewModel> FilteredEmployees => AllEmployees
            .Where(e => string.IsNullOrWhiteSpace(SearchTerm) || 
                       e.FullName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) || 
                       e.Code.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));

        private int SelectedCount => AllEmployees.Count(e => e.IsSelected);
        private List<EmployeeSelectViewModel> SelectedEmployees => AllEmployees.Where(e => e.IsSelected).ToList();

        private bool IsAllSelected => FilteredEmployees.Any(e => !e.IsAlreadyEnrolled) && 
                                      FilteredEmployees.Where(e => !e.IsAlreadyEnrolled).All(e => e.IsSelected);

        private void ToggleSelectAll(ChangeEventArgs e)
        {
            bool isChecked = (bool)(e.Value ?? false);
            foreach (var emp in FilteredEmployees.Where(x => !x.IsAlreadyEnrolled))
            {
                emp.IsSelected = isChecked;
            }
        }

        private void ToggleEmployeeSelection(EmployeeSelectViewModel emp)
        {
            if (emp.IsAlreadyEnrolled) return;
            emp.IsSelected = !emp.IsSelected;
        }

        public class EmployeeSelectViewModel
        {
            public string Id { get; set; } = "";
            public string FullName { get; set; } = "";
            public string Code { get; set; } = "";
            public string Department { get; set; } = "";
            public string Status { get; set; } = "";
            public bool IsSelected { get; set; }
            public bool IsAlreadyEnrolled { get; set; }
        }
    }
}
