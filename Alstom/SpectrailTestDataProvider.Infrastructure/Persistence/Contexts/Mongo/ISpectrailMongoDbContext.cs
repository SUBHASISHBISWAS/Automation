#region

using MongoDB.Driver;
using SpectrailTestDataProvider.Application.Contracts;

#endregion

namespace SpectrailTestDataProvider.Infrastructure.Persistence.Contexts.Mongo;

public interface ISpectrailMongoDbContext<T> : IDataContext<T> where T : class
{
    IMongoCollection<T>? SpectrailData { get; }
}