#region

using System.Linq.Expressions;
using SpectrailTestDataProvider.Domain.Common;

#endregion

namespace SpectrailTestDataProvider.Application.Contracts;

public interface IAsyncRepository<T> where T : EntityBase
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetByFilterAsync(Expression<Func<T, bool>> filter);
    Task AddAsync(T entity);
    Task<bool> UpdateAsync(string id, T entity);
    Task<bool> DeleteAsync(string id);

    /// <summary>
    ///     ✅ Adds multiple records efficiently in batch.
    /// </summary>
    Task InitializeAsync(IEnumerable<T> entities);

    /// <summary>
    ///     ✅ Deletes all records in the collection.
    /// </summary>
    Task<bool> DeleteAllAsync();
}