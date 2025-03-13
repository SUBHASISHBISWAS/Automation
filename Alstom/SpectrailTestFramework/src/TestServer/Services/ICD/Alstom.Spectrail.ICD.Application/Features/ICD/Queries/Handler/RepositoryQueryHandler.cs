#region

using Alstom.Spectrail.ICD.Application.Contracts;
using Alstom.Spectrail.ICD.Application.Features.ICD.Queries.Query;
using MediatR;
using Alstom.Spectrail.ICD.Domain.Common;

#endregion

namespace Alstom.Spectrail.ICD.Application.Features.ICD.Queries.Handler;

public class RepositoryQueryHandler<T>(IAsyncRepository<T> repository)
    : IRequestHandler<RepositoryQuery<T>, IEnumerable<T>>
    where T : EntityBase
{
    public async Task<IEnumerable<T>> Handle(RepositoryQuery<T> request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(request.Id))
        {
            var entity = await repository.GetByIdAsync(request.Id);
            return new List<T> { entity };
        }

        if (request.Filter != null) return await repository.GetByFilterAsync(request.Filter);

        return await repository.GetAllAsync();
    }
}