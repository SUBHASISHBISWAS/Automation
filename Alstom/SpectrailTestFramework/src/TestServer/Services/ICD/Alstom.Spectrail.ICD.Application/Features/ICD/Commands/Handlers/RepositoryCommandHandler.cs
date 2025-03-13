#region

using System.Reflection;
using Alstom.Spectrail.ICD.Application.Attributes;
using Alstom.Spectrail.ICD.Application.Contracts;
using Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Command;
using MediatR;
using SpectrailTestDataProvider.Domain.Common;

#endregion

namespace Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Handlers;

public class RepositoryCommandHandler<T>(IAsyncRepository<T> repository) : IRequestHandler<RepositoryCommand<T>, bool>
    where T : EntityBase
{
    private static readonly Dictionary<string, MethodInfo> _methodCache = typeof(IAsyncRepository<T>)
        .GetMethods()
        .ToDictionary(
            m => m.GetCustomAttribute<RepositoryOperationAttribute>()?.Name ??
                 m.Name, // ✅ Use attribute if available, otherwise fallback to method name
            m => m
        );

    public async Task<bool> Handle(RepositoryCommand<T> request, CancellationToken cancellationToken)
    {
        try
        {
            if (!_methodCache.TryGetValue(request.Operation, out var method))
                throw new InvalidOperationException($"❌ Operation '{request.Operation}' not found in repository!");
            object?[] parameters = method.GetParameters().Length switch
            {
                1 when request.Entity != null => [request.Entity],
                1 when request.Entities != null => [request.Entities],
                1 when request.Id != null => [request.Id],
                _ => []
            };

            var result = method.Invoke(repository, parameters);
            return await (result switch
            {
                Task<bool> taskBool => taskBool,
                Task task => task.ContinueWith(_ => true, cancellationToken),
                _ => Task.FromResult(true)
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error executing '{request.Operation}': {ex.Message}");
            return false;
        }
    }
}