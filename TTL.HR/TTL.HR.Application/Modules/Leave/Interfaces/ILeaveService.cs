using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Leave.Models;

namespace TTL.HR.Application.Modules.Leave.Interfaces
{
    public interface ILeaveService
    {
        Task<IEnumerable<LeaveRequestModel>> GetLeaveRequestsAsync();
        Task<bool> SubmitLeaveRequestAsync(LeaveRequestModel request);
        Task<bool> ApproveLeaveRequestAsync(string id, string status);
        Task<bool> ProcessLeaveRequestAsync(string id, bool approved, string? note);
    }
}
