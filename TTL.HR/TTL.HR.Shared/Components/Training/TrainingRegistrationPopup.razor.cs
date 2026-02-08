using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace TTL.HR.Shared.Components.Training
{
    public partial class TrainingRegistrationPopup
    {
        [Parameter] public bool IsVisible { get; set; }
        [Parameter] public string CourseTitle { get; set; } = "";
        [Parameter] public EventCallback<List<EmployeeSelectViewModel>> OnConfirm { get; set; }
        [Parameter] public EventCallback OnCancel { get; set; }

        private string SearchTerm = "";
        private List<EmployeeSelectViewModel> AllEmployees = new();

        protected override void OnInitialized()
        {
            // Mock data - In real app, this would be passed as a Parameter or fetched via Service
            AllEmployees = new List<EmployeeSelectViewModel>
            {
                new() { FullName = "Phạm Minh Hùng", Code = "GD-001", Department = "Ban Giám Đốc", Status = "Active" },
                new() { FullName = "Nguyễn Thị Mai", Code = "NS-001", Department = "Phòng Nhân Sự", Status = "Active" },
                new() { FullName = "Lê Văn Tuấn", Code = "IT-001", Department = "Phòng IT", Status = "Active" },
                new() { FullName = "Trần Thanh Bình", Code = "IT-002", Department = "Phòng IT", Status = "Active" },
                new() { FullName = "Hoàng Thu Phương", Code = "IT-003", Department = "Phòng IT", Status = "Probation" },
                new() { FullName = "Vũ Thị Lan", Code = "TCKT-001", Department = "Phòng Tài Chính", Status = "Active" },
                new() { FullName = "Ngô Văn Long", Code = "SALES-001", Department = "Phòng Kinh Doanh", Status = "Active" }
            };
        }

        private IEnumerable<EmployeeSelectViewModel> FilteredEmployees => AllEmployees
            .Where(e => string.IsNullOrWhiteSpace(SearchTerm) || 
                       e.FullName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) || 
                       e.Code.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));

        private int SelectedCount => AllEmployees.Count(e => e.IsSelected);
        private List<EmployeeSelectViewModel> SelectedEmployees => AllEmployees.Where(e => e.IsSelected).ToList();

        private bool IsAllSelected => FilteredEmployees.Any() && FilteredEmployees.All(e => e.IsSelected);

        private void ToggleSelectAll(ChangeEventArgs e)
        {
            bool isChecked = (bool)(e.Value ?? false);
            foreach (var emp in FilteredEmployees)
            {
                emp.IsSelected = isChecked;
            }
        }

        public class EmployeeSelectViewModel
        {
            public string FullName { get; set; } = "";
            public string Code { get; set; } = "";
            public string Department { get; set; } = "";
            public string Status { get; set; } = "";
            public bool IsSelected { get; set; }
        }
    }
}
