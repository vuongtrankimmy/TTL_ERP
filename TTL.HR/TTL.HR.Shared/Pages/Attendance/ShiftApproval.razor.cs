using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using TTL.HR.Application.Modules.Attendance.Interfaces;
using TTL.HR.Application.Modules.Attendance.Models;

namespace TTL.HR.Shared.Pages.Attendance
{
    public partial class ShiftApproval
    {
        [Inject] private IAttendanceService AttendanceService { get; set; } = default!;
        [Inject] private TTL.HR.Application.Modules.Common.Interfaces.IAuthService AuthService { get; set; } = default!;

        private TTL.HR.Application.Modules.Common.Models.UserDto? _currentUser;

        private bool _showDetail = false;
        private bool _isLoading = true;
        private ShiftRequestModel? _selectedRequest;
        private int activeTab = 0;
        private List<ShiftRequestModel> _requests = new();
        private ShiftRequestSummaryModel _summary = new();

        // Paging
        private int _pageIndex = 1;
        private int _pageSize = 10;
        private long _totalCount = 0;
        private string? _searchTerm;

        private string GetShiftStyle(string? color, bool isBackground = true)
        {
            if (string.IsNullOrEmpty(color)) return "";
            if (color.StartsWith("#"))
            {
                return isBackground ? $"background-color: {color} !important;" : $"color: {color} !important;";
            }
            return "";
        }

        private string GetShiftClass(string? color, string prefix = "badge-")
        {
            if (string.IsNullOrEmpty(color)) return $"{prefix}secondary";
            if (color.StartsWith("#")) return "";
            return $"{prefix}{color}";
        }

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            _currentUser = await AuthService.GetCurrentUserAsync();
            await LoadData();
        }

        private async System.Threading.Tasks.Task LoadData()
        {
            _isLoading = true;
            try
            {
                string? status = activeTab switch
                {
                    0 => "Pending",
                    1 => "Approved",
                    2 => "Rejected",
                    _ => null
                };

                var result = await AttendanceService.GetShiftRequestsAsync(_pageIndex, _pageSize, status, _searchTerm);
                if (result != null)
                {
                    _requests = result.Items;
                    _totalCount = result.TotalCount;
                }

                _summary = await AttendanceService.GetShiftRequestSummaryAsync();
            }
            catch (Exception)
            {
                // Error handling
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async System.Threading.Tasks.Task SetTab(int index)
        {
            activeTab = index;
            _pageIndex = 1;
            await LoadData();
        }

        private async System.Threading.Tasks.Task ChangePage(int newPage)
        {
            if (newPage < 1 || newPage > TotalPages) return;
            _pageIndex = newPage;
            await LoadData();
        }

        private async System.Threading.Tasks.Task Search(ChangeEventArgs e)
        {
            _searchTerm = e.Value?.ToString();
            _pageIndex = 1;
            await LoadData();
        }

        private IEnumerable<ShiftRequestModel> FilteredRequests => _requests;

        private int TotalPages => (int)Math.Ceiling(_totalCount / (double)_pageSize);

        private bool _showActionModal = false;
        private string _modalTitle = "";
        private string _modalHeading = "";
        private string _modalMessage = "";
        private string _modalColor = "primary";
        private string _modalIcon = "";
        private string _modalConfirmLabel = "";
        private bool _showReasonInput = false;
        private string _actionType = ""; // "APPROVE" or "REJECT"
        private ShiftRequestModel? _requestToProcess;

        private void OpenDetail(ShiftRequestModel req)
        {
            _selectedRequest = req;
            _showDetail = true;
        }

        private void CloseDetail()
        {
            _showDetail = false;
            _selectedRequest = null;
        }

        private void ApproveRequest(ShiftRequestModel req)
        {
            _requestToProcess = req;
            _actionType = "APPROVE";
            _modalTitle = "Phê duyệt yêu cầu";
            _modalHeading = "Xác nhận phê duyệt?";
            _modalMessage = $"Bạn có chắc chắn muốn phê duyệt yêu cầu đổi ca cho nhân viên {req.EmployeeName} ({req.EmployeeId})?";
            _modalColor = "success";
            _modalIcon = "ki-outline ki-check-circle";
            _modalConfirmLabel = "Đồng ý Duyệt";
            _showReasonInput = false;
            _showActionModal = true;
        }

        private bool CanApprove(ShiftRequestModel req)
        {
            if (_currentUser == null) return false;
            if (_currentUser.Role == "Admin" || _currentUser.Role == "SuperAdmin") return true;
            if (!string.IsNullOrEmpty(req.PendingApproverId) && req.PendingApproverId == _currentUser.Id) return true;
            // Fallback for old records or generic states
            return string.IsNullOrEmpty(req.PendingApproverId);
        }

        private void RejectRequest(ShiftRequestModel req)
        {
            _requestToProcess = req;
            _actionType = "REJECT";
            _modalTitle = "Từ chối yêu cầu";
            _modalHeading = "Xác nhận từ chối?";
            _modalMessage = $"Hành động này sẽ từ chối yêu cầu đổi sang {req.TargetShift} của {req.EmployeeName}. Vui lòng nhập lý do bên dưới.";
            _modalColor = "danger";
            _modalIcon = "ki-outline ki-cross-circle";
            _modalConfirmLabel = "Xác nhận Từ chối";
            _showReasonInput = true;
            _showActionModal = true;
        }

        private void CloseModal()
        {
            _showActionModal = false;
            _requestToProcess = null;
        }

        private async System.Threading.Tasks.Task ConfirmAction(string reason)
        {
            if (_requestToProcess == null) return;

            bool isApprove = _actionType == "APPROVE";
            var success = await AttendanceService.ProcessShiftRequestAsync(_requestToProcess.Id, isApprove, reason);
            
            if (success)
            {
                await LoadData();
                if (_selectedRequest?.Id == _requestToProcess.Id)
                {
                    CloseDetail();
                }
                CloseModal();
            }
        }
    }
}
