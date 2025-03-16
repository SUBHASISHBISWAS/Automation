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
// Created by SUBHASISH BISWAS On: 2025-03-12
// Updated by SUBHASISH BISWAS On: 2025-03-16
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.ICD.Application.Contracts;
using Alstom.Spectrail.ICD.Application.Features.ICD.Queries.Query;
using Alstom.Spectrail.Server.Common.Entities;
using MediatR;

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