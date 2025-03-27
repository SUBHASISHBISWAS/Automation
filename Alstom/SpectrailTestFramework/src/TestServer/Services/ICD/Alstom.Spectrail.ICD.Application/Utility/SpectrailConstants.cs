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
// FileName: SpectrailConstants.cs
// ProjectName: Alstom.Spectrail.ICD.Application
// Created by SUBHASISH BISWAS On: 2025-03-22
// Updated by SUBHASISH BISWAS On: 2025-03-28
//  ******************************************************************************/

#endregion

namespace Alstom.Spectrail.ICD.Application.Utility;

public static class SpectrailConstants
{
    public const string DynamicAssemblyName = "Alstom.Spectrail.ICD.Domain.Entities.ICD";

    public const string Settings_DynamicEntityFilters = "Settings:DynamicEntityFilters";

    public const string ICD_NetworkConfig = "network_config";

    public const string DynamicEntitiesFolder = "DynamicEntities";

    #region Redis Keys

    public const string RedisPrefix = "Spectrail";

    public const string RedisKeyFolderHash = $"{RedisPrefix}:ICD:DataFolder:LastFolderHash";

    public const string RedisFileHashKey = $"{RedisPrefix}:ICD:Files:Hash:";

    public const string RedisEquipmentHashKey = $"{RedisPrefix}:ICD:Files:Equipments:";

    public const string RedisEntityListKey = $"{RedisPrefix}:ICD:EntityRegistry:FileToEntityMapping";

    public const string RedisDynamicType = $"{RedisPrefix}:ICD:DynamicTypes:";

    public const string RedisDynamicAssemblyCreated = $"{RedisPrefix}:ICD:DynamicAssesmly:Status";

    #endregion
}