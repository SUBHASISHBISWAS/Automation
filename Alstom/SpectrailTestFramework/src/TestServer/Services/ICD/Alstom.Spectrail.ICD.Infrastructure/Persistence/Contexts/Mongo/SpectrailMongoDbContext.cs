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
// Updated by SUBHASISH BISWAS On: 2025-03-13
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.ICD.Application.Contracts;
using Alstom.Spectrail.ICD.Application.Models;
using Alstom.Spectrail.Server.Common.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

#endregion

namespace Alstom.Spectrail.ICD.Infrastructure.Persistence.Contexts.Mongo;

public abstract class SpectrailMongoDbContext<T> : ISpectrailMongoDbContext<T> where T : EntityBase
{
    protected SpectrailMongoDbContext(IOptions<SpectrailMongoDatabaseSettings> databaseSettings)
    {
        ArgumentNullException.ThrowIfNull(databaseSettings);
        SpectrailDatabaseSettings = databaseSettings.Value;
        var client = new MongoClient(SpectrailDatabaseSettings.ConnectionString);
        var database = client.GetDatabase(SpectrailDatabaseSettings.DatabaseName);
        SpectrailData = database.GetCollection<T>(typeof(T).Name);
    }

    private SpectrailMongoDatabaseSettings SpectrailDatabaseSettings { get; }

    public IMongoCollection<T>? SpectrailData { get; set; }
}