using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Shared.Models;

namespace TTL.HR.Shared.Interfaces
{
    public interface IDepartmentService
    {
        Task<List<DepartmentModel>> GetDepartmentsAsync();
    }

    public interface IPositionService
    {
        Task<List<PositionModel>> GetPositionsAsync(string? departmentId = null);
    }
}
