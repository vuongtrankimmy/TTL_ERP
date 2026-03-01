using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Payroll.Interfaces;
using TTL.HR.Application.Modules.Payroll.Models;

using TTL.HR.Application.Modules.Organization.Models;
using TTL.HR.Application.Modules.Common.Interfaces;

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
        [Inject] public IPdfService PdfService { get; set; } = default!;

        private string SearchTerm = "";
        private List<PayslipViewModel> Payslips = new();
        private bool IsAllSelected = false;
        private bool IsDrawerOpen = false;
        private PayslipViewModel? SelectedSlip;
        private bool _isLoading = true;
        private List<DepartmentModel> _departments = new();
        private string _selectedDepartmentId = "";
        private int _page = 1;
        private int _pageSize = 20;
        private long _totalCount = 0;
        private string PeriodName = "";
        private PayrollPeriodModel? SelectedPeriod;

        [Inject] private IMasterDataService MasterDataService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            try {
                var depts = await MasterDataService.GetCachedLookupsAsync("Department");
                if (depts != null)
                {
                    _departments = depts.Select(d => new DepartmentModel { Id = d.Id, Name = d.Name }).ToList();
                }
            } catch (Exception ex) {
                Console.WriteLine($"Error loading departments: {ex.Message}");
            }
            await LoadData();
        }

        private async Task LoadData()
        {
            _isLoading = true;
            try
            {
                IEnumerable<PayrollModel>? payrolls = null;
                string? effectiveId = Id;

                // Fetch data using ID or Year/Month
                var detail = await PayrollService.GetPeriodDetailAsync(effectiveId, SearchTerm, _selectedDepartmentId, _page, _pageSize, Year, Month);
                if (detail != null)
                {
                    SelectedPeriod = detail.Period;
                    PeriodName = detail.Period.Name;
                    Month = detail.Period.Month;
                    Year = detail.Period.Year;
                    
                    if (detail.Payrolls != null)
                    {
                        payrolls = detail.Payrolls.Items ?? new List<PayrollModel>();
                        _totalCount = detail.Payrolls.TotalCount;
                        Console.WriteLine($"[PayrollDetail] DATA LOADED: Period.EmployeeCount={detail.Period.EmployeeCount}, Payrolls.TotalCount={_totalCount}, Items={payrolls.Count()}");
                    }
                    else
                    {
                        Console.WriteLine("[PayrollDetail] Payrolls object is NULL in detail");
                        payrolls = new List<PayrollModel>();
                        _totalCount = 0;
                    }
                }
                else 
                {
                    await InvokeToastr("error", "Không thể kết nối với máy chủ hoặc dữ liệu kỳ lương không hợp lệ.");
                    _isLoading = false;
                    return;
                }

                var mappedSlips = new List<PayslipViewModel>();
                if (payrolls != null && payrolls.Any())
                {
                    Console.WriteLine($"[PayrollDetail] Received {payrolls.Count()} payroll items. Starting mapping...");
                    
                    var itemList = payrolls.ToList();
                    for (int i = 0; i < itemList.Count; i++)
                    {
                        var p = itemList[i];
                        try 
                        {
                            mappedSlips.Add(new PayslipViewModel
                            {
                                Id = p.Id ?? "",
                                EmployeeId = p.EmployeeId,
                                EmployeeCode = p.EmployeeCode ?? "N/A",
                                EmployeeName = p.EmployeeName ?? "N/A",
                                Department = p.DepartmentName ?? "N/A",
                                AvatarUrl = string.IsNullOrEmpty(p.EmployeeAvatar) 
                                    ? $"/assets/media/avatars/300-{(i % 20) + 1}.jpg" 
                                    : p.EmployeeAvatar,
                                
                                BasicSalary = p.BasicSalary,
                                InsuranceSalary = p.InsuranceSalary,
                                ActualWorkDays = p.ActualWorkDays,
                                LeaveDays = p.LeaveDays,
                                HolidayDays = p.HolidayDays,
                                StandardWorkDays = (p.TotalRequiredDays > 0) ? p.TotalRequiredDays : 26,
                                OvertimeHours = p.OvertimeHours,
                                OvertimeHoursWeekend = p.OvertimeHoursWeekend,
                                OvertimeHoursHoliday = p.OvertimeHoursHoliday,
                                OvertimeSalaryWeekend = p.OvertimeSalaryWeekend,
                                OvertimeSalaryHoliday = p.OvertimeSalaryHoliday,
                                
                                TotalWorkSalary = p.TotalWorkSalary,
                                OvertimeSalary = p.OvertimeSalary,
                                TotalAllowances = p.Allowance,
                                Bonus = p.Bonus,
                                
                                BhxhAmount = p.BhxhAmount,
                                BhytAmount = p.BhytAmount,
                                BhtnAmount = p.BhtnAmount,
                                UnionFee = p.UnionFee,
                                
                                TaxableIncome = p.TaxableIncome,
                                PersonalDeduction = p.PersonalDeduction,
                                DependentDeduction = p.DependentDeduction,
                                TaxAmount = p.TaxAmount,
                                
                                OtRateWeekday = SelectedPeriod?.OtRateWeekday ?? 1.5,
                                OtRateWeekend = SelectedPeriod?.OtRateWeekend ?? 2.0,
                                OtRateHoliday = SelectedPeriod?.OtRateHoliday ?? 3.0,
                                
                                AdvanceAmount = p.AdvanceAmount,
                                TotalDeductions = p.Deduction,
                                
                                NetSalary = p.NetSalary,
                                Status = (p.Status == "Closed" || p.Status == "Đã chốt") ? "Đã chốt" : "Dự thảo",
                                WarningMessage = p.WarningMessage,
                                OriginalModel = p
                            });
                        }
                        catch (Exception ex) 
                        { 
                            Console.WriteLine($"[PayrollDetail] Error mapping item at index {i} ({p?.EmployeeName}): {ex.Message}");
                        }
                    }
                    
                    if (!mappedSlips.Any() && payrolls.Any()) 
                    {
                        await InvokeToastr("error", "Dữ liệu nhân viên không thể hiển thị do lỗi định dạng dữ liệu.");
                    }
                    else if (mappedSlips.Count < payrolls.Count())
                    {
                         await InvokeToastr("warning", $"Chỉ hiển thị được {mappedSlips.Count}/{payrolls.Count()} nhân viên do một số dòng dữ liệu bị lỗi.");
                    }
                }
                else 
                {
                    Console.WriteLine($"[PayrollDetail] No payroll items received. TotalCount reported as {_totalCount}");
                    if (_totalCount > 0)
                    {
                        await InvokeToastr("error", $"Hệ thống báo có {_totalCount} nhân sự nhưng chi tiết danh sách trống (Lỗi API hoặc Deserialization).");
                    }
                    else
                    {
                        await InvokeToastr("warning", "Hiện tại không có dữ liệu nhân viên nào cho các tiêu chí bộ lọc này.");
                    }
                }
                
                Payslips = mappedSlips;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical error mapping payrolls: {ex.Message}");
                await InvokeToastr("error", $"Lỗi xử lý: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private async Task ReCalculatePayroll()
        {
            if (!Month.HasValue || !Year.HasValue) 
            {
                await InvokeToastr("info", "Vui lòng đợi thông tin kỳ lương tải xong.");
                return;
            }

            _isLoading = true;
            await InvokeToastr("info", "Đang xử lý tính toán lương trong nền... Vui lòng đợi trong giây lát.");
            
            var success = await PayrollService.GeneratePayrollAsync(Month.Value, Year.Value);
            
            if (success)
            {
                // Wait a bit for the background task to start/process
                await Task.Delay(2000);
                await LoadData();
                await InvokeToastr("success", "Đã gửi lệnh tính toán lương.");
            }
            else
            {
                await InvokeToastr("error", "Gửi lệnh tính toán lương thất bại.");
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

        private async Task OnSearchChange(string searchTerm)
        {
            SearchTerm = searchTerm;
            _page = 1;
            await LoadData();
        }

        private async Task OnDepartmentChange(string departmentId)
        {
            _selectedDepartmentId = departmentId;
            _page = 1;
            await LoadData();
        }

        private async Task OnPageChanged(int page)
        {
            _page = page;
            await LoadData();
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
                await InvokeToastr("success", "Đã chốt bảng lương thành công.");
                await LoadData();
            }
            else
            {
                await InvokeToastr("error", "Chốt bảng lương thất bại.");
            }
        }

        private async Task SaveAdjustments()
        {
            if (SelectedSlip == null) return;

            SelectedSlip.CalculateNet();
            var model = SelectedSlip.OriginalModel;
            model.BasicSalary = SelectedSlip.BasicSalary;
            model.TotalWorkSalary = SelectedSlip.TotalWorkSalary;
            model.OvertimeSalary = SelectedSlip.OvertimeSalary;
            model.OvertimeSalaryWeekend = SelectedSlip.OvertimeSalaryWeekend;
            model.OvertimeSalaryHoliday = SelectedSlip.OvertimeSalaryHoliday;
            model.Allowance = SelectedSlip.TotalAllowances;
            model.Deduction = SelectedSlip.TotalDeductions;
            model.NetSalary = SelectedSlip.NetSalary;

            var success = await PayrollService.UpdatePayrollAsync(SelectedSlip.Id, model);
            if (success)
            {
                await InvokeToastr("success", "Đã lưu thay đổi.");
                CloseDrawer();
            }
            else
            {
                await InvokeToastr("error", "Lưu thay đổi thất bại.");
            }
        }

        private async Task DownloadPayslip(PayslipViewModel slip)
        {
            if (slip == null) return;
            
            try
            {
                var payroll = slip.OriginalModel;
                // Sync any changes made in UI back to original model for PDF
                payroll.BasicSalary = slip.BasicSalary;
                payroll.TotalWorkSalary = slip.TotalWorkSalary;
                payroll.OvertimeSalary = slip.OvertimeSalary;
                payroll.Allowance = slip.TotalAllowances;
                payroll.Bonus = slip.Bonus;
                payroll.BhxhAmount = slip.BhxhAmount;
                payroll.BhytAmount = slip.BhytAmount;
                payroll.BhtnAmount = slip.BhtnAmount;
                payroll.UnionFee = slip.UnionFee;
                payroll.TaxAmount = slip.TaxAmount;
                payroll.Deduction = slip.TotalDeductions;
                payroll.NetSalary = slip.NetSalary;

                var pdfBytes = await PdfService.GeneratePayslipPdfAsync(payroll);
                var fileName = $"Payslip_{slip.EmployeeCode}_{Month}_{Year}.pdf";
                
                await JS.InvokeVoidAsync("downloadFile", fileName, "application/pdf", Convert.ToBase64String(pdfBytes));
                await InvokeToastr("success", "Đã khởi tạo file PDF phiếu lương.");
            }
            catch (Exception ex)
            {
                await InvokeToastr("error", $"Lỗi tạo PDF: {ex.Message}");
            }
        }

        private List<PayslipViewModel> FilteredPayslips =>
            string.IsNullOrWhiteSpace(SearchTerm) 
            ? Payslips 
            : Payslips.Where(p => 
                (p.EmployeeName ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) || 
                (p.EmployeeCode ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

        private string GetStatusColor(string status)
        {
            return (status == "Đã chốt" || status == "Closed") ? "success" : "warning";
        }

        private async Task InvokeToastr(string type, string message)
        {
            try {
                // Only invoke JS if we're not pre-rendering on the server
                await JS.InvokeVoidAsync($"toastr.{type}", message);
            } catch {
                // Ignore JS errors during pre-rendering
            }
        }

        public class PayslipViewModel
        {
            public string Id { get; set; } = "";
            public string EmployeeId { get; set; } = string.Empty;
            public bool IsSelected { get; set; }
            public string EmployeeCode { get; set; } = string.Empty;
            public string EmployeeName { get; set; } = string.Empty;
            public string Department { get; set; } = string.Empty;
            public string AvatarUrl { get; set; } = string.Empty;
            
            public decimal BasicSalary { get; set; }
            public decimal InsuranceSalary { get; set; }
            public double ActualWorkDays { get; set; }
            public double LeaveDays { get; set; }
            public double HolidayDays { get; set; }
            public double StandardWorkDays { get; set; }
            public double OvertimeHours { get; set; }
            public double OvertimeHoursWeekend { get; set; }
            public double OvertimeHoursHoliday { get; set; }
            public decimal OvertimeSalaryWeekend { get; set; }
            public decimal OvertimeSalaryHoliday { get; set; }
            
            public decimal TotalWorkSalary { get; set; }
            public decimal OvertimeSalary { get; set; }
            public decimal TotalAllowances { get; set; }
            public decimal Bonus { get; set; }
            
            public decimal BhxhAmount { get; set; }
            public decimal BhytAmount { get; set; }
            public decimal BhtnAmount { get; set; }
            public decimal UnionFee { get; set; }
            public decimal TotalInsurance => BhxhAmount + BhytAmount + BhtnAmount + UnionFee;
            
            public decimal TaxableIncome { get; set; }
            public decimal PersonalDeduction { get; set; }
            public decimal DependentDeduction { get; set; }
            public decimal TaxAmount { get; set; }
            
            public decimal AdvanceAmount { get; set; }
            public decimal TotalDeductions { get; set; }
            
            public decimal GrandTotalDeductions => TotalInsurance + TaxAmount + AdvanceAmount + TotalDeductions;
            public decimal GrossSalary => TotalWorkSalary + OvertimeSalary + TotalAllowances + Bonus;
            public decimal NetSalary { get; set; }
            
            public string Status { get; set; } = string.Empty;
            public string? WarningMessage { get; set; }
            public double OtRateWeekday { get; set; } = 1.5;
            public double OtRateWeekend { get; set; } = 2.0;
            public double OtRateHoliday { get; set; } = 3.0;
            
            public PayrollModel OriginalModel { get; set; } = new();

            public void CalculateNet()
            {
                // Re-calculate basic components based on daily rate logic
                if (StandardWorkDays > 0)
                {
                    decimal dailyRate = BasicSalary / (decimal)StandardWorkDays;
                    TotalWorkSalary = Math.Round(dailyRate * (decimal)ActualWorkDays, 0);
                    
                    decimal hourlyRate = dailyRate / 8m;
                    
                    double otNormalHours = Math.Max(0, OvertimeHours - OvertimeHoursWeekend - OvertimeHoursHoliday);
                    
                    OvertimeSalaryWeekend = Math.Round(hourlyRate * (decimal)OvertimeHoursWeekend * (decimal)OtRateWeekend, 0);
                    OvertimeSalaryHoliday = Math.Round(hourlyRate * (decimal)OvertimeHoursHoliday * (decimal)OtRateHoliday, 0);
                    
                    OvertimeSalary = Math.Round(hourlyRate * (decimal)otNormalHours * (decimal)OtRateWeekday, 0)
                                     + OvertimeSalaryWeekend 
                                     + OvertimeSalaryHoliday;
                }

                NetSalary = GrossSalary - TotalInsurance - TaxAmount - AdvanceAmount - TotalDeductions;
            }
        }
    }
}
