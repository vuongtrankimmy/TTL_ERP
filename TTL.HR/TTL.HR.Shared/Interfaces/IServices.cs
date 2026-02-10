using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Shared.Entities.HumanResource;
using TTL.HR.Shared.Entities.Recruitment;
using TTL.HR.Shared.Entities.Contracts;

namespace TTL.HR.Shared.Interfaces
{
    // Service Interfaces (Client-side usage)
    
    public interface IApiRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(string endpoint);
        Task<T?> GetByIdAsync(string endpoint, string id);
        Task<T?> CreateAsync(string endpoint, T entity);
        Task<bool> UpdateAsync(string endpoint, string id, T entity);
        Task<bool> DeleteAsync(string endpoint, string id);
    }

    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeModel>> GetEmployeesAsync();
        Task<EmployeeModel?> GetEmployeeAsync(string id);
        Task<EmployeeModel?> CreateEmployeeAsync(EmployeeModel employee);
        Task UpdateEmployeeAsync(string id, EmployeeModel employee);
        Task DeleteEmployeeAsync(string id);
    }

    public interface IDashboardService
    {
        Task<DashboardOverviewModel?> GetOverviewAsync();
    }

    public interface IAttendanceService
    {
        Task<IEnumerable<AttendanceModel>> GetTimesheetsAsync();
        Task<bool> CheckInAsync(AttendanceModel attendance);
        Task<bool> CheckOutAsync(string id, AttendanceModel attendance);
    }

    public interface ILeaveService
    {
        Task<IEnumerable<LeaveRequestModel>> GetLeaveRequestsAsync();
        Task<bool> SubmitLeaveRequestAsync(LeaveRequestModel request);
        Task<bool> ApproveLeaveRequestAsync(string id, string status);
    }

    public interface ITrainingService
    {
        Task<IEnumerable<CourseModel>> GetCoursesAsync();
        Task<CourseModel?> GetCourseAsync(string id);
        Task<bool> RegisterCourseAsync(string courseId);
    }

    public interface IPayrollService
    {
        Task<IEnumerable<PayrollModel>> GetPayrollsAsync();
        Task<PayrollModel?> GetPayrollAsync(string id);
    }

    public interface IAssetService
    {
        Task<IEnumerable<AssetModel>> GetAssetsAsync();
        Task<AssetModel?> GetAssetAsync(string id);
        Task<bool> AllocateAssetAsync(string assetId, string employeeId);
    }

    public interface IRecruitmentService
    {
        Task<IEnumerable<JobDetail>> GetJobsAsync();
        Task<JobDetail?> GetJobAsync(string id);
        Task<JobDetail?> CreateJobAsync(JobDetail job);
        Task UpdateJobAsync(string id, JobDetail job);
        Task DeleteJobAsync(string id);
    }
}
