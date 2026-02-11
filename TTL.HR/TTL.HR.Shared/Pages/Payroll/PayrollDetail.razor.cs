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
        [Parameter] public int Year { get; set; }
        [Parameter] public int Month { get; set; }

        [Inject] public NavigationManager Navigation { get; set; } = default!;
        [Inject] public IPayrollService PayrollService { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;

        private string SearchTerm = "";
        private List<PayslipViewModel> Payslips = new();
        private bool IsAllSelected = false;
        private bool IsDrawerOpen = false;
        private PayslipViewModel? SelectedSlip;
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
                var payrolls = await PayrollService.GetPayrollsAsync(Month, Year);
                if (payrolls != null)
                {
                    Payslips = payrolls.Select(p => new PayslipViewModel
                    {
                        Id = p.Id,
                        EmployeeCode = p.EmployeeId,
                        EmployeeName = p.EmployeeName,
                        Department = "Kỹ thuật", // Mock department for now
                        AvatarUrl = $"/assets/media/avatars/300-{new Random().Next(1, 10)}.jpg",
                        BasicSalary = p.BasicSalary,
                        TotalAllowances = p.Bonus,
                        TotalDeductions = p.Deductions,
                        NetSalary = p.NetSalary,
                        ActualWorkDays = 22,
                        StandardWorkDays = 22,
                        Status = (p.Status == "Closed" || p.Status == "Đã chốt") ? "Đã chốt" : "Dự thảo",
                        OriginalModel = p
                    }).ToList();
                }
            }
            catch (Exception)
            {
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
            var success = await PayrollService.LockPayrollAsync(Month, Year);
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
            model.Bonus = SelectedSlip.TotalAllowances;
            model.Deductions = SelectedSlip.TotalDeductions;
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
