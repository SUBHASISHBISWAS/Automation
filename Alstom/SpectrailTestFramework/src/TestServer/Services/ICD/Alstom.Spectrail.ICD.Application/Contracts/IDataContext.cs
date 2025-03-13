#region

using SpectrailTestDataProvider.Domain.Common;

#endregion

namespace Alstom.Spectrail.ICD.Application.Contracts;

public interface IDataContext<in T> where T : EntityBase
{
}