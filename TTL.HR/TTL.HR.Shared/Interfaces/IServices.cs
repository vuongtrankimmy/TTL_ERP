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
        Task<IEnumerable<Employee>> GetEmployeesAsync();
        Task<Employee?> GetEmployeeAsync(string id);
        Task<Employee?> CreateEmployeeAsync(Employee employee);
        Task UpdateEmployeeAsync(string id, Employee employee);
        Task DeleteEmployeeAsync(string id);
    }

    public interface IRecruitmentService
    {
        Task<IEnumerable<JobPosting>> GetJobsAsync();
        Task<JobPosting?> GetJobAsync(string id);
        Task<JobPosting?> CreateJobAsync(JobPosting job);
        Task UpdateJobAsync(string id, JobPosting job);
    }

    public interface IContractService
    {
        Task<IEnumerable<ContractTemplate>> GetTemplatesAsync();
        Task<ContractTemplate> CreateTemplateAsync(ContractTemplate template);
        // Add more methods as needed
    }
}
