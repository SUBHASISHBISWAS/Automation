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
// FileName: ICDConfig.cs
// ProjectName: Alstom.Spectrail.Server.Common
// Created by SUBHASISH BISWAS On: 2025-03-13
// Updated by SUBHASISH BISWAS On: 2025-03-22
//  ******************************************************************************/

#endregion

namespace Alstom.Spectrail.Server.Common.Configuration;

/// <summary>
///     ✅ Represents configuration settings for ICD processing.
/// </summary>
public class ICDConfig
{
    /// <summary>
    ///     ✅ The root folder where all ICD Excel files are stored.
    /// </summary>
    public string ICD_FOLDER_PATH { get; init; } = string.Empty;

    /// <summary>
    ///     ✅ Per-file dynamic entity filters from `appsettings.json`.
    ///     ✅ If empty, all entities will be processed.
    /// </summary>
    public Dictionary<string, List<string>> DynamicEntityFilters { get; init; } = new(StringComparer
        .OrdinalIgnoreCase);
}