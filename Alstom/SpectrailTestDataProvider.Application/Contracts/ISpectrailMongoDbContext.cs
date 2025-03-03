#region

using MongoDB.Driver;
using SpectrailTestDataProvider.Domain.Common;

#endregion

namespace SpectrailTestDataProvider.Application.Contracts;

public interface ISpectrailMongoDbContext<T> : IDataContext<T> where T : EntityBase
{
    IMongoCollection<T>? SpectrailData { get; }
}