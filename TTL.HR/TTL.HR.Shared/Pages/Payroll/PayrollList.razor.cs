using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Payroll.Interfaces;
using TTL.HR.Application.Modules.Payroll.Models;

namespace TTL.HR.Shared.Pages.Payroll
{
    public partial class PayrollList
    {
        [Inject] public NavigationManager Navigation { get; set; } = default!;
        [Inject] public IPayrollService PayrollService { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;

        private List<PayrollPeriodViewModel> Periods = new();
        private bool _isLoading = true;
        private int _selectedYear = DateTime.Now.Year;
        private int _selectedMonth = 0; // Default to all months
        private int _totalEmployeeCount = 0;
        private decimal _totalTax = 0;
        private decimal _totalInsurance = 0;
        private decimal _totalNet = 0;

        // Delete Modal State
        private bool _isDeleteModalVisible = false;
        private string _itemToDeleteId = string.Empty;
        private string _itemToDeleteName = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task OnYearChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int year))
            {
                _selectedYear = year;
                await LoadData();
            }
        }

        private async Task OnMonthChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int month))
            {
                _selectedMonth = month;
                await LoadData();
            }
        }

        private async Task LoadData()
        {
            _isLoading = true;
            try
            {
                int? filterYear = _selectedYear == 0 ? null : (int?)_selectedYear;
                int? filterMonth = _selectedMonth == 0 ? null : (int?)_selectedMonth;
                
                // Fetch Periods from DB
                var periodsFromApi = await PayrollService.GetPeriodsAsync(filterYear, filterMonth);
                
                var periodsList = new List<PayrollPeriodViewModel>();
                
                // Determine years to show. 
                // If a year is selected, show that year.
                // If "All" is selected, show all years that have at least one period record.
                var yearsToShow = new List<int>();
                if (_selectedYear != 0)
                {
                    yearsToShow.Add(_selectedYear);
                }
                else if (periodsFromApi != null && periodsFromApi.Any())
                {
                    yearsToShow = periodsFromApi.Select(p => p.Year).Distinct().OrderByDescending(y => y).ToList();
                }
                else
                {
                    // Fallback to current year if no data
                    yearsToShow.Add(DateTime.Now.Year);
                }

                foreach (var year in yearsToShow)
                {
                    // If a month is selected, only show that month
                    int startMonth = _selectedMonth != 0 ? _selectedMonth : 12;
                    int endMonth = _selectedMonth != 0 ? _selectedMonth : 1;

                    for (int month = startMonth; month >= endMonth; month--)
                    {
                        var official = periodsFromApi?.FirstOrDefault(p => p.Year == year && p.Month == month);
                        if (official != null)
                        {
                            periodsList.Add(new PayrollPeriodViewModel
                            {
                                Id = official.Id,
                                Name = official.Name,
                                Month = official.Month,
                                Year = official.Year,
                                Status = official.Status,
                                StatusName = official.StatusName,
                                StatusColor = official.StatusColor,
                                TotalAmount = official.TotalNetSalary,
                                TotalInsurance = official.TotalInsurance,
                                TotalTax = official.TotalTax,
                                EmployeeCount = official.EmployeeCount,
                                IsPlaceholder = false
                            });
                        }
                        else
                        {
                            // Placeholder for non-existent month
                            periodsList.Add(new PayrollPeriodViewModel
                            {
                                Id = "",
                                Name = $"Bảng lương tháng {month:D2}/{year} (Chưa khởi tạo)",
                                Month = month,
                                Year = year,
                                Status = "Chưa khởi tạo",
                                StatusName = "Chưa khởi tạo",
                                StatusColor = "secondary",
                                TotalAmount = 0,
                                TotalInsurance = 0,
                                TotalTax = 0,
                                EmployeeCount = 0,
                                IsPlaceholder = true
                            });
                        }
                    }
                }

                Periods = periodsList;
                _totalEmployeeCount = periodsList.Where(p => !p.IsPlaceholder).Sum(p => p.EmployeeCount);
                _totalTax = periodsList.Where(p => !p.IsPlaceholder).Sum(p => p.TotalTax);
                _totalInsurance = periodsList.Where(p => !p.IsPlaceholder).Sum(p => p.TotalInsurance);
                _totalNet = periodsList.Where(p => !p.IsPlaceholder).Sum(p => p.TotalAmount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading payroll periods: {ex.Message}");
                await JS.InvokeVoidAsync("toastr.error", "Lỗi tải danh sách bảng lương. Vui lòng thử lại sau.");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task CreateNewPeriod() 
        { 
            var now = DateTime.Now;
            await CreateNewPeriod(now.Month, now.Year);
        }

        private async Task CreateNewPeriod(int month, int year)
        {
            // Check if already exists in official list
            if (Periods.Any(p => p.Month == month && p.Year == year && !p.IsPlaceholder))
            {
                await JS.InvokeVoidAsync("toastr.info", $"Bảng lương tháng {month}/{year} đã tồn tại.");
                return;
            }

            var success = await PayrollService.GeneratePayrollAsync(month, year);
            if (success)
            {
                await JS.InvokeVoidAsync("toastr.success", $"Đã khởi tạo bảng lương tháng {month}/{year}.");
                await LoadData();
            }
            else
            {
                await JS.InvokeVoidAsync("toastr.error", "Khởi tạo bảng lương thất bại.");
            }
        }

        private void DeletePeriod(string id, string name)
        {
            if (string.IsNullOrEmpty(id)) return;
            
            _itemToDeleteId = id;
            _itemToDeleteName = name;
            _isDeleteModalVisible = true;
            StateHasChanged();
        }

        private async Task OnConfirmDelete()
        {
            if (string.IsNullOrEmpty(_itemToDeleteId)) return;

            var success = await PayrollService.DeletePeriodAsync(_itemToDeleteId);
            if (success)
            {
                await JS.InvokeVoidAsync("toastr.success", $"Đã xóa {_itemToDeleteName} thành công.");
                _isDeleteModalVisible = false;
                _itemToDeleteId = string.Empty;
                _itemToDeleteName = string.Empty;
                await LoadData();
            }
            else
            {
                await JS.InvokeVoidAsync("toastr.error", "Xóa kỳ lương thất bại. Vui lòng thử lại.");
            }
        }

        private void OnCancelDelete()
        {
            _isDeleteModalVisible = false;
            _itemToDeleteId = string.Empty;
            _itemToDeleteName = string.Empty;
        }

        private void ViewDetail(int month, int year)
        {
            Navigation.NavigateTo($"/payroll/period/{year}/{month}");
        }
        
        private void ViewDetail(string id, int month, int year)
        {
            if (!string.IsNullOrEmpty(id))
            {
                Navigation.NavigateTo($"/payroll/period/{id}");
            }
            else
            {
                Navigation.NavigateTo($"/payroll/period/{year}/{month}");
            }
        }

        public class PayrollPeriodViewModel
        {
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public int Month { get; set; }
            public int Year { get; set; }
            public string Status { get; set; } = string.Empty;
            public string? StatusName { get; set; }
            public string? StatusColor { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal TotalInsurance { get; set; }
            public decimal TotalTax { get; set; }
            public int EmployeeCount { get; set; }
            public bool IsPlaceholder { get; set; }
        }
        private string GetStatusColor(string? color, string? status)
        {
            if (!string.IsNullOrEmpty(color)) return color;
            return (status == "Đã chốt" || status == "Closed") ? "success" : "warning";
        }

        private string GetStatusBg(string? color, string? status)
        {
            var c = GetStatusColor(color, status);
            if (c == "secondary" || c == "warning" || string.IsNullOrEmpty(c)) return "gray-900";
            return c;
        }
    }
}
