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
// FileName: IICDDbContext.cs
// ProjectName: Alstom.Spectrail.ICD.Application
// Created by SUBHASISH BISWAS On: 2025-03-16
// Updated by SUBHASISH BISWAS On: 2025-03-16
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.ICD.Application.Registry;
using MongoDB.Driver;

#endregion

namespace Alstom.Spectrail.ICD.Application.Contracts;

public interface IICDDbContext : ISpectrailMongoDbContext
{
    IMongoDatabase ICDDatabase { get; init; }

    IMongoDatabase ICDEntityRegistry { get; init; }

    IMongoCollection<EntityMapping>? ICDEntityMapping { get; }
}