using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Leave.Interfaces;
using TTL.HR.Application.Modules.Leave.Models;

namespace TTL.HR.Shared.Pages.Attendance
{
    public partial class LeaveTripManagement
    {
        [Inject] public ILeaveService LeaveService { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;

        private List<LeaveRequestModel> _requests = new();
        private LeaveStateSummaryModel _summary = new();
        private string _activeTab = "All";
        private bool _isLoading = true;
        private bool _showDetail = false;
        private bool _showActionModal = false;
        private LeaveRequestModel? _selectedRequest;
        private LeaveRequestModel? _requestToProcess;
        private string _searchTerm = "";

        private int _pageIndex = 1;
        private int _pageSize = 10;
        private long _totalCount = 0;
        private int TotalPages => (int)Math.Ceiling(_totalCount / (double)_pageSize);

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            _isLoading = true;
            try
            {
                var summaryTask = LeaveService.GetLeaveSummaryAsync();
                var statusFilter = _activeTab == "All" ? null : _activeTab;
                var requestsTask = LeaveService.GetLeaveRequestsAsync(_pageIndex, _pageSize, statusFilter, _searchTerm);

                await Task.WhenAll(summaryTask, requestsTask);

                _summary = await summaryTask;
                var result = await requestsTask;

                _requests = result?.Items ?? new List<LeaveRequestModel>();
                _totalCount = result?.TotalCount ?? 0;
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("toastr.error", "Lỗi tải dữ liệu: " + ex.Message);
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private async Task SetTab(string tab)
        {
            _activeTab = tab;
            _pageIndex = 1;
            await LoadData();
        }

        private async Task ChangePage(int newPage)
        {
            if (newPage < 1 || newPage > TotalPages) return;
            _pageIndex = newPage;
            await LoadData();
        }

        private async Task OnSearch(ChangeEventArgs e)
        {
            _searchTerm = e.Value?.ToString() ?? "";
            _pageIndex = 1;
            await LoadData();
        }

        private string _modalTitle = "";
        private string _modalHeading = "";
        private string _modalMessage = "";
        private string _modalColor = "primary";
        private string _modalIcon = "";
        private string _modalConfirmLabel = "";
        private bool _showReasonInput = false;
        private string _actionType = ""; // "APPROVE" or "REJECT"

        private void OpenDetail(LeaveRequestModel req)
        {
            _selectedRequest = req;
            _showDetail = true;
        }

        private void CloseDetail()
        {
            _showDetail = false;
            _selectedRequest = null;
        }

        private void RequestApprove(LeaveRequestModel req)
        {
            _requestToProcess = req;
            _actionType = "APPROVE";
            _modalTitle = "Phê duyệt yêu cầu";
            _modalHeading = "Xác nhận phê duyệt?";
            _modalMessage = $"Bạn có chắc chắn muốn phê duyệt yêu cầu {req.Type} của {req.EmployeeName}?";
            _modalColor = "success";
            _modalIcon = "ki-outline ki-check-circle";
            _modalConfirmLabel = "Đồng ý Duyệt";
            _showReasonInput = false;
            _showActionModal = true;
        }

        private void RequestReject(LeaveRequestModel req)
        {
            _requestToProcess = req;
            _actionType = "REJECT";
            _modalTitle = "Từ chối yêu cầu";
            _modalHeading = "Xác nhận từ chối?";
            _modalMessage = $"Hành động này sẽ từ chối yêu cầu {req.Type} của {req.EmployeeName}. Vui lòng nhập lý do từ chối bên dưới.";
            _modalColor = "danger";
            _modalIcon = "ki-outline ki-cross-circle";
            _modalConfirmLabel = "Xác nhận Từ chối";
            _showReasonInput = true;
            _showActionModal = true;
        }

        private async Task ConfirmAction(string reason)
        {
            if (_requestToProcess == null) return;

            bool approved = _actionType == "APPROVE";
            
            try 
            {
                var response = await LeaveService.ProcessLeaveRequestAsync(_requestToProcess.Id, approved, reason);
                if (response.Success)
                {
                    await JS.InvokeVoidAsync("toastr.success", approved ? "Đã phê duyệt yêu cầu thành công." : "Đã từ chối yêu cầu thành công.");
                    await LoadData();
                    if (_selectedRequest?.Id == _requestToProcess.Id)
                    {
                        CloseDetail();
                    }
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", $"Thao tác thất bại: {response.Message}");
                }
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("toastr.error", "Có lỗi hệ thống: " + ex.Message);
            }
            
            CloseModal();
        }

        private void CloseModal()
        {
            _showActionModal = false;
            _requestToProcess = null;
        }

        private string GetTypeClass(string type) => type switch
        {
            "Nghỉ phép năm" or "Annual Leave" or "Nghỉ phép" => "badge-light-primary",
            "Công tác" or "Business Trip" or "Đi công tác" => "badge-light-info",
            "Nghỉ ốm" or "Sick Leave" or "Nghỉ ốm" => "badge-light-warning",
            "Làm từ xa" or "WFH" or "Remote" => "badge-light-success",
            "Thai sản" or "Maternity Leave" or "Thai sản" => "badge-light-purple",
            _ => "badge-light-dark"
        };

        private string GetStatusClass(string status) => status switch
        {
            "Đã phê duyệt" or "Approved" or "Đã duyệt" => "badge-light-success",
            "Từ chối" or "Rejected" => "badge-light-danger",
            "Chờ phê duyệt" or "Pending" or "Chờ duyệt" or "PartiallyApproved" => "badge-light-warning",
            _ => "badge-light-secondary"
        };
    }
}
