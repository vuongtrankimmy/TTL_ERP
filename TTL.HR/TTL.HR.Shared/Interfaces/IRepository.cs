using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Shared.Entities.Base;

namespace TTL.HR.Shared.Interfaces
{
    // Generic Repository Interface (Server-side mainly)
    public interface IRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(string id);
        Task<T> CreateAsync(T entity);
        Task<bool> UpdateAsync(string id, T entity);
        Task<bool> DeleteAsync(string id);
    }
}
