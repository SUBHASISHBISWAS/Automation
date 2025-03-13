#region

using Alstom.Spectrail.ICD.Domain.Common;
using Alstom.Spectrail.Server.Common.Entities;

#endregion

namespace Alstom.Spectrail.ICD.Application.Contracts;

public interface IICDRepository<T> : IAsyncRepository<T> where T : EntityBase
{
}