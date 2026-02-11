using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TTL.HR.Application.Modules.Common.Interfaces
{
    public interface IApiRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(string endpoint);
        Task<T?> GetByIdAsync(string endpoint, string id);
        Task<T> CreateAsync(string endpoint, T entity);
        Task UpdateAsync(string endpoint, string id, T entity);
        Task DeleteAsync(string endpoint, string id);
    }

    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(string id);
        Task CreateAsync(T entity);
        Task UpdateAsync(string id, T entity);
        Task DeleteAsync(string id);
        Task<List<T>> FindAsync(Expression<Func<T, bool>> filter);
    }

    public interface IMasterDataService
    {
        Task<List<TTL.HR.Application.Modules.Common.Models.LookupModel>> GetLookupsAsync(string type);
        Task<List<TTL.HR.Application.Modules.Common.Models.LookupModel>> GetCachedLookupsAsync(string type);
    }

    public interface IAuthService
    {
        Task<TTL.HR.Application.Modules.Common.Models.ApiResponse<TTL.HR.Application.Modules.Common.Models.AuthResponse>> LoginAsync(TTL.HR.Application.Modules.Common.Models.LoginRequest request);
        Task<TTL.HR.Application.Modules.Common.Models.ApiResponse<bool>> RegisterAsync(TTL.HR.Application.Modules.Common.Models.RegisterRequest request);
        Task<TTL.HR.Application.Modules.Common.Models.ApiResponse<bool>> ChangePasswordAsync(string currentPassword, string newPassword);
        Task LogoutAsync();
        Task<TTL.HR.Application.Modules.Common.Models.UserDto?> GetCurrentUserAsync();
    }
}
