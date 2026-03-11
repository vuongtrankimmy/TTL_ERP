using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Attendance.Interfaces;
using TTL.HR.Application.Modules.Attendance.Models;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Organization.Models;
using TTL.HR.Application.Modules.Common.Constants;
using TTL.HR.Application.Modules.Common.Interfaces;
using Microsoft.Extensions.Localization;

namespace TTL.HR.Shared.Pages.Attendance
{
    public partial class Attendance
    {
        [Inject] private IAttendanceService AttendanceService { get; set; } = default!;
        [Inject] private HttpClient _httpClient { get; set; } = default!;
        [Inject] private IMasterDataService MasterDataService { get; set; } = default!;
        [Inject] private IStringLocalizer<SharedResource> L { get; set; } = default!;
        [Inject] private INavigationService NavigationService { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;

        private bool _showDetail = false;
        private bool _isLoading = true;
        private AttendanceModel? _selectedEmployee;
        private List<AttendanceModel> _employees = new();
        private List<AttendanceDetailModel> _selectedDetails = new();
        private List<DepartmentModel> _departments = new();
        private List<DateTime> _periods = new();
        private string _searchTerm = "";
        private string _selectedDepartmentId = "";
        private int _currentMonth = DateTime.Now.Month;
        private int _currentYear = DateTime.Now.Year;
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalItems = 0;
        private bool _isJsReady = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _isJsReady = true;
            }
        }

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            if (!await NavigationService.UserHasPermissionAsync("Permissions.Attendance.View"))
            {
                Nav.NavigateTo("/dashboard");
                return;
            }

            // Generate last 12 months for periods
            var today = DateTime.Today;
            for (int i = 0; i < 12; i++)
            {
                _periods.Add(new DateTime(today.Year, today.Month, 1).AddMonths(-i));
            }

            var depts = await MasterDataService.GetCachedLookupsAsync("Department");
            _departments = depts.Select(d => new DepartmentModel { Id = d.Id, Name = d.Name }).ToList();

            await LoadData();
        }

        [Inject] public IJSRuntime JS { get; set; } = default!;

        private async System.Threading.Tasks.Task LoadData()
        {
            _isLoading = true;
            try
            {
                var result = await AttendanceService.GetTimesheetsAsync(_currentMonth, _currentYear, _selectedDepartmentId, _searchTerm, _currentPage, _pageSize);
                _employees = result?.Items?.ToList() ?? new List<AttendanceModel>();
                _totalItems = (int)(result?.TotalCount ?? 0);
            }
            catch (Exception ex)
            {
                if (_isJsReady)
                {
                    await JS.InvokeVoidAsync("toastr.error", string.Format(L["Message_LoadDataError"], ex.Message));
                }
                else
                {
                    System.Console.WriteLine($"[Attendance] LoadData Error: {ex.Message}");
                }
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private async Task RefreshData() => await LoadData();

        private async Task OnSearchChange(string searchTerm)
        {
            _searchTerm = searchTerm;
            _currentPage = 1;
            await LoadData();
        }

        private async Task OnDepartmentChange(ChangeEventArgs e)
        {
            _selectedDepartmentId = e.Value?.ToString() ?? "";
            _currentPage = 1;
            await LoadData();
        }

        private async Task OnPageChange(int page)
        {
            _currentPage = page;
            await LoadData();
        }
        
        private async Task OnPeriodChange(ChangeEventArgs e)
        {
             if (DateTime.TryParse(e.Value?.ToString(), out DateTime date))
             {
                 _currentMonth = date.Month;
                 _currentYear = date.Year;
                 await LoadData();
             }
        }

        private async System.Threading.Tasks.Task openDetail(AttendanceModel emp)
        {
            _selectedEmployee = emp;
            _selectedDetails = (await AttendanceService.GetAttendanceDetailsAsync(emp.EmployeeId, new DateTime(_currentYear, _currentMonth, 1))).ToList();
            _showDetail = true;
            StateHasChanged();
        }

        private void closeDetail()
        {
            _showDetail = false;
        }

        private List<AttendanceModel> FilteredEmployees => _employees;
        
        private bool _isPeriodLocked => _employees.Any() && _employees.All(e => e.Status == "Locked" || e.Status == "Closed" || e.Status == "Đã chốt");

        private async Task LockMonth()
        {
            try
            {
                _isLoading = true;
                StateHasChanged();

                var response = await AttendanceService.CloseMonthlyAsync(_currentMonth, _currentYear);
                if (response.Success)
                {
                    if (_isJsReady) await JS.InvokeVoidAsync("toastr.success", L["Message_LockMonthSuccess"]);
                    await LoadData();
                }
                else
                {
                    if (_isJsReady) await JS.InvokeVoidAsync("toastr.error", response.Message ?? L["Message_LockMonthError"]);
                }
            }
            catch (Exception ex)
            {
                if (_isJsReady) await JS.InvokeVoidAsync("toastr.error", L["Message_ErrorOccurred"] + ": " + ex.Message);
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private async Task LockTimesheet(string id)
        {
            try
            {
                _isLoading = true;
                StateHasChanged();

                var response = await AttendanceService.CloseMonthlyAsync(_currentMonth, _currentYear, id);
                if (response.Success)
                {
                    if (_isJsReady) await JS.InvokeVoidAsync("toastr.success", "Chốt công nhân viên thành công.");
                    await LoadData();
                }
                else
                {
                    if (_isJsReady) await JS.InvokeVoidAsync("toastr.error", response.Message ?? "Lỗi khi chốt công");
                }
            }
            catch (Exception ex)
            {
                if (_isJsReady) await JS.InvokeVoidAsync("toastr.error", "Có lỗi xảy ra: " + ex.Message);
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private async Task ExportExcel()
        {
            try
            {
                await JS.InvokeVoidAsync("Swal.fire", new
                {
                    title = "Đang chuẩn bị trích xuất",
                    html = "<div class='progress mt-3' style='height: 20px;'><div id='swal-progress-bar' class='progress-bar progress-bar-striped progress-bar-animated' role='progressbar' style='width: 0%'>0%</div></div><div id='swal-status-text' class='mt-2 text-muted fs-7'>Khởi tạo yêu cầu...</div>",
                    allowOutsideClick = false,
                    showConfirmButton = false,
                    didOpen = new JSObjectReference()
                });

                var startResponse = await AttendanceService.ExportTimesheetStartAsync(_currentMonth, _currentYear, _searchTerm, _selectedDepartmentId);
                if (!startResponse.Success || string.IsNullOrEmpty(startResponse.Data))
                {
                    await JS.InvokeVoidAsync("Swal.fire", "Lỗi", startResponse.Message ?? "Không thể khởi tạo tiến trình trích xuất", "error");
                    return;
                }

                var jobId = startResponse.Data;
                bool isFinished = false;
                int retryCount = 0;

                while (!isFinished && retryCount < 60) // Max 5 minutes (5s * 60)
                {
                    await Task.Delay(2000); // Poll every 2 seconds
                    var statusResponse = await AttendanceService.GetExportStatusAsync(jobId);
                    
                    if (statusResponse != null && statusResponse.Success && statusResponse.Data != null)
                    {
                        var info = statusResponse.Data;
                        
                        // Update Progress Bar in Swal
                        await JS.InvokeVoidAsync("eval", $@"
                            var bar = document.getElementById('swal-progress-bar');
                            var text = document.getElementById('swal-status-text');
                            if(bar) {{ bar.style.width = '{info.Percentage}%'; bar.innerText = '{info.Percentage}%'; }}
                            if(text) {{ text.innerText = '{info.Status ?? "Đang xử lý..."}'; }}
                        ");

                        if (info.IsCompleted)
                        {
                            isFinished = true;
                            await JS.InvokeVoidAsync("eval", "var text = document.getElementById('swal-status-text'); if(text) text.innerText = 'Đang tải file...';");
                            
                            var fileBytes = await AttendanceService.DownloadExportedFileAsync(jobId);
                            await JS.InvokeVoidAsync("Swal.close");

                            if (fileBytes != null && fileBytes.Length > 0)
                            {
                                var fileName = $"TTL_BangCong_{_currentMonth}_{_currentYear}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                                await JS.InvokeVoidAsync("LayoutHelper.downloadFile", fileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileBytes);
                                await JS.InvokeVoidAsync("toastr.success", "Đã xuất file thành công!");
                            }
                            else
                            {
                                await JS.InvokeVoidAsync("Swal.fire", "Thông báo", "Lỗi khi tải file đã trích xuất.", "error");
                            }
                        }
                        else if (info.IsFailed)
                        {
                            isFinished = true;
                            await JS.InvokeVoidAsync("Swal.fire", "Lỗi", "Quá trình trích xuất thất bại: " + info.Error, "error");
                        }
                    }
                    else
                    {
                        retryCount++;
                    }
                }

                if (!isFinished)
                {
                    await JS.InvokeVoidAsync("Swal.fire", "Thông báo", "Yêu cầu quá thời gian xử lý. Vui lòng kiểm tra lại sau.", "warning");
                }
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("Swal.fire", "Lỗi", "Lỗi hệ thống khi xuất file: " + ex.Message, "error");
            }
        }

        private class JSObjectReference { }


        private string GetShiftClass(string? color, string prefix = "bg-")
        {
            if (string.IsNullOrEmpty(color)) return prefix + "secondary";
            if (color.StartsWith("#")) return ""; // Custom hex doesn't use classes
            return prefix + color;
        }

        private string GetShiftStyle(string? color, bool isBackground = true)
        {
            if (string.IsNullOrEmpty(color) || !color.StartsWith("#")) return "";
            return isBackground ? $"background-color: {color} !important; border-color: {color} !important; color: white !important;" 
                                : $"color: {color} !important;";
        }
    }
}
