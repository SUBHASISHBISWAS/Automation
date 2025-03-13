#region

using Alstom.Spectrail.Server.Common.Entities;
using MongoDB.Driver;

#endregion

namespace Alstom.Spectrail.ICD.Application.Contracts;

public interface ISpectrailMongoDbContext<T> : IDataContext<T> where T : EntityBase
{
    IMongoCollection<T>? SpectrailData { get; }
}