using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Payroll.Models;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Application.Modules.Payroll.Interfaces
{
    public interface IPayrollService
    {
        Task<IEnumerable<PayrollModel>> GetPayrollAsync(int? month = null, int? year = null);
        Task<IEnumerable<PayrollModel>> GetPayrollsAsync(int? month = null, int? year = null);
        Task<IEnumerable<PayrollPeriodModel>> GetPeriodsAsync(int? year = null, int? month = null);
        Task<PayrollPeriodDetailModel?> GetPeriodDetailAsync(string? id, string? searchTerm = null, string? departmentId = null, int page = 1, int pageSize = 10, int? year = null, int? month = null);
        Task<bool> GeneratePayrollAsync(int month, int year);
        Task<bool> UpdatePayrollAsync(string id, PayrollModel payroll);
        Task<bool> LockPayrollAsync(int month, int year);
        Task<bool> LockPayrollAsync(string id);
        Task<bool> DeletePeriodAsync(string id);
        Task<PagedResult<PayrollModel>> GetMyPayrollsAsync(string? employeeId = null, int? year = null, int page = 1, int pageSize = 10);
    }
}
