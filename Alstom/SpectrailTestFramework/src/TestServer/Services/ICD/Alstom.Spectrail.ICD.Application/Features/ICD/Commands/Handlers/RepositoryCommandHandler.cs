#region ©COPYRIGHT

// /*******************************************************************************
//  *   © COPYRIGHT ALSTOM SA 2025 - ALL RIGHTS RESERVED
//  *
//  *  This file is part of the Alstom Spectrail project.
//  *  Unauthorized copying, modification, distribution, or use of this file,
//  *  via any medium, is strictly prohibited.
//  *
//  *  Proprietary and confidential.
//  *  Created and maintained by Alstom for internal use in the Spectrail project.
//  ******************************************************************************/
// 
//  /*******************************************************************************
// AuthorName: SUBHASISH BISWAS
// Email: subhasish.biswas@alstomgroup.com
// FileName: RepositoryCommandHandler.cs
// ProjectName: Alstom.Spectrail.ICD.Application
// Created by SUBHASISH BISWAS On: 2025-03-12
// Updated by SUBHASISH BISWAS On: 2025-03-28
//  ******************************************************************************/

#endregion

#region

using System.Reflection;
using Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Command;
using Alstom.Spectrail.Server.Common.Attributes;
using Alstom.Spectrail.Server.Common.Contracts;
using MediatR;

#endregion

namespace Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Handlers;

public class RepositoryCommandHandler(IAsyncRepository repository, IDynamicEntityLoader dynamicEntityLoader) :
    IRequestHandler<RepositoryCommand, bool>

{
    private static readonly Dictionary<string, MethodInfo> _methodCache = typeof(IAsyncRepository)
        .GetMethods()
        .ToDictionary(
            m => m.GetCustomAttribute<RepositoryOperationAttribute>()?.Name ??
                 m.Name, // ✅ Use attribute if available, otherwise fallback to method name
            m => m
        );

    public async Task<bool> Handle(RepositoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!_methodCache.TryGetValue(request.Operation, out var method))
                throw new InvalidOperationException($"❌ Operation '{request.Operation}' not found in repository!");

            object?[] parameters = method.GetParameters().Length switch
            {
                1 when request.Entities != null => [request.Entities],
                1 when request.FileName != null => [request.FileName],
                _ => []
            };

            var result = method.Invoke(repository, parameters);

            var operationResult = await (result switch
            {
                Task<bool> taskBool => taskBool,
                Task task => task.ContinueWith(_ => true, cancellationToken),
                _ => Task.FromResult(true)
            });

            // ✅ If delete or delete all, clear cache accordingly
            if (!operationResult || request.Operation is not ("Delete" or "DeleteAll")) return operationResult;
            if (dynamicEntityLoader is not { } loader) return operationResult;
            if (request.Operation.Equals("DeleteAll", StringComparison.OrdinalIgnoreCase))
                await loader.ClearEntityCacheAsync(deleteFolder: true);
            else if (!string.IsNullOrWhiteSpace(request.FileName))
                await loader.ClearEntityCacheAsync([request.FileName]);

            return operationResult;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error executing '{request.Operation}': {ex.Message}");
            return false;
        }
    }
}