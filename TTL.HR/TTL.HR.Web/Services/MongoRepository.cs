using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TTL.HR.Shared.Entities.Base;
using TTL.HR.Shared.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TTL.HR.Web.Data
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
    }

    public class MongoRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(IOptions<MongoDbSettings> settings)
        {
            var mongoClient = new MongoClient(settings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);
            
            // Derive collection name from class name (e.g., "Employee" -> "Employees")
            var collectionName = typeof(T).Name + "s"; 
            _collection = mongoDatabase.GetCollection<T>(collectionName);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(e => !e.IsDeleted).ToListAsync();
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            return await _collection.Find(e => e.Id == id && !e.IsDeleted).FirstOrDefaultAsync();
        }

        public async Task<T> CreateAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task<bool> UpdateAsync(string id, T entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            var result = await _collection.ReplaceOneAsync(e => e.Id == id, entity);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
             // Soft Delete
            var update = Builders<T>.Update
                .Set(e => e.IsDeleted, true)
                .Set(e => e.UpdatedAt, DateTime.UtcNow);
            
            var result = await _collection.UpdateOneAsync(e => e.Id == id, update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}
