#region

using MediatR;
using SpectrailTestDataProvider.Application.Contracts;
using SpectrailTestDataProvider.Domain.Common;

#endregion

namespace SpectrailTestDataProvider.Application.Features.ICD.Queries.Query;

public class GetRepositoryQuery<T> : IRequest<IAsyncRepository<T>> where T : EntityBase
{
}