#region

using MongoDB.Driver;
using Alstom.Spectrail.ICD.Domain.Common;

#endregion

namespace Alstom.Spectrail.ICD.Application.Contracts;

public interface ISpectrailMongoDbContext<T> : IDataContext<T> where T : EntityBase
{
    IMongoCollection<T>? SpectrailData { get; }
}