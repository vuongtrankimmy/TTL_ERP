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

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            _isLoading = true;
            try
            {
                var payrolls = await PayrollService.GetPayrollsAsync();
                if (payrolls != null)
                {
                    // Group by Month/Year to show periods
                    Periods = payrolls
                        .GroupBy(p => new { p.Month, p.Year })
                        .Select(g => new PayrollPeriodViewModel
                        {
                            Name = $"Bảng lương tháng {g.Key.Month}/{g.Key.Year}",
                            Month = g.Key.Month,
                            Year = g.Key.Year,
                            Status = g.First().Status, // simplified
                            TotalAmount = g.Sum(p => p.NetSalary),
                            EmployeeCount = g.Count()
                        })
                        .OrderByDescending(p => p.Year)
                        .ThenByDescending(p => p.Month)
                        .ToList();
                }
            }
            catch (Exception)
            {
                await JS.InvokeVoidAsync("toastr.error", "Lỗi tải danh sách bảng lương.");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task CreateNewPeriod() 
        { 
            var now = DateTime.Now;
            var month = now.Month;
            var year = now.Year;

            // Check if already exists
            if (Periods.Any(p => p.Month == month && p.Year == year))
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

        private void ViewDetail(int month, int year)
        {
            Navigation.NavigateTo($"/payroll/period/{year}/{month}");
        }

        public class PayrollPeriodViewModel
        {
            public string Name { get; set; } = string.Empty;
            public int Month { get; set; }
            public int Year { get; set; }
            public string Status { get; set; } = string.Empty;
            public decimal TotalAmount { get; set; }
            public int EmployeeCount { get; set; }
        }
    }
}
