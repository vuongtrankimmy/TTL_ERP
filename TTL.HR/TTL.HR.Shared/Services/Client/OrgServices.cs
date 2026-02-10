using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Shared.Interfaces;
using TTL.HR.Shared.Models;
using System.Linq;

namespace TTL.HR.Shared.Services.Client
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IApiRepository<DepartmentModel> _repository;
        private const string Endpoint = "api/v1/departments";

        public DepartmentService(IApiRepository<DepartmentModel> repository)
        {
            _repository = repository;
        }

        public async Task<List<DepartmentModel>> GetDepartmentsAsync()
        {
            var result = await _repository.GetAllAsync(Endpoint);
            return result.ToList();
        }
    }

    public class PositionService : IPositionService
    {
        private readonly IApiRepository<PositionModel> _repository;
        private const string Endpoint = "api/v1/positions";

        public PositionService(IApiRepository<PositionModel> repository)
        {
            _repository = repository;
        }

        public async Task<List<PositionModel>> GetPositionsAsync(string? departmentId = null)
        {
            var url = Endpoint;
            if (!string.IsNullOrEmpty(departmentId))
            {
                url += $"?departmentId={departmentId}";
            }
            var result = await _repository.GetAllAsync(url);
            return result.ToList();
        }
    }
}
