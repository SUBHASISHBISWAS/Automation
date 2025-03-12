#region

using System.Linq.Expressions;
using SpectrailTestDataProvider.Domain.Common;

#endregion

namespace SpectrailTestDataProvider.Application.Contracts;

public interface IAsyncRepository<T> where T : EntityBase
{
    [RepositoryOperation("GetAll")]
    Task<IEnumerable<T>> GetAllAsync();

    [RepositoryOperation("GetById")]
    Task<T> GetByIdAsync(string id);

    [RepositoryOperation("GetByFilter")]
    Task<IEnumerable<T>> GetByFilterAsync(Expression<Func<T, bool>> filter);

    [RepositoryOperation("Add")]
    Task AddAsync(T entity);

    [RepositoryOperation("Update")]
    Task<bool> UpdateAsync(string id, T entity);

    [RepositoryOperation("Delete")]
    Task<bool> DeleteAsync(string id);

    /// <summary>
    ///     ✅ Adds multiple records efficiently in batch.
    /// </summary>
    [RepositoryOperation("Initialize")]
    Task InitializeAsync(IEnumerable<T> entities);

    /// <summary>
    ///     ✅ Deletes all records in the collection.
    /// </summary>
    [RepositoryOperation("DeleteAll")]
    Task<bool> DeleteAllAsync();
}