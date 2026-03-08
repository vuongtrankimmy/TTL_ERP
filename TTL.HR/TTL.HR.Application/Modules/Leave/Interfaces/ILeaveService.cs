using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Leave.Models;

namespace TTL.HR.Application.Modules.Leave.Interfaces
{
    public interface ILeaveService
    {
        Task<PagedResult<LeaveRequestModel>> GetLeaveRequestsAsync(int page = 1, int pageSize = 10, string? status = null, string? searchTerm = null);
        Task<LeaveStateSummaryModel> GetLeaveSummaryAsync();
        Task<LeaveBalanceModel?> GetLeaveBalanceAsync(string employeeId, int year);
        Task<List<LeaveTypeDto>> GetLeaveTypesAsync();
        Task<ApiResponse<string>> SubmitLeaveRequestAsync(LeaveRequestModel request);
        Task<ApiResponse<bool>> ProcessLeaveRequestAsync(string id, bool approved, string? note);
    }
}
