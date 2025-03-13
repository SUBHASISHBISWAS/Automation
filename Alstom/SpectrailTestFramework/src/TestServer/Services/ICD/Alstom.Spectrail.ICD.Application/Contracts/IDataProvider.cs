#region

using System.Linq.Expressions;
using Alstom.Spectrail.ICD.Domain.Common;

#endregion

namespace Alstom.Spectrail.ICD.Application.Contracts;

public interface IDataProvider<T> where T : EntityBase
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
    Task AddManyAsync(IEnumerable<T> entities);


    /// <summary>
    ///     ✅ Deletes all records in the collection.
    /// </summary>
    Task<bool> DeleteAllAsync();
}