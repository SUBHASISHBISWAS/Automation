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
// FileName: BaseEntity.cs
// ProjectName: Alstom.Spectrail.TestFramework
// Created by SUBHASISH BISWAS On: 2025-03-13
// Updated by SUBHASISH BISWAS On: 2025-03-13
//  ******************************************************************************/

#endregion

#region

using Newtonsoft.Json;

#endregion

namespace Alstom.Spectrail.TestFramework.API.Models;

/// <summary>
///     ✅ Base class for all entities that include auditing fields.
/// </summary>
public abstract class BaseEntity
{
    [JsonProperty("createdBy")] public string CreatedBy { get; set; }

    [JsonProperty("createdDate")] public DateTime CreatedDate { get; set; }

    [JsonProperty("lastModifiedBy")] public string LastModifiedBy { get; set; }

    [JsonProperty("lastModifiedDate")] public DateTime LastModifiedDate { get; set; }
}