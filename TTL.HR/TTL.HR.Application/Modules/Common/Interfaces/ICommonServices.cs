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
        Task<List<TTL.HR.Application.Modules.Common.Models.LookupModel>> GetLookupsAsync(string type, string? lang = null);
        Task<List<TTL.HR.Application.Modules.Common.Models.LookupModel>> GetCachedLookupsAsync(string type, string? lang = null);
    }

    public interface IAuthService
    {
        Task<TTL.HR.Application.Modules.Common.Models.ApiResponse<TTL.HR.Application.Modules.Common.Models.AuthResponse>> LoginAsync(TTL.HR.Application.Modules.Common.Models.LoginRequest request);
        Task<TTL.HR.Application.Modules.Common.Models.ApiResponse<bool>> RegisterAsync(TTL.HR.Application.Modules.Common.Models.RegisterRequest request);
        Task<TTL.HR.Application.Modules.Common.Models.ApiResponse<bool>> ChangePasswordAsync(string currentPassword, string newPassword);
        Task LogoutAsync();
        Task<TTL.HR.Application.Modules.Common.Models.UserDto?> GetCurrentUserAsync();
        Task<TTL.HR.Application.Modules.Common.Models.ApiResponse<bool>> ConfirmEmailAsync(string token);
        Task<TTL.HR.Application.Modules.Common.Models.ApiResponse<bool>> RequestPasswordResetAsync(string email);
        Task<TTL.HR.Application.Modules.Common.Models.ApiResponse<bool>> ResetPasswordAsync(string token, string newPassword);
        Task InitializeAsync();
    }

    public interface IFormatService
    {
        string FormatCurrency(decimal amount);
        string FormatDate(DateTime? date);
        string FormatTime(DateTime? date);
        string FormatDateTime(DateTime? date);
        string FormatNumber(decimal value, int decimals = 0);
        string FormatNumber(double value, int decimals = 0);
        string FormatNumber(int value, int decimals = 0);


        string FormatPercent(double value);
        DateTime? ToLocalTime(DateTime? utcDate);

        // Advanced String Resource Formatting
        string FormatFullName(string? name);
        string FormatIdCard(string? idCard);
        string FormatEmail(string? email);
        string FormatPhone(string? phone);
        string FormatAddress(string? address);
        bool IsValidEmail(string? email);
        string CleanDigits(string? input);
        string NormalizeUsername(string? input);
        string NormalizePassword(string? input);
        string GenerateDefaultUsername(string? phone, string? email, string? idCard);
    }

    public interface IAuditService
    {
        Task<List<TTL.HR.Application.Modules.Common.Models.AuditLogModel>> GetMyAuditLogsAsync();
        Task<TTL.HR.Application.Modules.Common.Models.PagedResult<TTL.HR.Application.Modules.Common.Models.AuditLogModel>> GetPagedAuditLogsAsync(int page, int pageSize, string? userId = null, string? action = null, string? entityName = null);
    }
}


