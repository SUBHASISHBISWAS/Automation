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
// FileName: MongoDataProvider.cs
// ProjectName: Alstom.Spectrail.ICD.Infrastructure
// Created by SUBHASISH BISWAS On: 2025-03-20
// Updated by SUBHASISH BISWAS On: 2025-03-28
//  ******************************************************************************/

#endregion

#region

using Alstom.Spectrail.ICD.Application.Contracts;
using Alstom.Spectrail.ICD.Application.Registry;
using Alstom.Spectrail.Server.Common.Contracts;
using Alstom.Spectrail.Server.Common.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

#endregion

namespace Alstom.Spectrail.ICD.Infrastructure.Persistence.Drivers;

public class MongoDataProvider(IICDDbContext icdDataContext) : IDataProvider

{
    private readonly IMongoDatabase _icdDatabase = icdDataContext.ICDDatabase;

    /// <summary>
    ///     ✅ Deletes a record by ID.
    /// </summary>
    public async Task<bool> DeleteAsync(string fileName)
    {
        try
        {
            var collectionName = Path.GetFileNameWithoutExtension(fileName)?.Replace(" ", "_").ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(collectionName))
            {
                Console.WriteLine("⚠️ Invalid file name provided. Cannot resolve collection.");
                return false;
            }

            await _icdDatabase.DropCollectionAsync(collectionName);
            Console.WriteLine($"🗑 Dropped collection: {collectionName}");

            return true;
        }
        catch (MongoCommandException ex) when (ex.CodeName == "NamespaceNotFound")
        {
            Console.WriteLine($"⚠️ Collection not found: {fileName}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error dropping collection for file '{fileName}': {ex.Message}");
            throw new InvalidOperationException($"Failed to drop MongoDB collection for file: {fileName}", ex);
        }
    }

    /// <summary>
    ///     ✅ Deletes all records from the collection.
    /// </summary>
    public async Task<bool> DeleteAllAsync()
    {
        try
        {
            var collectionNames = await (await _icdDatabase.ListCollectionNamesAsync()).ToListAsync();
            if (!collectionNames.Any())
            {
                Console.WriteLine("⚠️ No collections found to delete.");
                return false;
            }

            foreach (var name in collectionNames)
                try
                {
                    await _icdDatabase.DropCollectionAsync(name);
                    Console.WriteLine($"🗑 Dropped collection: {name}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Failed to drop collection '{name}': {ex.Message}");
                }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error dropping all collections: {ex.Message}");
            throw new InvalidOperationException("Error dropping all MongoDB collections.", ex);
        }
    }

    public async Task<IEnumerable<EntityBase>> GetEntityAsync(string fileName, string sheetName)
    {
        var allEntities = new List<EntityBase>();

        try
        {
            Console.WriteLine("📌 Fetching Registered Entities from EntityRegistry...");

            var registeredMappings = await EntityRegistry.GetRegisteredEquipmentMappingsByFile(fileName);
            var targetMapping = registeredMappings.FirstOrDefault(x =>
                x.SheetName.Equals(sheetName, StringComparison.OrdinalIgnoreCase));

            if (targetMapping == null)
            {
                Console.WriteLine($"⚠️ No registered mapping found for sheet '{sheetName}' in file '{fileName}'");
                return allEntities;
            }

            var entityType = EntityRegistry.GetEntityType(sheetName, fileName);
            if (entityType == null)
            {
                Console.WriteLine($"❌ Failed to resolve entity type for sheet '{sheetName}'");
                return allEntities;
            }

            Console.WriteLine($"✅ Resolved entity type: {entityType.FullName}");
            var collectionName = Path.GetFileNameWithoutExtension(fileName).ToLowerInvariant().Trim();
            var collection = _icdDatabase.GetCollection<BsonDocument>(collectionName);
            var pascalName = char.ToUpper(sheetName[0]) + sheetName[1..].ToLowerInvariant();
            var entityKey = $"{pascalName}Entity";
            var documents = await collection.Find(Builders<BsonDocument>.Filter.Eq("_t", $"{entityKey}")).ToListAsync();
            foreach (var doc in documents)
                try
                {
                    var entity = (EntityBase)BsonSerializer.Deserialize(doc, entityType);
                    allEntities.Add(entity);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Deserialization failed for document: {ex.Message}");
                }

            Console.WriteLine(
                $"✅ Retrieved {allEntities.Count} entities from '{collectionName}' for sheet '{sheetName}'");
            return allEntities;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error retrieving entities: {ex.Message}");
            throw new InvalidOperationException("An unexpected error occurred while retrieving entities.", ex);
        }
    }

    /// <summary>
    ///     ✅ Adds multiple records efficiently in batch.
    /// </summary>
    /// <summary>
    ///     ✅ Adds multiple records efficiently in batch.
    /// </summary>
    public async Task AddManyAsync(IEnumerable<EntityBase> entities)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entities);

            // ✅ Group by FileKey (FileName_SheetName)
            var groupedEntities = entities.GroupBy(e => e.FileKey);

            foreach (var group in groupedEntities)
            {
                var collection = GetCollection(group.Key);
                Console.WriteLine($"📌 Storing {group.Count()} records in collection '{group.Key}'");

                // Convert to BsonDocument and insert
                var bsonEntities = group.Select(e => e.ToBsonDocument()).ToList();
                await collection.InsertManyAsync(bsonEntities);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error inserting records: {ex.Message}");
            throw new InvalidOperationException("Error inserting records into MongoDB.", ex);
        }
    }

    /// <summary>
    ///     ✅ Seeds data dynamically based on `FileKey`
    /// </summary>
    public async Task SeedDataAsync(IEnumerable<EntityBase> entities)
    {
        try
        {
            if (entities == null || !entities.Any())
                throw new ArgumentNullException(nameof(entities), "⚠️ No entities to insert.");

            // ✅ Group entities by runtime type and FileName
            var groupedEntities = entities
                .GroupBy(e => (
                    EntityType: e.GetType(),
                    CollectionName: Path.GetFileNameWithoutExtension(e.FileName)?.Replace(" ", "_").ToLowerInvariant()
                ));

            foreach (var group in groupedEntities)
            {
                var (entityType, collectionName) = group.Key;

                if (string.IsNullOrWhiteSpace(collectionName))
                {
                    Console.WriteLine($"⚠️ Skipping invalid collection name for type: {entityType.Name}");
                    continue;
                }

                var collection = _icdDatabase.GetCollection<BsonDocument>(collectionName);

                var docs = group.Select(entity =>
                {
                    var bson = entity.ToBsonDocument();
                    bson["_t"] = entityType.Name; // 🔧 ensure discriminator is correct
                    return bson;
                }).ToList();

                Console.WriteLine(
                    $"📌 Inserting {docs.Count} flat documents into '{collectionName}' as '{entityType.Name}'");

                const int batchSize = 1000;
                for (var i = 0; i < docs.Count; i += batchSize)
                {
                    var batch = docs.Skip(i).Take(batchSize).ToList();

                    try
                    {
                        await collection.InsertManyAsync(batch);
                    }
                    catch (MongoBulkWriteException<BsonDocument> ex)
                    {
                        Console.WriteLine($"❌ Bulk write error in collection '{collectionName}': {ex.Message}");
                        foreach (var err in ex.WriteErrors)
                            Console.WriteLine($"   ↳ {err.Code} | {err.Message}");
                    }
                }
            }
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
            throw;
        }
        catch (MongoConnectionException ex)
        {
            Console.WriteLine($"❌ MongoDB connection error: {ex.Message}");
            throw new InvalidOperationException("Error connecting to MongoDB.", ex);
        }
        catch (MongoCommandException ex)
        {
            Console.WriteLine($"❌ MongoDB command error: {ex.Message}");
            throw new InvalidOperationException("Error executing command in MongoDB.", ex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Unexpected error: {ex.Message}");
            throw new InvalidOperationException("An unexpected error occurred while inserting data.", ex);
        }
    }

    /// <summary>
    ///     ✅ Updates a record by ID.
    /// </summary>
    public async Task<bool> UpdateAsync(string id, EntityBase entity)
    {
        /*try
        {
            Debug.Assert(_collection is not null, $"{nameof(_collection)} is null");

            var filter = Builders<T>.Filter.Eq("Id", id);
            var result = await _collection.ReplaceOneAsync(filter, entity);

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error updating record with ID {id}: {ex.Message}");
            throw new InvalidOperationException($"Error updating record with ID {id}.", ex);
        }*/
        return false;
    }

    public async Task<Dictionary<string, List<EntityBase>>> GetEntitiesByFileAsync(string fileName)
    {
        var result = new Dictionary<string, List<EntityBase>>(StringComparer.OrdinalIgnoreCase);

        try
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("⚠️ File name cannot be null or empty.", nameof(fileName));

            var mappings = await EntityRegistry.GetRegisteredEquipmentMappingsByFile(fileName);
            if (mappings == null || !mappings.Any())
            {
                Console.WriteLine($"⚠️ No entity mappings found for file: {fileName}");
                return result;
            }

            var collectionName = Path.GetFileNameWithoutExtension(fileName)?.ToLowerInvariant().Trim();
            if (string.IsNullOrWhiteSpace(collectionName))
            {
                Console.WriteLine($"⚠️ Invalid collection name derived from file: {fileName}");
                return result;
            }

            var collection = _icdDatabase.GetCollection<BsonDocument>(collectionName);

            foreach (var mapping in mappings)
            {
                var sheetName = mapping.SheetName;
                var pascalName = char.ToUpper(sheetName[0]) + sheetName[1..].ToLowerInvariant();
                var entityKey = $"{pascalName}Entity";

                var entityType = EntityRegistry.GetEntityType(sheetName, fileName);
                if (entityType == null)
                {
                    Console.WriteLine($"⚠️ Could not resolve type for '{entityKey}' in '{fileName}'");
                    continue;
                }

                var filter = Builders<BsonDocument>.Filter.Eq("_t", entityKey);
                var documents = await collection.Find(filter).ToListAsync();

                var entities = new List<EntityBase>();
                foreach (var doc in documents)
                    try
                    {
                        var entity = (EntityBase)BsonSerializer.Deserialize(doc, entityType);
                        entities.Add(entity);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Deserialization failed for '{entityKey}': {ex.Message}");
                    }

                result[sheetName] = entities;
                Console.WriteLine($"✅ Retrieved {entities.Count} records for sheet '{sheetName}'");
            }

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error retrieving entities for file '{fileName}': {ex.Message}");
            throw new InvalidOperationException("An unexpected error occurred while retrieving entities by file.", ex);
        }
    }


    /// <summary>
    ///     ✅ Retrieves or initializes the collection dynamically based on `T` and `FileKey`
    /// </summary>
    private IMongoCollection<BsonDocument> GetCollection(string? fileKey)
    {
        var collectionName = Path.GetFileNameWithoutExtension(fileKey)?.Replace(" ", "_").ToUpper();
        return _icdDatabase.GetCollection<BsonDocument>(collectionName);
    }
}