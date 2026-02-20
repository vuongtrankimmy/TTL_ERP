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
    public partial class PayrollDetail
    {
        [Parameter] public string? Id { get; set; }
        [Parameter] public int? Year { get; set; }
        [Parameter] public int? Month { get; set; }

        [Inject] public NavigationManager Navigation { get; set; } = default!;
        [Inject] public IPayrollService PayrollService { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;

        private string SearchTerm = "";
        private List<PayslipViewModel> Payslips = new();
        private bool IsAllSelected = false;
        private bool IsDrawerOpen = false;
        private PayslipViewModel? SelectedSlip;
        private bool _isLoading = true;
        private string PeriodName = "";

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            _isLoading = true;
            try
            {
                IEnumerable<PayrollModel>? payrolls = null;
                string? effectiveId = Id;

                // 1. If we have Month/Year instead of Id, try to find the Period Id
                if (string.IsNullOrEmpty(effectiveId) && Month.HasValue && Year.HasValue)
                {
                    var periods = await PayrollService.GetPeriodsAsync(Year.Value);
                    var period = periods?.FirstOrDefault(p => p.Month == Month.Value);
                    if (period != null)
                    {
                        effectiveId = period.Id;
                    }
                    else
                    {
                        PeriodName = $"Không tìm thấy kỳ lương tháng {Month}/{Year}";
                    }
                }

                // 2. Fetch data using ID
                if (!string.IsNullOrEmpty(effectiveId))
                {
                    var detail = await PayrollService.GetPeriodDetailAsync(effectiveId, SearchTerm);
                    if (detail != null)
                    {
                        PeriodName = detail.Period.Name;
                        payrolls = detail.Payrolls.Items;
                    }
                }

                // 3. Map to ViewModel
                if (payrolls != null)
                {
                    Payslips = payrolls.Select(p => new PayslipViewModel
                    {
                        Id = p.Id,
                        EmployeeCode = p.EmployeeCode,
                        EmployeeName = p.EmployeeName,
                        Department = p.DepartmentName,
                        AvatarUrl = string.IsNullOrEmpty(p.EmployeeAvatar) 
                            ? $"/assets/media/avatars/300-{new Random().Next(1, 30)}.jpg" 
                            : p.EmployeeAvatar,
                        BasicSalary = p.BasicSalary,
                        TotalAllowances = p.Allowance,
                        TotalDeductions = p.Deduction,
                        NetSalary = p.NetSalary,
                        ActualWorkDays = (int)p.ActualWorkDays,
                        StandardWorkDays = (int)p.TotalRequiredDays,
                        Status = (p.Status == "Closed" || p.Status == "Đã chốt") ? "Đã chốt" : "Dự thảo",
                        OriginalModel = p
                    }).ToList();
                }
                else
                {
                    Payslips = new List<PayslipViewModel>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading payroll detail: {ex.Message}");
                await JS.InvokeVoidAsync("toastr.error", "Lỗi tải chi tiết bảng lương.");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void OpenDetailDrawer(PayslipViewModel slip)
        {
            SelectedSlip = slip;
            IsDrawerOpen = true;
        }

        private void CloseDrawer()
        {
            IsDrawerOpen = false;
        }

        private void ToggleSelectAll(ChangeEventArgs e)
        {
            IsAllSelected = (bool)(e.Value ?? false);
            foreach (var slip in FilteredPayslips) { slip.IsSelected = IsAllSelected; }
        }

        private async Task FinalizePayroll()
        {
            bool success = false;
            if (!string.IsNullOrEmpty(Id))
            {
                success = await PayrollService.LockPayrollAsync(Id);
            }
            else if (Month.HasValue && Year.HasValue)
            {
                success = await PayrollService.LockPayrollAsync(Month.Value, Year.Value);
            }

            if (success)
            {
                await JS.InvokeVoidAsync("toastr.success", "Đã chốt bảng lương thành công.");
                await LoadData();
            }
            else
            {
                await JS.InvokeVoidAsync("toastr.error", "Chốt bảng lương thất bại.");
            }
        }

        private async Task SaveAdjustments()
        {
            if (SelectedSlip == null) return;

            SelectedSlip.CalculateNet();
            var model = SelectedSlip.OriginalModel;
            model.BasicSalary = SelectedSlip.BasicSalary;
            model.Allowance = SelectedSlip.TotalAllowances;
            model.Deduction = SelectedSlip.TotalDeductions;
            model.NetSalary = SelectedSlip.NetSalary;

            var success = await PayrollService.UpdatePayrollAsync(SelectedSlip.Id, model);
            if (success)
            {
                await JS.InvokeVoidAsync("toastr.success", "Đã lưu thay đổi.");
                CloseDrawer();
            }
            else
            {
                await JS.InvokeVoidAsync("toastr.error", "Lưu thay đổi thất bại.");
            }
        }

        private IEnumerable<PayslipViewModel> FilteredPayslips =>
            string.IsNullOrWhiteSpace(SearchTerm) 
            ? Payslips 
            : Payslips.Where(p => p.EmployeeName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) || p.EmployeeCode.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));

        private string GetStatusColor(string status)
        {
            return (status == "Đã chốt" || status == "Closed") ? "success" : "warning";
        }

        public class PayslipViewModel
        {
            public string Id { get; set; } = "";
            public bool IsSelected { get; set; }
            public string EmployeeCode { get; set; } = string.Empty;
            public string EmployeeName { get; set; } = string.Empty;
            public string Department { get; set; } = string.Empty;
            public string AvatarUrl { get; set; } = string.Empty;
            public decimal BasicSalary { get; set; }
            public decimal TotalAllowances { get; set; }
            public decimal TotalDeductions { get; set; }
            public decimal NetSalary { get; set; }
            public int ActualWorkDays { get; set; }
            public int StandardWorkDays { get; set; }
            public string Status { get; set; } = string.Empty;
            public PayrollModel OriginalModel { get; set; } = new();

            public void CalculateNet()
            {
                NetSalary = BasicSalary + TotalAllowances - TotalDeductions;
            }
        }
    }
}
