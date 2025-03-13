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
// FileName: GetICDQueryHandler.cs
// ProjectName: Alstom.Spectrail.ICD.Application
// Created by SUBHASISH BISWAS On: 2025-03-06
// Updated by SUBHASISH BISWAS On: 2025-03-13
//  ******************************************************************************/

#endregion

/*#region

using MediatR;
using Alstom.Spectrail.ICD.Application.Contracts;
using Alstom.Spectrail.ICD.Application.Features.ICD.Queries.Query;
using Alstom.Spectrail.ICD.Domain.Common;

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

