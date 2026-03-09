using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using TTL.HR.Application.Modules.Leave.Interfaces;
using TTL.HR.Application.Modules.Leave.Models;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.HumanResource.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Models;
using Microsoft.JSInterop;

namespace TTL.HR.Shared.Pages.Leave
{
    public partial class LeaveRequests
    {
        private bool _showDetail = false;
        private bool _showCreate = false;
        private bool _showActionModal = false;
        private bool _isLoading = true;
        private LeaveRequestItem? _selectedRequest;
        private LeaveRequestItem? _requestToProcess;
        private LeaveBalanceModel? _currentBalance;
        private string? _currentUserId;

        // Paging & Filter
        private int _pageIndex = 1;
        private int _pageSize = 10;
        private long _totalCount = 0;
        private string? _searchTerm;
        private string _activeTab = "All";
        private bool _assignedToMe = false;
        private string? _statusFilter;
        private LeaveStateSummaryModel _summary = new();
        
        private bool IsHR { get; set; } = false;
        private LeaveRequestModel _createModel = new() { StartDate = DateTime.Today, EndDate = DateTime.Today };
        private bool _isSaving = false;

        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private ILeaveService LeaveService { get; set; } = default!;
        [Inject] private IEmployeeService EmployeeService { get; set; } = default!;
        [Inject] private TTL.HR.Application.Modules.Common.Interfaces.IAuthService AuthService { get; set; } = default!;

        private List<EmployeeDto> _employees = new();
        private List<LeaveTypeDto> _leaveTypes = new();


        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            try
            {
                var user = await AuthService.GetCurrentUserAsync();
                _currentUserId = user?.Id;
                IsHR = user?.Role == "Admin" || user?.Role == "HR";
                try { _leaveTypes = await LeaveService.GetLeaveTypesAsync(); } catch { }
                if (IsHR)
                {
                    try { _employees = await EmployeeService.GetEmployeesAsync() ?? new(); } catch { }
                }
                
                // If there are items for approval, default to "Assigned" tab
                _summary = await LeaveService.GetLeaveSummaryAsync();
                if (_summary.AssignedToMeCount > 0)
                {
                    _activeTab = "Assigned";
                    _assignedToMe = true;
                }
                
                await LoadData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LeaveRequests] Init error: {ex.Message}");
            }
        }

        private async System.Threading.Tasks.Task LoadData()
        {
            _isLoading = true;
            try
            {
                var status = (_activeTab == "All" || _activeTab == "Assigned") ? null : _activeTab;
                var result = await LeaveService.GetLeaveRequestsAsync(_pageIndex, _pageSize, status, _searchTerm, _assignedToMe);
                if (result != null)
                {
                    _leaveRequests = result.Items.Select(r => new LeaveRequestItem
                    {
                        Id = r.Id,
                        EmployeeId = r.EmployeeId,
                        Name = r.EmployeeName,
                        EmployeeCode = r.EmployeeCode,
                        Department = r.Department,
                        Avatar = r.Avatar,
                        AvatarBg = "bg-primary",
                        Type = r.Type,
                        TypeColor = $"badge-light-{r.TypeColor}",
                        DateRange = $"{r.StartDate:dd/MM} - {r.EndDate:dd/MM}",
                        Duration = $"{r.TotalDays} ngày",
                        Reason = r.Reason,
                        Status = TranslateStatus(r.Status),
                        StatusColor = r.StatusBadgeClass,
                        PendingApproverId = r.PendingApproverId
                    }).ToList();
                    _totalCount = result.TotalCount;
                }

                _summary = await LeaveService.GetLeaveSummaryAsync();
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("toastr.error", $"Lỗi tải dữ liệu: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private async Task ChangePage(int newPage)
        {
            if (newPage < 1 || newPage > TotalPages) return;
            _pageIndex = newPage;
            await LoadData();
        }

        private async Task Search(ChangeEventArgs e)
        {
            _searchTerm = e.Value?.ToString();
            _pageIndex = 1;
            await LoadData();
        }

        private async Task SetTab(string tab)
        {
            _activeTab = tab;
            _assignedToMe = (tab == "Assigned");
            _pageIndex = 1;
            await LoadData();
        }

        private int TotalPages => (int)Math.Ceiling(_totalCount / (double)_pageSize);

        private string _modalTitle = "";
        private string _modalHeading = "";
        private string _modalMessage = "";
        private string _modalColor = "primary";
        private string _modalIcon = "";
        private string _modalConfirmLabel = "";
        private bool _showReasonInput = false;
        private string _actionType = ""; // "APPROVE" or "REJECT"

        private async Task openDetail(LeaveRequestItem item)
        {
            _selectedRequest = item;
            _showDetail = true;
            _showCreate = false;
            
            // Load actual balance
            try {
                _currentBalance = await LeaveService.GetLeaveBalanceAsync(item.EmployeeId, DateTime.Now.Year);
            } catch {
                _currentBalance = null;
            }
        }

        private void closeDetail() => _showDetail = false;

        private async Task openCreate()
        {
            _createModel = new LeaveRequestModel { StartDate = DateTime.Today, EndDate = DateTime.Today };
            var user = await AuthService.GetCurrentUserAsync();
            _createModel.EmployeeId = user?.Id ?? "";
            _showCreate = true;
            _showDetail = false;
        }

        private void closeCreate() => _showCreate = false;

        private async Task handleSubmitCreate()
        {
            // Calculate total days automatically if not set
            if (_createModel.TotalDays <= 0)
            {
                var days = (_createModel.EndDate - _createModel.StartDate).TotalDays + 1;
                _createModel.TotalDays = Math.Max(0, days);
            }

            if (string.IsNullOrEmpty(_createModel.LeaveTypeId))
            {
                await JS.InvokeVoidAsync("toastr.warning", "Vui lòng chọn loại nghỉ");
                return;
            }

            if (string.IsNullOrEmpty(_createModel.Reason))
            {
                await JS.InvokeVoidAsync("toastr.warning", "Vui lòng nhập lý do");
                return;
            }

            if (_createModel.EndDate < _createModel.StartDate)
            {
                await JS.InvokeVoidAsync("toastr.warning", "Ngày kết thúc không được trước ngày bắt đầu");
                return;
            }

            _isSaving = true;
            StateHasChanged();

            try
            {
                if (string.IsNullOrEmpty(_createModel.EmployeeId))
                {
                    var user = await AuthService.GetCurrentUserAsync();
                    _createModel.EmployeeId = user?.Id ?? "";
                }

                var response = await LeaveService.SubmitLeaveRequestAsync(_createModel);
                if (response.Success)
                {
                    await JS.InvokeVoidAsync("toastr.success", "Gửi yêu cầu thành công!");
                    closeCreate();
                    await LoadData();
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", $"Gửi yêu cầu thất bại: {response.Message}");
                }
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("toastr.error", $"Lỗi: {ex.Message}");
            }
            finally
            {
                _isSaving = false;
                StateHasChanged();
            }
        }

        private void requestApprove(LeaveRequestItem item)
        {
            _requestToProcess = item;
            _actionType = "APPROVE";
            _modalTitle = "Phê duyệt nghỉ phép";
            _modalHeading = "Xác nhận phê duyệt?";
            _modalMessage = $"Bạn có chắc chắn muốn phê duyệt yêu cầu {item.Type} của {item.Name}?";
            _modalColor = "success";
            _modalIcon = "ki-outline ki-check-circle";
            _modalConfirmLabel = "Xác nhận Duyệt";
            _showReasonInput = false;
            _showActionModal = true;
        }

        private void requestReject(LeaveRequestItem item)
        {
            _requestToProcess = item;
            _actionType = "REJECT";
            _modalTitle = "Từ chối nghỉ phép";
            _modalHeading = "Xác nhận từ chối?";
            _modalMessage = $"Hành động này sẽ từ chối yêu cầu {item.Type} của {item.Name}. Vui lòng nhập lý do từ chối.";
            _modalColor = "danger";
            _modalIcon = "ki-outline ki-cross-circle";
            _modalConfirmLabel = "Xác nhận Từ chối";
            _showReasonInput = true;
            _showActionModal = true;
        }

        private async Task confirmAction(string reason)
        {
            if (_requestToProcess == null) return;

            try 
            {
                bool isApprove = _actionType == "APPROVE";
                string status = isApprove ? "Approved" : "Rejected";
                var response = await LeaveService.ProcessLeaveRequestAsync(_requestToProcess.Id, isApprove, reason);

                if (response.Success)
                {
                    await JS.InvokeVoidAsync("toastr.success", isApprove ? "Đã phê duyệt yêu cầu." : "Đã từ chối yêu cầu.");
                    _requestToProcess.Status = status == "Approved" ? "Đã phê duyệt" : "Đã từ chối";
                    _requestToProcess.StatusColor = status == "Approved" ? "badge-light-success" : "badge-light-danger";
                    _requestToProcess.ManagerNote = reason;
                    await LoadData();
                    if (_selectedRequest?.Id == _requestToProcess.Id)
                    {
                        closeDetail();
                    }
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", $"Thao tác thất bại: {response.Message}");
                }
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("toastr.error", $"Lỗi hệ thống: {ex.Message}");
                Console.WriteLine($"[LeaveRequests] Process error: {ex}");
            }
            finally
            {
                closeModal();
            }
        }

        private void closeModal()
        {
            _showActionModal = false;
            _requestToProcess = null;
        }

        private bool CanApprove(LeaveRequestItem? item)
        {
            if (item == null || string.IsNullOrEmpty(_currentUserId)) return false;
            if (IsHR) return true;
            return !string.IsNullOrEmpty(item.PendingApproverId) && item.PendingApproverId == _currentUserId;
        }

        private List<LeaveRequestItem> _leaveRequests = new();

        private class LeaveRequestItem
        {
            public string Id { get; set; } = "";
            public string EmployeeId { get; set; } = "";
            public string Name { get; set; } = "";
            public string EmployeeCode { get; set; } = "";
            public string Department { get; set; } = "";
            public string Avatar { get; set; } = "";
            public string AvatarBg { get; set; } = "bg-primary";
            public string Type { get; set; } = "";
            public string TypeColor { get; set; } = "";
            public string DateRange { get; set; } = "";
            public string Duration { get; set; } = "";
            public string Reason { get; set; } = "";
            public string Status { get; set; } = "";
            public string StatusColor { get; set; } = "";
            public string? ManagerNote { get; set; }
            public string? PendingApproverId { get; set; }
        }
        private string TranslateStatus(string? status) => status switch
        {
            "Pending" => "Chờ duyệt",
            "PartiallyApproved" => "Duyệt cấp 1",
            "Approved" => "Đã duyệt",
            "Rejected" => "Từ chối",
            "Cancelled" => "Đã hủy",
            _ => status ?? "N/A"
        };
    }
}
