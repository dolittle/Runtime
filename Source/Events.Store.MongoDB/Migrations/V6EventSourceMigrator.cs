// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Diagnostics.OpenTelemetry;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations;

public static class V6EventSourceMigrator
{
    static readonly string[] _migratedFields = { "Metadata.EventSource" };

    public static async Task MigrateEventsourceId(IMongoCollection<BsonDocument> collection)
    {
        using var activity = RuntimeActivity.Source.StartActivity();
        
        var eventLogHasUuidFields = await ContainsUuidFieldsAsync(collection, _migratedFields);
        if (eventLogHasUuidFields)
        {
            await ConvertUuidFieldsToStringAsync(collection, _migratedFields);
        }
    }

    public static async Task<bool> ContainsUuidFieldsAsync(IMongoCollection<BsonDocument> collection, params string[] uuidFieldNames)
    {
        // Build the filter to check for any UUID fields
        var queryFilter = AnyIsBinaryFilter(uuidFieldNames);

        // Check if any document matches the criteria
        var isUuidPresent = await collection.Find(queryFilter).AnyAsync();

        return isUuidPresent;
    }

    static FilterDefinition<BsonDocument> AnyIsBinaryFilter(string[] uuidFieldNames)
    {
        var filters = new List<FilterDefinition<BsonDocument>>();
        foreach (var fieldName in uuidFieldNames)
        {
            // Check for UUID type
            var binaryTypeFilter = Builders<BsonDocument>.Filter.Type(fieldName, BsonType.Binary);

            filters.Add(binaryTypeFilter);
        }

        // Combine all filters using $or to check if any of the fields are of UUID type
        var queryFilter = Builders<BsonDocument>.Filter.Or(filters);
        return queryFilter;
    }

    /// <summary>
    /// Creates a copy of the collection with the given UUID fields converted to strings
    /// </summary>
    /// <param name="collection">Source collection</param>
    /// <param name="uuidFieldNames">Which fields to convert</param>
    public static async Task ConvertUuidFieldsToStringAsync(IMongoCollection<BsonDocument> collection, params string[] uuidFieldNames)
    {
        var collectionName = collection.CollectionNamespace.CollectionName;
        var tempCollectionName = $"{collectionName}_temp";
        // Create a temporary collection
        var tempCollection = collection.Database.GetCollection<BsonDocument>(tempCollectionName);
        // Backup the original collection to the temporary collection


        // Create a projection to include only the fields that need to be converted
        var projection = Builders<BsonDocument>.Projection.Include("_id");
        foreach (var fieldName in uuidFieldNames)
        {
            projection = projection.Include(fieldName);
        }

        // Find all documents that contain the specified fields
        using var cursor = await collection.Find(AnyIsBinaryFilter(uuidFieldNames)).Project(projection).ToCursorAsync();
        var bulkOps = new List<WriteModel<BsonDocument>>();
        while (await cursor.MoveNextAsync())
        {
            foreach (var doc in cursor.Current)
            {
                var updateDoc = new BsonDocument();
                foreach (var fieldName in uuidFieldNames)
                {
                    if (!TryGetValue(doc,fieldName, out var element))
                    {
                        Console.WriteLine($"Field {fieldName} not found for id {doc["_id"]}");
                    }
                    else if (element!.IsBsonBinaryData)
                    {
                        // Console.WriteLine($"Converting {fieldName} to string for id {doc["_id"]}");
                        var binaryData = element.AsBsonBinaryData;
                        if (binaryData.SubType is BsonBinarySubType.UuidLegacy or BsonBinarySubType.UuidStandard)
                        {
                            var guid = binaryData.ToGuid();
                            updateDoc.Add(fieldName, guid.ToString());
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Field {fieldName} is not a UUID for id {doc["_id"]}");
                    }
                }

                if (updateDoc.ElementCount > 0)
                {
                    // Create the updated document with UUID fields converted to strings
                    var filter = Builders<BsonDocument>.Filter.Eq("_id", doc["_id"]);
                    var update = Builders<BsonDocument>.Update.Combine(updateDoc.Select(field => Builders<BsonDocument>.Update.Set(field.Name, field.Value)));
                    bulkOps.Add(new UpdateOneModel<BsonDocument>(filter, update));
                }

                // Execute in batches of 1000 for efficiency
                if (bulkOps.Count >= 1000)
                {
                    await collection.BulkWriteAsync(bulkOps);
                    bulkOps.Clear();
                }
            }
        }

        // Write any remaining operations
        if (bulkOps.Count > 0)
        {
            await collection.BulkWriteAsync(bulkOps);
        }
    }


    static bool TryGetValue(BsonDocument doc, string path, [NotNullWhen(true)] out BsonValue? value)
    {
        // Initialize the output value
        value = default;

        // Split the path into parts for nested documents
        var parts = path.Split('.');
        var currentDoc = doc;

        for (var i = 0; i < parts.Length; i++)
        {
            if (currentDoc.TryGetValue(parts[i], out var bsonValue))
            {
                if (i == parts.Length - 1)
                {
                    value = bsonValue;
                    return true;
                }
                else if (bsonValue.IsBsonDocument)
                {
                    // Move deeper into the nested documents
                    currentDoc = bsonValue.AsBsonDocument;
                }
                else
                {
                    return false; // The path is not valid
                }
            }
            else
            {
                return false; // The key does not exist
            }
        }

        return false; // The path was not valid
    }
}
