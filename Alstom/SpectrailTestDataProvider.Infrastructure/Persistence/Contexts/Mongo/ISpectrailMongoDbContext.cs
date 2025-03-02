#region

using MongoDB.Driver;
using SpectrailTestDataProvider.Application.Contracts;
using SpectrailTestDataProvider.Domain.Common;

#endregion

namespace SpectrailTestDataProvider.Infrastructure.Persistence.Contexts.Mongo;

public interface ISpectrailMongoDbContext<T> : IDataContext<T> where T : EntityBase
{
    IMongoCollection<T>? SpectrailData { get; }
}