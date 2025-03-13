/*#region

using MediatR;
using Alstom.Spectrail.ICD.Application.Contracts;
using Alstom.Spectrail.ICD.Application.Features.ICD.Queries.Query;
using SpectrailTestDataProvider.Domain.Common;

#endregion

namespace Alstom.Spectrail.ICD.Application.Features.ICD.Queries.Handler;

public class GetRepositoryQueryHandler<T>(IAsyncRepository<T> repository)
    : IRequestHandler<GetRepositoryQuery<T>, IAsyncRepository<T>>
    where T : EntityBase
{
    private readonly IAsyncRepository<T>
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));


    public Task<IAsyncRepository<T>> Handle(GetRepositoryQuery<T> request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_repository);
    }
}*/

