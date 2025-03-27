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
// FileName: MoveAndRegisterEntityCommandHandler.cs
// ProjectName: Alstom.Spectrail.ICD.Application
// Created by SUBHASISH BISWAS On: 2025-03-28
// Updated by SUBHASISH BISWAS On: 2025-03-28
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Command;
using Alstom.Spectrail.Server.Common.Configuration;
using Alstom.Spectrail.Server.Common.Contracts;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Handlers;

public class MoveAndRegisterEntityCommandHandler(IServerConfigHelper configHelper, IServiceProvider serviceProvider)
    : IRequestHandler<MoveAndRegisterEntityCommand, bool>
{
    public async Task<bool> Handle(MoveAndRegisterEntityCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var targetDir = configHelper.GetICDFolderPath();
            var sourceFile = request.SourceFilePath;
            var destFile = Path.Combine(targetDir, Path.GetFileName(sourceFile));

            if (!File.Exists(sourceFile))
            {
                Console.WriteLine($"❌ Source file not found: {sourceFile}");
                return false;
            }

            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            File.Copy(sourceFile, destFile, true);
            Console.WriteLine($"✅ Moved file to: {destFile}");

            // 🔄 Resolve and trigger EntityRegistryOrchestrator
            using var scope = serviceProvider.CreateScope();
            var orchestrator = scope.ServiceProvider.GetRequiredService<IEntityRegistryOrchestrator>();
            await orchestrator.ExecuteAsync(true);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error moving and registering file: {ex.Message}");
            return false;
        }
    }
}