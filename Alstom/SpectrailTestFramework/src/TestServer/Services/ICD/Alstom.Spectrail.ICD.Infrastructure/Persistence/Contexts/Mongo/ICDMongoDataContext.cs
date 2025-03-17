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
// FileName: ICDMongoDataContext.cs
// ProjectName: Alstom.Spectrail.ICD.Infrastructure
// Created by SUBHASISH BISWAS On: 2025-03-04
// Updated by SUBHASISH BISWAS On: 2025-03-17
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.ICD.Application.Contracts;
using Alstom.Spectrail.ICD.Application.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using EntityMapping = Alstom.Spectrail.ICD.Application.Models.EntityMapping;

#endregion

namespace Alstom.Spectrail.ICD.Infrastructure.Persistence.Contexts.Mongo;

public class ICDMongoDataContext
    : SpectrailMongoDbContext, IICDDbContext
{
    public ICDMongoDataContext(IOptions<SpectrailMongoDatabaseSettings> databaseSettings) : base(databaseSettings)
    {
        // Connect to the registry database
        ICDEntityRegistry = Client.GetDatabase(SpectrailDatabaseSettings.ICDEntityRegistry);
        ICDEntityMapping =
            ICDEntityRegistry.GetCollection<EntityMapping>(SpectrailDatabaseSettings
                .ICDEntityMapping);

        ICDDatabase = Client.GetDatabase(SpectrailDatabaseSettings.ICDDatabase);
    }

    public IMongoDatabase ICDDatabase { get; init; }
    public IMongoDatabase ICDEntityRegistry { get; init; }
    public IMongoCollection<EntityMapping>? ICDEntityMapping { get; }

    /// <summary>
    ///     ✅ Ensures collections exist for registered entities.
    /// </summary>
    public void EnsureCollectionsExist()
    {
        if (ICDEntityMapping == null)
        {
            Console.WriteLine("⚠️ Entity Mapping Collection is null. Skipping collection initialization.");
            return;
        }

        var existingCollections = ICDDatabase.ListCollectionNames().ToList();
        var entityMappings = ICDEntityMapping.Find(_ => true).ToList();

        foreach (var mapping in entityMappings)
        {
            var collectionName = mapping.EntityName
                .Split('.').Last() // Extract "DCUEntity" from "Alstom.Spectrail.ICD.Domain.Entities.ICD.DCUEntity"
                .Replace("Entity", "") // Normalize naming convention
                .ToLower();

            if (!existingCollections.Contains(collectionName))
            {
                ICDDatabase.CreateCollection(collectionName);
                Console.WriteLine($"✅ Created MongoDB collection: {collectionName}");
            }
            else
            {
                Console.WriteLine($"🔍 Collection '{collectionName}' already exists.");
            }
        }
    }
}