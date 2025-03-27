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
// Updated by SUBHASISH BISWAS On: 2025-03-27
//  ******************************************************************************/

#endregion

#region

using System.Linq.Expressions;
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
    public async Task<bool> DeleteAsync(string id)
    {
        /*try
        {
            Debug.Assert(_collection is not null, $"{nameof(_collection)} is null");

            var filter = Builders<T>.Filter.Eq("Id", id);
            var result = await _collection.DeleteOneAsync(filter);

            return result.IsAcknowledged && result.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error deleting record with ID {id}: {ex.Message}");
            throw new InvalidOperationException($"Error deleting record with ID {id}.", ex);
        }*/
        return false;
    }

    /// <summary>
    ///     ✅ Deletes all records from the collection.
    /// </summary>
    public async Task<bool> DeleteAllAsync()
    {
        /*try
        {
            Debug.Assert(_collection is not null, $"{nameof(_collection)} is null");

            var result = await _collection.DeleteManyAsync(_ => true);
            return result.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error deleting all records: {ex.Message}");
            throw new InvalidOperationException("Error deleting all records from MongoDB.", ex);
        }*/

        return false;
    }

    public async Task<IEnumerable<EntityBase>> GetAllAsync(string fileName, string sheetName)
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
    ///     ✅ Retrieves a record by ID.
    /// </summary>
    public async Task<EntityBase> GetByIdAsync(string id)
    {
        /*try
        {
            Debug.Assert(_collection is not null, $"{nameof(_collection)} is null");
            var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id)); // Use `_id`
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error fetching record with ID {id}: {ex.Message}");
            throw new InvalidOperationException($"Error retrieving record with ID {id}.", ex);
        }*/

        return default!;
    }

    /// <summary>
    ///     ✅ Adds a new record to MongoDB.
    /// </summary>
    public async Task AddAsync(EntityBase entity)
    {
        /*try
        {
            Debug.Assert(_collection is not null, $"{nameof(_collection)} is null");
            await _collection.InsertOneAsync(entity);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error inserting record: {ex.Message}");
            throw new InvalidOperationException("Error adding record to MongoDB.", ex);
        }*/
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
    ///     ✅ Retrieves records based on a filter.
    /// </summary>
    public async Task<IEnumerable<EntityBase>> GetByFilterAsync(Expression<Func<EntityBase, bool>> filter)
    {
        /*try
        {
            Debug.Assert(_collection is not null, $"{nameof(_collection)} is null");
            return await _collection.Find(filter).ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error fetching records with filter: {ex.Message}");
            throw new InvalidOperationException("Error retrieving records based on the provided filter.", ex);
        }*/
        return new List<EntityBase>();
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

    private async Task<Dictionary<string, List<List<EntityBase>>>> GetStoredDataAsync(string? fileName = null)
    {
        try
        {
            var result = await GetAllEntityAsync(fileName);

            if (result is not List<EntityBase> allEntities || !allEntities.Any())
            {
                Console.WriteLine("⚠️ No data found to convert.");
                return new Dictionary<string, List<List<EntityBase>>>();
            }

            // ✅ Group by file name (normalize to lowercase & remove extension)
            var groupedByFile = allEntities
                .Where(e => !string.IsNullOrWhiteSpace(e.FileName))
                .GroupBy(e => Path.GetFileNameWithoutExtension(e.FileName!).ToLowerInvariant())
                .ToDictionary(
                    g => g.Key,
                    g => g.ToList()
                );

            Dictionary<string, List<List<EntityBase>>> databaseResults = new();

            foreach (var kvp in groupedByFile)
            {
                var fileKey = kvp.Key;
                var entities = kvp.Value;

                // ✅ Group further by entity type inside the file (if needed)
                var groupedByEntityType = entities
                    .GroupBy(e => e.GetType().Name)
                    .Select(g => g.ToList())
                    .ToList();

                databaseResults[fileKey] = groupedByEntityType;
            }

            Console.WriteLine("✅ Successfully grouped data by file name.");
            return databaseResults;
        }
        catch (TypeLoadException ex)
        {
            Console.WriteLine($"❌ TypeLoadException: {ex.Message}");
            throw new InvalidOperationException(
                "Error loading dynamic type. Ensure the type was correctly created.",
                ex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error converting data: {ex.Message}");
            throw new InvalidOperationException("Error converting output to dictionary format.", ex);
        }
    }

    private async Task<List<EntityBase>> GetAllEntityAsync(string? fileName = null)
    {
        try
        {
            List<EntityBase> allEntities = new();
            List<string> collectionsToProcess;

            if (string.IsNullOrEmpty(fileName))
            {
                Console.WriteLine("📌 Fetching all data from EntityRegistry...");
                var allMappings = EntityRegistry.GetAllMappings();

                if (!allMappings.Any())
                {
                    Console.WriteLine("⚠️ No entity mappings found in the registry.");
                    return allEntities;
                }

                collectionsToProcess = allMappings
                    .Select(m => Path.GetFileNameWithoutExtension(m.FileName).Replace(" ", "_").ToLower())
                    .Distinct()
                    .ToList();
            }
            else
            {
                collectionsToProcess = new List<string>
                    { Path.GetFileNameWithoutExtension(fileName).Replace(" ", "_").ToLower() };
            }

            var availableCollections = (await _icdDatabase.ListCollectionNamesAsync()).ToList();
            Console.WriteLine($"📌 Available collections in MongoDB: {string.Join(", ", availableCollections)}");

            await ExtractEntitiesFromDocuments(collectionsToProcess, availableCollections, allEntities);

            Console.WriteLine($"✅ Total entities retrieved: {allEntities.Count}");
            return allEntities;
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
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"⚠️ Missing expected field: {ex.Message}");
            return new List<EntityBase>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Unexpected error: {ex.Message}");
            throw new InvalidOperationException("An unexpected error occurred while retrieving data.", ex);
        }
    }

    private async Task ExtractEntitiesFromDocuments(List<string> collectionsToProcess,
        List<string> availableCollections, List<EntityBase> allEntities)
    {
        foreach (var collectionName in collectionsToProcess)
        {
            if (!availableCollections.Contains(collectionName))
            {
                Console.WriteLine($"⚠️ Collection '{collectionName}' does not exist.");
                continue;
            }

            var collection = _icdDatabase.GetCollection<BsonDocument>(collectionName);
            var documents = await collection.Find(new BsonDocument()).ToListAsync();

            Console.WriteLine($"📌 Found {documents.Count} documents in '{collectionName}'");

            if (!documents.Any())
            {
                Console.WriteLine($"⚠️ No records found in '{collectionName}'");
                continue;
            }

            foreach (var doc in documents)
            foreach (var element in doc.Elements)
            {
                if (element.Name == "_id") continue;

                var entityTypeName = element.Name;
                var correctEntityType = EntityRegistry.GetEntityType(entityTypeName);
                if (correctEntityType == null)
                {
                    Console.WriteLine($"❌ Unable to resolve entity type for '{entityTypeName}'. Skipping...");
                    continue;
                }

                Console.WriteLine($"✅ Using entity type: {correctEntityType.FullName}");

                if (!doc.Contains(entityTypeName) || !doc[entityTypeName].IsBsonDocument)
                {
                    Console.WriteLine($"⚠️ Skipping '{entityTypeName}' as it is not a valid entity.");
                    continue;
                }

                var entityDoc = doc[entityTypeName].AsBsonDocument;

                if (!entityDoc.Contains("Entities") || !entityDoc["Entities"].IsBsonArray)
                {
                    Console.WriteLine($"⚠️ Skipping '{entityTypeName}' as it lacks a valid 'Entities' array.");
                    continue;
                }

                var entities = entityDoc["Entities"]
                    .AsBsonArray
                    .Select(e => BsonSerializer.Deserialize(e.AsBsonDocument, correctEntityType))
                    .OfType<EntityBase>()
                    .ToList();

                allEntities.AddRange(entities);
            }
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