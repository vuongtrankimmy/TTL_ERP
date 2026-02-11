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
        private List<LeaveRequestModel> _filteredRequests = new();

        private bool _showDetail = false;
        private bool _showActionModal = false;
        private bool _isLoading = true;
        private LeaveRequestModel? _selectedRequest;
        private LeaveRequestModel? _requestToProcess;

        private string _searchTerm = "";

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            _isLoading = true;
            try
            {
                var result = await LeaveService.GetLeaveRequestsAsync();
                _requests = result.ToList();
                ApplyFilter();
            }
            catch (Exception)
            {
                await JS.InvokeVoidAsync("toastr.error", "Lỗi tải danh sách yêu cầu nghỉ phép.");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(_searchTerm))
            {
                _filteredRequests = _requests;
            }
            else
            {
                _filteredRequests = _requests.Where(r => 
                    r.EmployeeName.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    r.Type.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }
        }

        private void OnSearch(ChangeEventArgs e)
        {
            _searchTerm = e.Value?.ToString() ?? "";
            ApplyFilter();
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
                var success = await LeaveService.ProcessLeaveRequestAsync(_requestToProcess.Id, approved, reason);
                if (success)
                {
                    await JS.InvokeVoidAsync("toastr.success", approved ? "Đã phê duyệt yêu cầu." : "Đã từ chối yêu cầu.");
                    await LoadData();
                    if (_selectedRequest?.Id == _requestToProcess.Id)
                    {
                        CloseDetail();
                    }
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", "Có lỗi xảy ra khi xử lý yêu cầu.");
                }
            }
            catch (Exception)
            {
                await JS.InvokeVoidAsync("toastr.error", "Có lỗi hệ thống.");
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
            "Nghỉ phép" or "Annual Leave" => "badge-light-primary",
            "Công tác" or "Business Trip" => "badge-light-info",
            "Nghỉ ốm" or "Sick Leave" => "badge-light-danger",
            "Làm từ xa" or "WFH" => "badge-light-success",
            "Thai sản" or "Maternity Leave" => "badge-light-purple",
            _ => "badge-light-dark"
        };

        private string GetStatusClass(string status) => status switch
        {
            "Đã phê duyệt" or "Approved" => "badge-light-success",
            "Từ chối" or "Rejected" => "badge-light-danger",
            "Chờ phê duyệt" or "Pending" or "Chờ duyệt" => "badge-light-warning",
            _ => "badge-light-secondary"
        };
    }
}
