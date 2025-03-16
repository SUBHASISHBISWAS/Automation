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
// Created by SUBHASISH BISWAS On: 2025-03-04
// Updated by SUBHASISH BISWAS On: 2025-03-17
//  ******************************************************************************/

#endregion

#region

using System.Collections;
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

public class MongoDataProvider<T>(IICDDbContext icdDataContext) : IDataProvider<T>
    where T : EntityBase
{
    private readonly IMongoDatabase _icdDatabase = icdDataContext.ICDDatabase;

    /// <summary>
    ///     ✅ Retrieves a record by ID.
    /// </summary>
    public async Task<T> GetByIdAsync(string id)
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
    ///     ✅ Retrieves records based on a filter.
    /// </summary>
    public async Task<IEnumerable<T>> GetByFilterAsync(Expression<Func<T, bool>> filter)
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
        return new List<T>();
    }

    /// <summary>
    ///     ✅ Adds a new record to MongoDB.
    /// </summary>
    public async Task AddAsync(T entity)
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
    ///     ✅ Updates a record by ID.
    /// </summary>
    public async Task<bool> UpdateAsync(string id, T entity)
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

    /// <summary>
    ///     ✅ Adds multiple records efficiently in batch.
    /// </summary>
    /// <summary>
    ///     ✅ Adds multiple records efficiently in batch.
    /// </summary>
    public async Task AddManyAsync(IEnumerable<T> entities)
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
    public async Task SeedDataAsync(IEnumerable<T> entities)
    {
        try
        {
            if (entities == null || !entities.Any())
                throw new ArgumentNullException(nameof(entities), "⚠️ No entities to insert.");

            // ✅ Group entities by `FileKey` (FileName_SheetName)
            var groupedEntities = entities.GroupBy(e => e.FileName);

            foreach (var group in groupedEntities)
            {
                // ✅ Extract Collection Name (FileName without extension)
                var collectionName = Path.GetFileNameWithoutExtension(group.Key)?.Replace(" ", "_").ToLower();
                if (string.IsNullOrEmpty(collectionName))
                {
                    Console.WriteLine($"⚠️ Invalid collection name for FileKey: {group.Key}");
                    continue;
                }

                var collection = _icdDatabase.GetCollection<BsonDocument>(collectionName);

                // ✅ Extract Entity Type (DCUEntity, IDUEntity, etc.)
                var entityTypeName = typeof(T).Name;

                Console.WriteLine($"📌 Storing {group.Count()} records in '{collectionName} -> {entityTypeName}'");

                // ✅ Construct the document in the correct structure
                var filter = Builders<BsonDocument>.Filter.Exists(entityTypeName);
                var update = Builders<BsonDocument>.Update
                    .PushEach($"{entityTypeName}.Entities", group.Select(e => e.ToBsonDocument()).ToList());

                var options = new UpdateOptions { IsUpsert = true };

                await collection.UpdateOneAsync(filter, update, options);
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

    public async Task<IEnumerable<T>> GetAllAsync(string? fileName = null)
    {
        try
        {
            // ✅ Fetch stored data
            var storedData = await GetStoredDataAsync();

            // ✅ Extract entity collections for the specified file
            if (!storedData.TryGetValue(fileName ?? string.Empty, out var entityCollections) ||
                entityCollections == null)
            {
                Console.WriteLine($"⚠️ No data found for file: {fileName}");
                return new List<T>(); // Return an empty list if no data found
            }

            // ✅ Flatten the nested lists and filter to the correct type
            var allEntities = entityCollections
                .SelectMany(e => e)
                .Where(entity => entity is T)
                .Cast<T>()
                .ToList();

            Console.WriteLine($"✅ Retrieved {allEntities.Count} entities from '{fileName}'");

            return allEntities;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error retrieving entities: {ex.Message}");
            throw new InvalidOperationException("An unexpected error occurred while retrieving entities.", ex);
        }
    }

    private async Task<Dictionary<string, List<List<EntityBase>>>> GetStoredDataAsync(string? fileName = null)
    {
        try
        {
            // ✅ Fetch dynamic objects from GetAllEntityAsync
            var result = await GetAllEntityAsync(fileName);

            if (result is not List<object> allDatabaseInstances || !allDatabaseInstances.Any())
            {
                Console.WriteLine("⚠️ No data found to convert.");
                return new Dictionary<string, List<List<EntityBase>>>();
            }

            Dictionary<string, List<List<EntityBase>>> databaseResults = new();

            foreach (var dbInstance in allDatabaseInstances)
            {
                var dbType = dbInstance.GetType();
                var dbName = dbType.Name; // Extract database name

                List<List<EntityBase>> entityCollections = new();

                foreach (var property in dbType.GetProperties())
                {
                    // ✅ Get the List<EntityType> property (e.g., List<DCUEntity>)
                    var entityList = property.GetValue(dbInstance) as IList;

                    if (entityList == null) continue;

                    var entityType = entityList.GetType().GetGenericArguments()[0]; // Get DCUEntity, BCHEntity, etc.

                    // ✅ Convert each entity inside the list to EntityBase
                    var castedEntities = entityList.Cast<EntityBase>().ToList();

                    if (castedEntities.Any())
                        entityCollections.Add(castedEntities);
                }

                if (entityCollections.Any()) databaseResults[dbName] = entityCollections;
            }

            Console.WriteLine("\u2705 Successfully converted to dictionary format.");
            return databaseResults;
        }
        catch (TypeLoadException ex)
        {
            Console.WriteLine($"❌ TypeLoadException: {ex.Message}");
            throw new InvalidOperationException("Error loading dynamic type. Ensure the type was correctly created.",
                ex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error converting data: {ex.Message}");
            throw new InvalidOperationException("Error converting output to dictionary format.", ex);
        }
    }

    /// <summary>
    ///     ✅ Retrieves all records from MongoDB.
    /// </summary>
    private async Task<object> GetAllEntityAsync(string? fileName = null)
    {
        try
        {
            List<object> allDatabaseInstances = new();
            List<string> collectionsToProcess;

            // ✅ Fetch collections from EntityRegistry if `fileName` is null
            if (string.IsNullOrEmpty(fileName))
            {
                Console.WriteLine("📌 Fetching all data from EntityRegistry...");
                var allMappings = EntityRegistry.GetAllMappings();

                if (!allMappings.Any())
                {
                    Console.WriteLine("⚠️ No entity mappings found in the registry.");
                    return new List<object>(); // Return empty list if no mappings
                }

                collectionsToProcess = allMappings
                    .Select(m => Path.GetFileNameWithoutExtension(m.FileName).Replace(" ", "_").ToLower())
                    .Distinct()
                    .ToList();
            }
            else
            {
                // ✅ Process only the requested collection
                collectionsToProcess = new List<string>
                    { Path.GetFileNameWithoutExtension(fileName).Replace(" ", "_").ToLower() };
            }

            // ✅ Debug: Show available collections
            var availableCollections = _icdDatabase.ListCollectionNames().ToList();
            Console.WriteLine($"📌 Available collections in MongoDB: {string.Join(", ", availableCollections)}");

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

                // ✅ Identify all entity names inside the database collection
                var entityNames = documents
                    .SelectMany(doc => doc.Elements)
                    .Where(e => e.Name != "_id") // Ignore `_id`
                    .Select(e => e.Name)
                    .Distinct()
                    .ToList();

                // ✅ Create a dynamic database type (trdp_icd_cc, trdp_icd_generated)
                var dynamicDbType = DynamicTypeFactory.CreateDatabaseType(collectionName, entityNames);
                var dbInstance = Activator.CreateInstance(dynamicDbType);

                foreach (var doc in documents)
                foreach (var element in doc.Elements)
                {
                    if (element.Name == "_id") continue; // Skip `_id`

                    var entityTypeName = element.Name; // e.g., "BCHEntity", "DCUEntity"

                    // ✅ Ensure correct entity type is used from EntityRegistry
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

                    // ✅ Deserialize using the correct entity type
                    var entities = entityDoc["Entities"]
                        .AsBsonArray
                        .Select(e => BsonSerializer.Deserialize(e.AsBsonDocument, correctEntityType))
                        .ToList();

                    // ✅ Create a strongly typed list dynamically
                    var genericListType = typeof(List<>).MakeGenericType(correctEntityType);
                    var stronglyTypedEntities = Activator.CreateInstance(genericListType) as IList;

                    foreach (var entity in entities)
                        stronglyTypedEntities?.Add(entity);

                    // ✅ Store entity list inside the dynamic database wrapper
                    var dbProperty = dynamicDbType.GetProperty(entityTypeName);
                    if (dbProperty != null && stronglyTypedEntities != null)
                        dbProperty.SetValue(dbInstance, stronglyTypedEntities);
                }

                allDatabaseInstances.Add(dbInstance);
            }

            // ✅ Return all databases if `fileName` is null
            if (string.IsNullOrEmpty(fileName))
            {
                Console.WriteLine($"✅ Retrieved {allDatabaseInstances.Count} databases.");
                return allDatabaseInstances;
            }

            // ✅ Return a single database if `fileName` is provided
            var resultContainer = allDatabaseInstances.FirstOrDefault();
            Console.WriteLine($"✅ Retrieved data structure for '{fileName}'.");
            return resultContainer ?? new object();
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
            return new List<object>(); // Return an empty object instead of failing
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Unexpected error: {ex.Message}");
            throw new InvalidOperationException("An unexpected error occurred while retrieving data.", ex);
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