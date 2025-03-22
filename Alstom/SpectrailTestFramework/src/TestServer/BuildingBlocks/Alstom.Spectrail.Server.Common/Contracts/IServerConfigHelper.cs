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
// FileName: IServerConfigHelper.cs
// ProjectName: Alstom.Spectrail.Server.Common
// Created by SUBHASISH BISWAS On: 2025-03-13
// Updated by SUBHASISH BISWAS On: 2025-03-22
//  ******************************************************************************/

#endregion

namespace Alstom.Spectrail.Server.Common.Configuration;

/// <summary>
///     ✅ Interface for `ServerConfigHelper`, useful for dependency injection and testing.
/// </summary>
public interface IServerConfigHelper
{
    /// <summary>
    ///     ✅ Retrieves the ICD folder path and ensures it's accessible.
    /// </summary>
    string GetICDFolderPath();

    /// <summary>
    ///     ✅ Retrieves a list of all ICD Excel files in the ICD folder.
    /// </summary>
    List<string> GetICDFiles();

    /// <summary>
    ///     ✅ Checks if a feature flag is enabled (Defaults to false if missing).
    /// </summary>
    bool IsFeatureEnabled(string feature);

    T? GetSetting<T>(string key);

    T GetSection<T>(string sectionKey);
}