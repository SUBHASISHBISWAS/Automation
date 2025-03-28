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
// FileName: SpectrailMongoDbContext.cs
// ProjectName: Alstom.Spectrail.ICD.Infrastructure
// Created by SUBHASISH BISWAS On: 2025-03-04
// Updated by SUBHASISH BISWAS On: 2025-03-27
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.ICD.Application.Contracts;
using Alstom.Spectrail.Server.Common.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

#endregion

namespace Alstom.Spectrail.ICD.Infrastructure.Persistence.Contexts.Mongo;

public abstract class SpectrailMongoDbContext : ISpectrailMongoDbContext
{
    protected SpectrailMongoDbContext(IOptions<SpectrailMongoDatabaseConfig> databaseSettings)
    {
        ArgumentNullException.ThrowIfNull(databaseSettings);
        SpectrailMongoDbConfig = databaseSettings.Value;
        Client = new MongoClient(SpectrailMongoDbConfig.ConnectionString);
    }

    protected MongoClient Client { get; }

    protected SpectrailMongoDatabaseConfig SpectrailMongoDbConfig { get; }
}