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
// FileName: RepositoryCommand.cs
// ProjectName: Alstom.Spectrail.ICD.Application
// Created by SUBHASISH BISWAS On: 2025-03-12
// Updated by SUBHASISH BISWAS On: 2025-03-28
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.ICD.Application.Enums;
using Alstom.Spectrail.Server.Common.Entities;
using MediatR;

#endregion

namespace Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Command;

public class RepositoryCommand(
    RepositoryOperation operation,
    IEnumerable<EntityBase>? entities = null,
    string? fileName = null)
    : IRequest<bool>
{
    public string Operation { get; } = operation.ToString();
    public IEnumerable<EntityBase>? Entities { get; } = entities;

    public string? FileName { get; set; } = fileName;
}