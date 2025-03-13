#region

using System.Linq.Expressions;
using MediatR;
using Alstom.Spectrail.ICD.Domain.Common;
using Alstom.Spectrail.Server.Common.Entities;

#endregion

namespace Alstom.Spectrail.ICD.Application.Features.ICD.Queries.Query;

public class RepositoryQuery<T>(string? id = null, Expression<Func<T, bool>>? filter = null)
    : IRequest<IEnumerable<T>>
    where T : EntityBase
{
    public string? Id { get; } = id;
    public Expression<Func<T, bool>>? Filter { get; } = filter;
}