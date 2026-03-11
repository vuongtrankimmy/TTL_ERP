using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Attendance.Interfaces;
using TTL.HR.Application.Modules.Attendance.Models;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Common.Interfaces;

namespace TTL.HR.Shared.Pages.Attendance
{
    public partial class OvertimeApproval
    {
        [Inject] private IAttendanceService AttendanceService { get; set; } = default!;
        [Inject] private IAuthService AuthService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private IStringLocalizer<SharedResource> L { get; set; } = default!;
        [Inject] private INavigationService NavigationService { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;

        private UserDto? _currentUser;
        private bool _isLoading = true;
        private bool _showDetail = false;
        private int activeTab = 0;
        private string? _searchTerm;
        private int _pageIndex = 1;
        private int _pageSize = 10;
        private long _totalCount = 0;

        private List<OvertimeRequestModel> _requests = new();
        private OvertimeSummaryModel _summary = new();
        private OvertimeRequestModel? _selectedRequest;

        // Modal state
        private bool _showActionModal = false;
        private string _modalTitle = "";
        private string _modalHeading = "";
        private string _modalMessage = "";
        private string _modalColor = "primary";
        private string _modalIcon = "";
        private string _modalConfirmLabel = "";
        private bool _showReasonInput = false;
        private string _actionType = ""; // "APPROVE" or "REJECT"
        private OvertimeRequestModel? _requestToProcess;

        protected override async Task OnInitializedAsync()
        {
            if (!await NavigationService.UserHasPermissionAsync("Permissions.ShiftRequests.View"))
            {
                Nav.NavigateTo("/dashboard");
                return;
            }
            _currentUser = await AuthService.GetCurrentUserAsync();
            await LoadData();
        }

        private async Task LoadData()
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

                var result = await AttendanceService.GetOvertimeRequestsAsync(_pageIndex, _pageSize, status, _searchTerm);
                if (result != null)
                {
                    _requests = result.Items;
                    _totalCount = result.TotalCount;
                }

                _summary = await AttendanceService.GetOvertimeSummaryAsync();
            }
            catch (Exception ex)
            {
                try
                {
                    await JS.InvokeVoidAsync("toastr.error", string.Format(L["Message_LoadDataError"], ex.Message));
                }
                catch (JSDisconnectedException) { /* Circuit disconnected, ignore */ }
                catch (Exception) { /* Ignore JS errors when circuit is unstable */ }
                
                Console.WriteLine($"Error loading overtime data: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task SetTab(int index)
        {
            activeTab = index;
            _pageIndex = 1;
            await LoadData();
        }

        private async Task Search(ChangeEventArgs e)
        {
            _searchTerm = e.Value?.ToString();
            _pageIndex = 1;
            await LoadData();
        }

        private async Task ChangePage(int newPage)
        {
            if (newPage < 1 || newPage > TotalPages) return;
            _pageIndex = newPage;
            await LoadData();
        }

        private int TotalPages => (int)Math.Ceiling(_totalCount / (double)_pageSize);

        private void OpenDetail(OvertimeRequestModel req)
        {
            _selectedRequest = req;
            _showDetail = true;
        }

        private void CloseDetail()
        {
            _showDetail = false;
            _selectedRequest = null;
        }

        private void ApproveRequest(OvertimeRequestModel req)
        {
            _requestToProcess = req;
            _actionType = "APPROVE";
            _modalTitle = L["Page_Overtime_Title"];
            _modalHeading = L["Label_ConfirmApproveHeading"];
            _modalMessage = string.Format(L["Label_ConfirmApproveMessage"], req.Hours, req.Date, req.EmployeeName);
            _modalColor = "success";
            _modalIcon = "ki-outline ki-check-circle";
            _modalConfirmLabel = L["Label_ConfirmApproveAction"];
            _showReasonInput = false;
            _showActionModal = true;
        }

        private void RejectRequest(OvertimeRequestModel req)
        {
            _requestToProcess = req;
            _actionType = "REJECT";
            _modalTitle = L["Label_Reject"];
            _modalHeading = L["Label_ConfirmRejectHeading"];
            _modalMessage = string.Format(L["Label_ConfirmRejectMessage"], req.EmployeeName);
            _modalColor = "danger";
            _modalIcon = "ki-outline ki-cross-circle";
            _modalConfirmLabel = L["Label_ConfirmRejectAction"];
            _showReasonInput = true;
            _showActionModal = true;
        }

        private void CloseModal()
        {
            _showActionModal = false;
            _requestToProcess = null;
        }

        private async Task ConfirmAction(string reason)
        {
            if (_requestToProcess == null) return;

            bool isApprove = _actionType == "APPROVE";
            var success = await AttendanceService.ProcessOvertimeRequestAsync(_requestToProcess.Id, isApprove, reason);
            
            if (success)
            {
                await JS.InvokeVoidAsync("toastr.success", isApprove ? L["Message_ApproveSuccess"] : L["Message_RejectSuccess"]);
                await LoadData();
                if (_selectedRequest?.Id == _requestToProcess.Id)
                {
                    CloseDetail();
                }
                CloseModal();
            }
            else
            {
                await JS.InvokeVoidAsync("toastr.error", L["Message_ProcessError"]);
            }
        }
    }
}
