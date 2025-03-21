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
// FileName: MediatRExtensions.cs
// ProjectName: Alstom.Spectrail.ICD.Application
// Created by SUBHASISH BISWAS On: 2025-03-21
// Updated by SUBHASISH BISWAS On: 2025-03-21
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.ICD.Application.Features.ICD.Commands.Command;
using Alstom.Spectrail.ICD.Application.Registry;
using MediatR;

#endregion

namespace Alstom.Spectrail.ICD.Application.Utility;

public static class MediatRExtensions
{
    public static async Task<bool> SendRepositoryCommandAsync(this IMediator mediator, string sheetName,
        string fileName)
    {
        var mapping = EntityRegistry.GetAllMappings()
            .FirstOrDefault(x => x.SheetName.Equals(sheetName, StringComparison.OrdinalIgnoreCase));

        if (mapping is null)
            throw new InvalidOperationException($"❌ No mapping found for sheet '{sheetName}'");

        var entityType = Type.GetType(mapping.EntityName) ??
                         AppDomain.CurrentDomain.GetAssemblies()
                             .SelectMany(a => a.GetTypes())
                             .FirstOrDefault(t => t.FullName == mapping.EntityName);

        if (entityType is null)
            throw new InvalidOperationException($"❌ Cannot resolve type for: {mapping.EntityName}");

        var commandType = typeof(RepositoryCommand);
        var command = Activator.CreateInstance(commandType)!;
        commandType.GetProperty("FileName")?.SetValue(command, fileName);

        var sendMethod = typeof(IMediator).GetMethods()
            .First(m => m.Name == "Send" && m.GetParameters().Length == 1);
        var genericSend = sendMethod.MakeGenericMethod(typeof(bool));
        var result = await (Task<bool>)genericSend.Invoke(mediator, new[] { command })!;
        return result;
    }
}