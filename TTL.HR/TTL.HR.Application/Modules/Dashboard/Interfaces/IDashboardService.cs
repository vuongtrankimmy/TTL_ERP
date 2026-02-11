using System.Threading.Tasks;
using TTL.HR.Application.Modules.Dashboard.Models;

namespace TTL.HR.Application.Modules.Dashboard.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardOverviewModel> GetOverviewAsync();
    }
}
