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
// FileName: GetEntitiesByFileCommandHandler.cs
// ProjectName: Alstom.Spectrail.ICD.Application
// Created by SUBHASISH BISWAS On: 2025-03-28
// Updated by SUBHASISH BISWAS On: 2025-03-28
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Command;
using Alstom.Spectrail.Server.Common.Contracts;
using Alstom.Spectrail.Server.Common.Entities;
using MediatR;

#endregion

namespace Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Handlers;

public class
    GetEntitiesByFileCommandHandler(IAsyncRepository repository)
    : IRequestHandler<GetEntitiesByFileCommand, Dictionary<string, List<EntityBase>>>
{
    private readonly IAsyncRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<Dictionary<string, List<EntityBase>>> Handle(GetEntitiesByFileCommand request,
        CancellationToken cancellationToken)
    {
        var fileName = request.FileName;

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("❌ File name is required.", nameof(request.FileName));

        return await repository.GetEntitiesByFileAsync(fileName);
    }
}