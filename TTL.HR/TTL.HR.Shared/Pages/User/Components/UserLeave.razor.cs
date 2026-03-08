using Microsoft.AspNetCore.Components;
using TTL.HR.Application.Modules.Leave.Interfaces;
using TTL.HR.Application.Modules.Leave.Models;

namespace TTL.HR.Shared.Pages.User.Components;

public partial class UserLeave
{
    [Inject] public ILeaveService LeaveService { get; set; } = default!;

    [Parameter] public string? EmployeeId { get; set; }
    private string? employeeId => EmployeeId;

    private List<LeaveRequestModel> _leaveRequests = new();
    private List<LeaveTypeDto> _leaveTypes = new();
    private LeaveBalanceModel? _balance;
    private bool _isLoading = true;
    private bool _isModalOpen = false;
    private bool _isSubmitting = false;
    private string? _error;

    private LeaveRequestModel _newRequest = new() { StartDate = DateTime.Now, EndDate = DateTime.Now };

    protected override async Task OnInitializedAsync()
    {
        _leaveTypes = await LeaveService.GetLeaveTypesAsync();
        await LoadData();
    }

    private async Task LoadData()
    {
        _isLoading = true;
        try
        {
            var result = await LeaveService.GetLeaveRequestsAsync(1, 10); // Simple fetch
            _leaveRequests = result.Items.Where(x => x.EmployeeId == employeeId).ToList();

            if (employeeId != null)
            {
                _balance = await LeaveService.GetLeaveBalanceAsync(employeeId, DateTime.Now.Year);
            }
        }
        catch { }
        _isLoading = false;
    }

    private void ShowRequestModal()
    {
        _error = null;
        _isModalOpen = true;
        _newRequest = new() { StartDate = DateTime.Today, EndDate = DateTime.Today, LeaveTypeId = "", EmployeeId = employeeId ?? "" };
    }

    private void CloseModal() => _isModalOpen = false;

    private async Task SubmitRequest()
    {
        if (string.IsNullOrEmpty(_newRequest.LeaveTypeId)) { _error = "Vui lòng chọn loại nghỉ."; return; }
        if (string.IsNullOrEmpty(_newRequest.Reason)) { _error = "Vui lòng nhập lý do."; return; }
        if (_newRequest.EndDate < _newRequest.StartDate) { _error = "Ngày kết thúc không thể trước ngày bắt đầu."; return; }

        _isSubmitting = true;
        _error = null;

        var response = await LeaveService.SubmitLeaveRequestAsync(_newRequest);
        if (response.Success)
        {
            await LoadData();
            CloseModal();
        }
        else
        {
            _error = response.Message ?? "Không thể gửi yêu cầu. Vui lòng kiểm tra lại quỹ phép của bạn.";
        }
        _isSubmitting = false;
    }
}
