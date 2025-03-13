#region

using Alstom.Spectrail.ICD.Application.Enums;
using Alstom.Spectrail.Server.Common.Entities;
using MediatR;

#endregion

namespace Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Command;

public class RepositoryCommand<T> : IRequest<bool> where T : EntityBase
{
    // ✅ Constructor supporting Enum
    public RepositoryCommand(RepositoryOperation operation, T? entity = null, IEnumerable<T>? entities = null,
        string? id = null)
    {
        Operation = operation.ToString();
        Entity = entity;
        Entities = entities;
        Id = id;
    }

    // ✅ Constructor supporting String for backward compatibility
    public RepositoryCommand(string operation, T? entity = null, IEnumerable<T>? entities = null, string? id = null)
    {
        Operation = operation;
        Entity = entity;
        Entities = entities;
        Id = id;
    }

    public string Operation { get; }
    public T? Entity { get; }
    public IEnumerable<T>? Entities { get; }
    public string? Id { get; }
}