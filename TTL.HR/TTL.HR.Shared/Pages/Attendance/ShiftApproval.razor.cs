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

        private bool _showDetail = false;
        private bool _isLoading = true;
        private ShiftRequestModel? _selectedRequest;
        private int activeTab = 0;
        private List<ShiftRequestModel> _requests = new();

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async System.Threading.Tasks.Task LoadData()
        {
            _isLoading = true;
            try
            {
                var result = await AttendanceService.GetShiftRequestsAsync();
                if (result != null)
                {
                    _requests = result.ToList();
                }
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

        private void SetTab(int index)
        {
            activeTab = index;
        }

        private IEnumerable<ShiftRequestModel> FilteredRequests
        {
            get
            {
                return activeTab switch
                {
                    0 => _requests.Where(x => x.IsPending),
                    1 => _requests.Where(x => x.Status == "Đã duyệt" || x.Status == "Approved"),
                    2 => _requests.Where(x => x.Status == "Từ chối" || x.Status == "Rejected"),
                    _ => _requests
                };
            }
        }

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
