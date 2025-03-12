#region

using MediatR;
using SpectrailTestDataProvider.Application.Contracts;
using SpectrailTestDataProvider.Application.Features.ICD.Commands.Command;
using SpectrailTestDataProvider.Domain.Common;

#endregion

namespace SpectrailTestDataProvider.Application.Features.ICD.Commands.Handlers;

public class RepositoryCommandHandler<T>(IAsyncRepository<T> repository) : IRequestHandler<RepositoryCommand<T>, bool>
    where T : EntityBase
{
    public async Task<bool> Handle(RepositoryCommand<T> request, CancellationToken cancellationToken)
    {
        try
        {
            var repositoryType = typeof(IAsyncRepository<T>);
            var method = repositoryType.GetMethod(request.Operation);

            if (method == null)
                throw new InvalidOperationException($"❌ Operation '{request.Operation}' not found in repository!");

            object?[] parameters = method.GetParameters().Length switch
            {
                1 when request.Entity != null => [request.Entity],
                1 when request.Entities != null => [request.Entities],
                1 when request.Id != null => [request.Id],
                _ => []
            };

            var result = method.Invoke(repository, parameters);
            switch (result)
            {
                case Task<bool> taskBool:
                    return await taskBool;
                case Task task:
                    await task;
                    break;
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error executing '{request.Operation}': {ex.Message}");
            return false;
        }
    }
}