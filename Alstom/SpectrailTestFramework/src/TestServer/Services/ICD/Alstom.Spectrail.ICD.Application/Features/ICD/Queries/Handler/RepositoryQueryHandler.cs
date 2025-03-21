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
// FileName: RepositoryQueryHandler.cs
// ProjectName: Alstom.Spectrail.ICD.Application
// Created by SUBHASISH BISWAS On: 2025-03-21
// Updated by SUBHASISH BISWAS On: 2025-03-22
//  ******************************************************************************/

#endregion

#region

using System.Reflection;
using Alstom.Spectrail.ICD.Application.Features.ICD.Queries.Query;
using Alstom.Spectrail.ICD.Application.Registry;
using Alstom.Spectrail.Server.Common.Contracts;
using Alstom.Spectrail.Server.Common.Entities;
using MediatR;

#endregion

namespace Alstom.Spectrail.ICD.Application.Features.ICD.Queries.Handler;

public class RepositoryQueryHandler(
    IServiceProvider serviceProvider)
    : IRequestHandler<RepositoryQuery, IEnumerable<EntityBase>>
{
    public async Task<IEnumerable<EntityBase>> Handle(RepositoryQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.SheetName))
            throw new InvalidOperationException("EntityName is required for dynamic query resolution.");

        // 🧠 Resolve dynamic entity type from registry
        var entityType = EntityRegistry.GetEntityType(request.SheetName);
        if (entityType is null)
            throw new InvalidOperationException($"❌ Unable to resolve type for '{request.SheetName}'");

        // 🧪 Get repository of type IAsyncRepository<entityType>
        var repoInterfaceType = typeof(IAsyncRepository);
        var repository = serviceProvider.GetService(repoInterfaceType);
        if (repository is null)
            throw new InvalidOperationException($"❌ Repository not registered for type '{entityType.Name}'");

        // 🛠 Get methods
        MethodInfo method;
        object? result;

        if (!string.IsNullOrEmpty(request.Id))
        {
            method = repoInterfaceType.GetMethod("GetByIdAsync")!;
            result = await (dynamic)method.Invoke(repository, [request.Id])!;
            return new List<EntityBase> { (EntityBase)result! };
        }

        if (request.Filter != null)
            throw new NotSupportedException("⚠️ Filters not supported in dynamic query without generics.");

        method = repoInterfaceType.GetMethod("GetAllAsync", new[] { typeof(string), typeof(string) })!;
        result = await (dynamic)method.Invoke(repository, new object?[] { request.FileName, request.SheetName })!;
        return ((IEnumerable<object>)result!).Cast<EntityBase>();
    }
}