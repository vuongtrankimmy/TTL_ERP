using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace TTL.HR.Shared.Pages.Leave
{
    public partial class LeaveApprovals
    {
        private string searchQuery = "";
        private bool _showDetail = false;
        private bool _showActionModal = false;
        private bool _isLoading = true;
        private ApprovalItem? _selectedRequest;
        private ApprovalItem? _requestToProcess;

        [Inject] private ILeaveService LeaveService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            _isLoading = true;
            try
            {
                var requests = await LeaveService.GetLeaveRequestsAsync();
                if (requests != null)
                {
                    _approvals = requests.Select(r => new ApprovalItem
                    {
                        Id = int.TryParse(r.Id, out var id) ? id : 0,
                        Name = r.EmployeeName,
                        Department = "General",
                        Avatar = "",
                        Type = r.Type,
                        DateRange = $"{r.StartDate:dd/MM} - {r.EndDate:dd/MM}",
                        Duration = r.TotalDays.ToString("F1"),
                        Reason = r.Reason,
                        Status = r.Status
                    }).ToList();
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

        private string _modalTitle = "";
        private string _modalHeading = "";
        private string _modalMessage = "";
        private string _modalColor = "primary";
        private string _modalIcon = "";
        private string _modalConfirmLabel = "";
        private bool _showReasonInput = false;
        private string _actionType = ""; // "APPROVE" or "REJECT"

        private void ViewDetail(int id)
        {
            _selectedRequest = _approvals.FirstOrDefault(x => x.Id == id);
            _showDetail = true;
        }

        private void CloseDetail() => _showDetail = false;

        private void RequestApprove(ApprovalItem item)
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

        private void RequestReject(ApprovalItem item)
        {
            _requestToProcess = item;
            _actionType = "REJECT";
            _modalTitle = "Từ chối nghỉ phép";
            _modalHeading = "Xác nhận từ chối?";
            _modalMessage = $"Hành động này sẽ từ chối yêu cầu {item.Type} của {item.Name}. Vui lòng nhập lý do từ chối bên dưới.";
            _modalColor = "danger";
            _modalIcon = "ki-outline ki-cross-circle";
            _modalConfirmLabel = "Xác nhận Từ chối";
            _showReasonInput = true;
            _showActionModal = true;
        }

        private async Task ConfirmAction(string reason)
        {
            if (_requestToProcess == null) return;

            string status = _actionType == "APPROVE" ? "Approved" : "Rejected";
            var success = await LeaveService.ApproveLeaveRequestAsync(_requestToProcess.Id.ToString(), status);

            if (success)
            {
                _requestToProcess.Status = status;
                _requestToProcess.ManagerNote = reason;
                _approvals.Remove(_requestToProcess);
            }

            if (_selectedRequest == _requestToProcess)
            {
                CloseDetail();
            }

            CloseModal();
        }

        private void CloseModal()
        {
            _showActionModal = false;
            _requestToProcess = null;
        }

        private List<ApprovalItem> _approvals = new()
        {
        };

        private class ApprovalItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public string Department { get; set; } = "";
            public string Avatar { get; set; } = "";
            public string Type { get; set; } = "";
            public string DateRange { get; set; } = "";
            public string Duration { get; set; } = "";
            public string Reason { get; set; } = "";
            public string Status { get; set; } = "Pending";
            public string? ManagerNote { get; set; }
        }
    }
}
