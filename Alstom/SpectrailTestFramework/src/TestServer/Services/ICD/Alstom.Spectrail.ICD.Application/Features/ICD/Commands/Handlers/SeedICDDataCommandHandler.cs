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
// FileName: SeedICDDataCommandHandler.cs
// ProjectName: Alstom.Spectrail.ICD.Application
// Created by SUBHASISH BISWAS On: 2025-03-12
// Updated by SUBHASISH BISWAS On: 2025-03-26
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.ICD.Application.Contracts;
using Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Command;
using MediatR;

#endregion

namespace Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Handlers;

/// <summary>
///     ✅ Command Handler for Seeding ICD Data
/// </summary>
public class SeedICDDataCommandHandler(IExcelService icdExcelService)
    : IRequestHandler<SeedICDDataCommand, bool>
{
    public async Task<bool> Handle(SeedICDDataCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await icdExcelService.InitializeAsync(request.ICDFiles);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error seeding ICD Data: {ex.Message}");
            return false;
        }
    }
}