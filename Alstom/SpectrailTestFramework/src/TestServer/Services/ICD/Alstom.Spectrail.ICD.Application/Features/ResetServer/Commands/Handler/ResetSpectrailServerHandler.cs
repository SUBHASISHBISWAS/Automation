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
// FileName: ResetSpectrailServerHandler.cs
// ProjectName: Alstom.Spectrail.ICD.Application
// Created by SUBHASISH BISWAS On: 2025-03-27
// Updated by SUBHASISH BISWAS On: 2025-03-28
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.ICD.Application.Features.ResetServer.Commands.Command;
using Alstom.Spectrail.ICD.Application.Utility;
using Alstom.Spectrail.Server.Common.Configuration;
using Alstom.Spectrail.Server.Common.ServerUtility;
using MediatR;

#endregion

namespace Alstom.Spectrail.ICD.Application.Features.ResetServer.Commands.Handler;

public class ResetSpectrailServerCommandHandler(IServerConfigHelper configHelper)
    : IRequestHandler<ResetSpectrailServerCommand, bool>
{
    public async Task<bool> Handle(ResetSpectrailServerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await SpectrailServerUtility.ResetSpectrailServerAsync(
                configHelper.SpectrailMongoConfig.ConnectionString,
                [
                    configHelper.SpectrailMongoConfig.ICDDatabase ?? throw new InvalidOperationException(),
                    configHelper.SpectrailMongoConfig.ICDEntityRegistry ?? throw new InvalidOperationException()
                ],
                configHelper.SpectrailRedisConfig.ConnectionString ?? throw new InvalidOperationException(),
                $"{SpectrailConstants.RedisPrefix}:",
                $"{SpectrailConstants.DynamicEntitiesFolder}"
            );

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ResetSpectrailServerCommand failed: {ex.Message}");
            return false;
        }
    }
}