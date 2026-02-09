using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TTL.HR.Shared.Entities.HumanResource;
using TTL.HR.Shared.Interfaces;

namespace TTL.HR.Shared.Services.Client
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IApiRepository<Employee> _repository;
        private const string Endpoint = "api/employee";

        public EmployeeService(IApiRepository<Employee> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync()
        {
            return await _repository.GetAllAsync(Endpoint);
        }

        public async Task<Employee?> GetEmployeeAsync(string id)
        {
            return await _repository.GetByIdAsync(Endpoint, id);
        }

        public async Task<Employee?> CreateEmployeeAsync(Employee employee)
        {
            return await _repository.CreateAsync(Endpoint, employee);
        }

        public async Task UpdateEmployeeAsync(string id, Employee employee)
        {
            await _repository.UpdateAsync(Endpoint, id, employee);
        }

        public async Task DeleteEmployeeAsync(string id)
        {
            await _repository.DeleteAsync(Endpoint, id);
        }
    }
}
