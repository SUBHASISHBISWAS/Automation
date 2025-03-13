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
// FileName: FeatureFlagsConfig.cs
// ProjectName: Alstom.Spectrail.Server.Common
// Created by SUBHASISH BISWAS On: 2025-03-13
// Updated by SUBHASISH BISWAS On: 2025-03-13
//  ******************************************************************************/

#endregion

namespace Alstom.Spectrail.Server.Common.Configuration;

/// <summary>
///     ✅ Represents feature flags from `appsettings.json`.
/// </summary>
public class FeatureFlagsConfig
{
    public bool EnableChecksumValidation { get; init; } = false;
    public bool EnableEagerLoading { get; init; } = false;
    public bool EnableMiddlewarePreloading { get; init; } = false;
}