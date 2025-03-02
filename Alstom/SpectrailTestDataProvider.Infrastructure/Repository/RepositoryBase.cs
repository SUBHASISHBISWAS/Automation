#region

using System.Linq.Expressions;
using SpectrailTestDataProvider.Application.Contracts;
using SpectrailTestDataProvider.Domain.Common;

#endregion

namespace SpectrailTestDataProvider.Infrastructure.Repository;

public class RepositoryBase<T>(IDataProvider<T> dataProvider) : IAsyncRepository<T>
    where T : EntityBase
{
    public Task<IEnumerable<T>> GetAllAsync()
    {
        return dataProvider.GetAllAsync();
    }

    public Task<T> GetByIdAsync(string id)
    {
        return dataProvider.GetByIdAsync(id);
    }

    public Task<IEnumerable<T>> GetByFilterAsync(Expression<Func<T, bool>> filter)
    {
        return dataProvider.GetByFilterAsync(filter);
    }

    public Task AddAsync(T entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(string id, T entity)
    {
        return dataProvider.UpdateAsync(id, entity);
    }

    public Task<bool> DeleteAsync(string id)
    {
        return dataProvider.DeleteAsync(id);
    }
}