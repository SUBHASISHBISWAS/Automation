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
// FileName: EntityRegistrationBootstrapper.cs
// ProjectName: Alstom.Spectrail.ICD.Application
// Created by SUBHASISH BISWAS On: 2025-03-21
// Updated by SUBHASISH BISWAS On: 2025-03-21
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.ICD.Application.Registry;
using Alstom.Spectrail.ICD.Application.Utility;
using Alstom.Spectrail.Server.Common.Configuration;
using ClosedXML.Excel;
using Microsoft.Extensions.DependencyInjection;

#endregion

public static class EntityRegistrationBootstrapper
{
    public static void RegisterDynamicEntities(IServiceCollection services, IServerConfigHelper configHelper)
    {
        var icdFiles = configHelper.GetICDFiles();
        var dynamicTypes = new List<Type>();

        foreach (var file in icdFiles)
        {
            var selectedSheets = EntityRegistry.ExtractEquipmentNames(file, configHelper);

            using var workbook = new XLWorkbook(file);
            foreach (var sheet in workbook.Worksheets)
            {
                var sheetName = sheet.Name.Trim().Replace(" ", "").ToLower();

                if (selectedSheets.Any() && !selectedSheets.Contains(sheetName, StringComparer.OrdinalIgnoreCase))
                    continue;

                var entityType = EntityRegistry.GetEntityType(sheetName)
                                 ?? EntityRegistry.GenerateDynamicEntity(sheetName);

                if (entityType == null) continue;

                EntityRegistry.CacheEntityType(sheetName, entityType);
                dynamicTypes.Add(entityType);
            }
        }

        // ✅ Register repository handlers for dynamic types
        DynamicRepositoryRegistrar.RegisterRepositoryHandlers(services, dynamicTypes);
    }
}