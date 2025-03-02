#region

using System.Linq.Expressions;
using MongoDB.Driver;
using SpectrailTestDataProvider.Application.Contracts;
using SpectrailTestDataProvider.Domain.Common;
using SpectrailTestDataProvider.Infrastructure.Persistence.Contexts.Mongo;

#endregion

namespace SpectrailTestDataProvider.Infrastructure.Persistence.Drivers;

public abstract class MongoDataProvider<T>(ISpectrailMongoDbContext<T> mongoDataContext) : IDataProvider<T>
    where T : EntityBase
{
    private readonly IMongoCollection<T>? _collection = mongoDataContext.SpectrailData;

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection!.Find(_ => true).ToListAsync();
    }

    public async Task<T> GetByIdAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        return await _collection!.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetByFilterAsync(Expression<Func<T, bool>> filter)
    {
        return await _collection!.Find(filter).ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _collection!.InsertOneAsync(entity);
    }

    public async Task<bool> UpdateAsync(string id, T entity)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        var result = await _collection!.ReplaceOneAsync(filter, entity);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        var result = await _collection!.DeleteOneAsync(filter);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }
}