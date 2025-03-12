#region

using MediatR;
using SpectrailTestDataProvider.Domain.Common;

#endregion

namespace SpectrailTestDataProvider.Application.Features.ICD.Commands.Command;

public class RepositoryCommand<T>(
    string operation,
    T? entity = null,
    IEnumerable<T>? entities = null,
    string? id = null)
    : IRequest<bool>
    where T : EntityBase
{
    public string Operation { get; } = operation;
    public string? Id { get; } = id;
    public T? Entity { get; } = entity;
    public IEnumerable<T>? Entities { get; } = entities;
}