#region

using System.Diagnostics;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using SpectrailTestDataProvider.Application.Contracts;
using SpectrailTestDataProvider.Domain.Common;

#endregion

namespace SpectrailTestDataProvider.Infrastructure.Persistence.Drivers;

public class MongoDataProvider<T>(ISpectrailMongoDbContext<T> mongoDataContext) : IDataProvider<T>
    where T : EntityBase
{
    private readonly IMongoCollection<T>? _collection = mongoDataContext.SpectrailData;

    /// <summary>
    ///     ✅ Retrieves all records from MongoDB.
    /// </summary>
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        try
        {
            Debug.Assert(_collection is not null, $"{nameof(_collection)} is null");
            return await _collection.Find(_ => true).ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error fetching all records: {ex.Message}");
            throw new InvalidOperationException("Error retrieving records from MongoDB.", ex);
        }
    }

    /// <summary>
    ///     ✅ Retrieves a record by ID.
    /// </summary>
    public async Task<T> GetByIdAsync(string id)
    {
        try
        {
            Debug.Assert(_collection is not null, $"{nameof(_collection)} is null");
            var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id)); // Use `_id`
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error fetching record with ID {id}: {ex.Message}");
            throw new InvalidOperationException($"Error retrieving record with ID {id}.", ex);
        }
    }

    /// <summary>
    ///     ✅ Retrieves records based on a filter.
    /// </summary>
    public async Task<IEnumerable<T>> GetByFilterAsync(Expression<Func<T, bool>> filter)
    {
        try
        {
            Debug.Assert(_collection is not null, $"{nameof(_collection)} is null");
            return await _collection.Find(filter).ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error fetching records with filter: {ex.Message}");
            throw new InvalidOperationException("Error retrieving records based on the provided filter.", ex);
        }
    }

    /// <summary>
    ///     ✅ Adds a new record to MongoDB.
    /// </summary>
    public async Task AddAsync(T entity)
    {
        try
        {
            Debug.Assert(_collection is not null, $"{nameof(_collection)} is null");
            await _collection.InsertOneAsync(entity);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error inserting record: {ex.Message}");
            throw new InvalidOperationException("Error adding record to MongoDB.", ex);
        }
    }

    /// <summary>
    ///     ✅ Updates a record by ID.
    /// </summary>
    public async Task<bool> UpdateAsync(string id, T entity)
    {
        try
        {
            Debug.Assert(_collection is not null, $"{nameof(_collection)} is null");

            var filter = Builders<T>.Filter.Eq("Id", id);
            var result = await _collection.ReplaceOneAsync(filter, entity);

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error updating record with ID {id}: {ex.Message}");
            throw new InvalidOperationException($"Error updating record with ID {id}.", ex);
        }
    }

    /// <summary>
    ///     ✅ Deletes a record by ID.
    /// </summary>
    public async Task<bool> DeleteAsync(string id)
    {
        try
        {
            Debug.Assert(_collection is not null, $"{nameof(_collection)} is null");

            var filter = Builders<T>.Filter.Eq("Id", id);
            var result = await _collection.DeleteOneAsync(filter);

            return result.IsAcknowledged && result.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error deleting record with ID {id}: {ex.Message}");
            throw new InvalidOperationException($"Error deleting record with ID {id}.", ex);
        }
    }

    /// <summary>
    ///     ✅ Deletes all records from the collection.
    /// </summary>
    public async Task<bool> DeleteAllAsync()
    {
        try
        {
            Debug.Assert(_collection is not null, $"{nameof(_collection)} is null");

            var result = await _collection.DeleteManyAsync(_ => true);
            return result.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error deleting all records: {ex.Message}");
            throw new InvalidOperationException("Error deleting all records from MongoDB.", ex);
        }
    }

    /// <summary>
    ///     ✅ Adds multiple records efficiently in batch.
    /// </summary>
    public async Task AddManyAsync(IEnumerable<T> entities)
    {
        try
        {
            Debug.Assert(_collection is not null, $"{nameof(_collection)} is null");
            await _collection.InsertManyAsync(entities);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error initializing collection: {ex.Message}");
            throw new InvalidOperationException("Error initializing records in MongoDB.", ex);
        }
    }
}