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
// FileName: SpectrailMongoDatabaseConfig.cs
// ProjectName: Alstom.Spectrail.Server.Common
// Created by SUBHASISH BISWAS On: 2025-03-04
// Updated by SUBHASISH BISWAS On: 2025-03-27
//  ******************************************************************************/

#endregion

namespace Alstom.Spectrail.Server.Common.Configuration;

public class SpectrailMongoDatabaseConfig
{
    public string? ConnectionString { get; set; }
    public string? ICDDatabase { get; set; }
    public string? ICDEntityRegistry { get; set; }

    public string? ICDEntityMapping { get; set; }
}