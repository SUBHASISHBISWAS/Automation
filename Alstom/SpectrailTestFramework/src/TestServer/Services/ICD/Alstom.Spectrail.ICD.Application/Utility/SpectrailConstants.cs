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
// Updated by SUBHASISH BISWAS On: 2025-03-22
//  ******************************************************************************/

#endregion

namespace Alstom.Spectrail.ICD.Application.Utility;

public static class SpectrailConstants
{
    public const string DynamicAssemblyName = "Alstom.Spectrail.ICD.Domain.Entities.ICD";

    public const string Settings_DynamicEntityFilters = "Settings:DynamicEntityFilters";

    public const string ICD_NetworkConfig = "network_config";

    #region Redis Keys

    public const string RedisKeyFolderHash = "Spectrail:ICD:DataFolder:LastFolderHash";

    public const string RedisFileHashKey = "Spectrail:ICD:Files:Hash:";

    public const string RedisEquipmentHashKey = "Spectrail:ICD:Files:Equipments:";

    public const string RedisEntityListKey = "Spectrail:ICD:EntityRegistry:FileToEntityMapping";

    public const string RedisDynamicType = "Spectrail:ICD:DynamicTypes:";

    #endregion
}