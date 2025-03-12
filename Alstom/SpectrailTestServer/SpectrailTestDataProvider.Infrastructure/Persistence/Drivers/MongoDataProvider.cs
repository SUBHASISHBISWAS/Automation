#region

using System.Diagnostics;
using System.Linq.Expressions;
using MongoDB.Driver;
using SpectrailTestDataProvider.Application.Contracts;
using SpectrailTestDataProvider.Domain.Common;

#endregion

namespace SpectrailTestDataProvider.Infrastructure.Persistence.Drivers;

public class MongoDataProvider<T>(ISpectrailMongoDbContext<T> mongoDataContext) : IDataProvider<T>
    where T : EntityBase
{
    private readonly IMongoCollection<T>? _collection = mongoDataContext.SpectrailData;

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        Debug.Assert(_collection != null, nameof(_collection) + " != null");
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<T> GetByIdAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        Debug.Assert(_collection != null, nameof(_collection) + " != null");
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetByFilterAsync(Expression<Func<T, bool>> filter)
    {
        Debug.Assert(_collection != null, nameof(_collection) + " != null");
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        try
        {
            Debug.Assert(_collection != null, nameof(_collection) + " != null");
            await _collection!.InsertOneAsync(entity);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(string id, T entity)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        Debug.Assert(_collection != null, nameof(_collection) + " != null");
        var result = await _collection.ReplaceOneAsync(filter, entity);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        Debug.Assert(_collection != null, nameof(_collection) + " != null");
        var result = await _collection.DeleteOneAsync(filter);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }

    /// <summary>
    ///     ✅ Deletes all records in the collection.
    /// </summary>
    public async Task<bool> DeleteAllAsync()
    {
        Debug.Assert(_collection != null, nameof(_collection) + " != null");
        var result = await _collection.DeleteManyAsync(_ => true);
        return result.DeletedCount > 0;
    }

    /// <summary>
    ///     ✅ Adds multiple records efficiently in batch.
    /// </summary>
    public async Task InitializeAsync(IEnumerable<T> entities)
    {
        Debug.Assert(_collection != null, nameof(_collection) + " != null");
        await _collection.InsertManyAsync(entities);
    }
}