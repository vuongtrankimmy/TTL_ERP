using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.HumanResource.Models;

namespace TTL.HR.Shared.Components.HumanResource
{
    public partial class EmployeeSelectionDrawer
    {
        [Parameter] public string Title { get; set; } = "Chọn nhân sự";
        [Parameter] public bool IsVisible { get; set; }
        [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }

        [Parameter] public List<EmployeeDto> Employees { get; set; } = new();
        [Parameter] public List<string> InitialSelectedIds { get; set; } = new();
        [Parameter] public EventCallback<List<string>> OnSave { get; set; }
        [Parameter] public bool IsLoading { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public int ZIndex { get; set; } = 110;
        [Parameter] public bool SingleSelection { get; set; } = false;

        private List<string> SelectedEmployeeIds { get; set; } = new();
        private string _searchTerm = "";
        private bool _wasVisible = false;

        protected override void OnParametersSet()
        {
            if (IsVisible && !_wasVisible)
            {
                // Drawer just opened
                SelectedEmployeeIds = new List<string>(InitialSelectedIds);
                _searchTerm = "";
            }
            _wasVisible = IsVisible;
        }

        private IEnumerable<EmployeeDto> FilteredEmployees => string.IsNullOrWhiteSpace(_searchTerm)
            ? Employees
            : Employees.Where(e => (e.FullName?.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) || (e.Code?.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) ?? false));

        private int _currentPage = 1;
        private int _pageSize = 15;

        private int TotalPages => (int)Math.Ceiling(FilteredEmployees.Count() / (double)_pageSize);

        private IEnumerable<EmployeeDto> PaginatedEmployees => FilteredEmployees
            .Skip((_currentPage - 1) * _pageSize)
            .Take(_pageSize);

        private void OnSearchChanged(ChangeEventArgs e)
        {
            _searchTerm = e.Value?.ToString() ?? "";
            _currentPage = 1; // Reset to first page when searching
        }

        private void ChangePage(int newPage)
        {
            if (newPage >= 1 && newPage <= Math.Max(1, TotalPages))
            {
                _currentPage = newPage;
            }
        }

        private async Task CloseDrawer()
        {
            IsVisible = false;
            await IsVisibleChanged.InvokeAsync(IsVisible);
        }

        private void ToggleSelection(string empId, object checkedValue)
        {
            if (checkedValue is bool isChecked)
            {
                if (isChecked && !SelectedEmployeeIds.Contains(empId))
                {
                    SelectedEmployeeIds.Add(empId);
                }
                else if (!isChecked)
                {
                    SelectedEmployeeIds.Remove(empId);
                }
            }
        }

        private void SelectSingle(string empId)
        {
            if (SingleSelection)
            {
                SelectedEmployeeIds.Clear();
                SelectedEmployeeIds.Add(empId);
            }
        }

        private async Task SaveSelection()
        {
            await OnSave.InvokeAsync(SelectedEmployeeIds);
        }

        private string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "NV";
            var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0].Length >= 2 ? parts[0].Substring(0, 2).ToUpper() : parts[0].ToUpper();
            return (parts[0].Substring(0, 1) + parts[^1].Substring(0, 1)).ToUpper();
        }

        private readonly string[] _colors = { "primary", "success", "info", "warning", "danger", "dark" };
        private string GetRandomColor(string text)
        {
            if (string.IsNullOrEmpty(text)) return "bg-light-primary text-primary";
            var hash = 0;
            foreach (var c in text) hash += c;
            var index = hash % _colors.Length;
            return $"bg-light-{_colors[index]} text-{_colors[index]}";
        }
    }
}
