using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using TTL.HR.Application.Modules.Leave.Interfaces;
using TTL.HR.Application.Modules.Leave.Models;
using TTL.HR.Application.Modules.Common.Models;

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

        // Paging & Filter
        private int _pageIndex = 1;
        private int _pageSize = 10;
        private long _totalCount = 0;
        private string? _searchTerm;
        private string? _statusFilter;
        private LeaveStateSummaryModel _summary = new();
        
        [Inject] private ILeaveService LeaveService { get; set; } = default!;

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async System.Threading.Tasks.Task LoadData()
        {
            _isLoading = true;
            try
            {
                var result = await LeaveService.GetLeaveRequestsAsync(_pageIndex, _pageSize, _statusFilter, _searchTerm);
                if (result != null)
                {
                    _leaveRequests = result.Items.Select(r => new LeaveRequestItem
                    {
                        Id = r.Id,
                        Name = r.EmployeeName,
                        Department = r.Department,
                        Avatar = r.Avatar,
                        AvatarBg = "bg-primary",
                        Type = r.Type,
                        TypeColor = $"badge-light-{r.TypeColor}",
                        DateRange = $"{r.StartDate:dd/MM} - {r.EndDate:dd/MM}",
                        Duration = $"{r.TotalDays} ngày",
                        Reason = r.Reason,
                        Status = r.Status,
                        StatusColor = r.StatusBadgeClass
                    }).ToList();
                    _totalCount = result.TotalCount;
                }

                _summary = await LeaveService.GetLeaveSummaryAsync();
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

        private async Task FilterByStatus(ChangeEventArgs e)
        {
            var val = e.Value?.ToString();
            _statusFilter = (val == "All" || string.IsNullOrEmpty(val)) ? null : val;
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

        private void openDetail(LeaveRequestItem item)
        {
            _selectedRequest = item;
            _showDetail = true;
            _showCreate = false;
        }

        private void closeDetail() => _showDetail = false;

        private void openCreate()
        {
            _showCreate = true;
            _showDetail = false;
        }

        private void closeCreate() => _showCreate = false;

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

            bool isApprove = _actionType == "APPROVE";
            string status = isApprove ? "Approved" : "Rejected";
            var success = await LeaveService.ProcessLeaveRequestAsync(_requestToProcess.Id, isApprove, reason);

            if (success)
            {
                _requestToProcess.Status = status == "Approved" ? "Đã phê duyệt" : "Đã từ chối";
                _requestToProcess.StatusColor = status == "Approved" ? "badge-light-success" : "badge-light-danger";
                _requestToProcess.ManagerNote = reason;
            }

            if (_selectedRequest == _requestToProcess)
            {
                _showDetail = false;
            }

            closeModal();
        }

        private void closeModal()
        {
            _showActionModal = false;
            _requestToProcess = null;
        }

        private List<LeaveRequestItem> _leaveRequests = new();

        private class LeaveRequestItem
        {
            public string Id { get; set; } = "";
            public string Name { get; set; } = "";
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
        }
    }
}
