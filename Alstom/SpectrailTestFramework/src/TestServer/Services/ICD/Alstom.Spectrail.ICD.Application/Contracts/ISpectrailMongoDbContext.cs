#region

using MongoDB.Driver;
using Alstom.Spectrail.ICD.Domain.Common;
using Alstom.Spectrail.Server.Common.Entities;

#endregion

namespace Alstom.Spectrail.ICD.Application.Contracts;

public interface ISpectrailMongoDbContext<T> : IDataContext<T> where T : EntityBase
{
    IMongoCollection<T>? SpectrailData { get; }
}