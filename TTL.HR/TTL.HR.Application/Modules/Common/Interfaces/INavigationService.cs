using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Application.Modules.Common.Interfaces
{
    public interface INavigationService
    {
        Task<List<NavItem>> GetMenuItemsAsync();
        Task<bool> UserHasPermissionAsync(string? permission);
    }
}
