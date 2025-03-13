#region

using System.Linq.Expressions;
using Alstom.Spectrail.ICD.Application.Contracts;
using SpectrailTestDataProvider.Domain.Common;

#endregion

namespace SpectrailTestDataProvider.Infrastructure.Repository;

public class RepositoryBase<T>(IDataProvider<T> dataProvider) : IAsyncRepository<T>
    where T : EntityBase
{
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await dataProvider.GetAllAsync();
    }

    public async Task<T> GetByIdAsync(string id)
    {
        return await dataProvider.GetByIdAsync(id);
    }

    public async Task<IEnumerable<T>> GetByFilterAsync(Expression<Func<T, bool>> filter)
    {
        return await dataProvider.GetByFilterAsync(filter);
    }

    /// <summary>
    ///     ✅ Saves a new entity in MongoDB.
    /// </summary>
    public async Task AddAsync(T entity)
    {
        await dataProvider.AddAsync(entity);
    }

    public async Task<bool> UpdateAsync(string id, T entity)
    {
        return await dataProvider.UpdateAsync(id, entity);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        return await dataProvider.DeleteAsync(id);
    }

    public async Task AddManyAsync(IEnumerable<T> entities)
    {
        await dataProvider.AddManyAsync(entities);
    }

    public async Task SeedDataAsync(IEnumerable<T> entities)
    {
        await dataProvider.AddManyAsync(entities);
    }

    public async Task<bool> DeleteAllAsync()
    {
        return await dataProvider.DeleteAllAsync();
    }
}