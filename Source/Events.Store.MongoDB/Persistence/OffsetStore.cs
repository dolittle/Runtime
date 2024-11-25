// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.MongoDB.Migrations;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Persistence;

[Singleton, PerTenant]
public class OffsetStore : EventStoreConnection, IOffsetStore
{
    static readonly FilterDefinitionBuilder<StreamMetadata> _filterBuilder = new();


    const string EventLogMetadataCollectionName = "event-log-metadata";

    IMongoCollection<StreamMetadata> Collection { get; }

    public OffsetStore(IDatabaseConnection connection) : base(connection)
    {
        Collection = Database.GetCollection<StreamMetadata>(EventLogMetadataCollectionName);

        Init();
    }

    void Init()
    {
        var collectionNames = Database.ListCollectionNames().ToList();
        if (!collectionNames.Contains(EventLogMetadataCollectionName))
        {
            Database.CreateCollection(EventLogMetadataCollectionName);
        }

        SetInitialOffsetsForStreams(collectionNames);
    }

    void SetInitialOffsetsForStreams(List<string> collectionNames)
    {
        using var session = Database.Client.StartSession();
        try
        {
            session.StartTransaction();
            var streamCollections = collectionNames.Where(StreamIdMatcher.IsStreamOrEventLog).ToList();
            var currentOffsets = Collection.Find(_ => true).ToList();
            foreach (var stream in streamCollections)
            {
                var storedOffset = currentOffsets.FirstOrDefault(metadata => metadata.StreamName == stream)
                    ?.NextEventOffset;
                InitStream(session, stream, storedOffset);
            }
            session.CommitTransaction();
        }
        catch (Exception e)
        {
            session.AbortTransaction();
            throw new OffsetStoreInitFailed(e, "Failed to initialize stream metadata");
        }
    }

    void InitStream(IClientSessionHandle session, string stream, ulong? storedOffset)
    {
        var currentOffset = GetCurrentOffsetForStream(session, stream);
        if (currentOffset is null)
        {
            return;
        }
        var nextOffset = currentOffset.Value + 1;
        if(storedOffset >= nextOffset)
        {
            // Initialized, no need to update
            return;
        }

        Collection.ReplaceOne(session, GetFilter(stream), new StreamMetadata
        {
            StreamName = stream,
            NextEventOffset = nextOffset,
        }, new ReplaceOptions
        {
            IsUpsert = true,
        });
    }

    /// <summary>
    ///  Get _id of the last event in the stream
    /// </summary>
    /// <param name="session"></param>
    /// <param name="stream"></param>
    /// <returns></returns>
    private ulong? GetCurrentOffsetForStream(IClientSessionHandle session, string stream)
    {
        var collection = Database.GetCollection<object>(stream);
        var filter = Builders<object>.Filter.Empty;
        var sort = Builders<object>.Sort.Descending("_id");
        var projection = Builders<object>.Projection.Include("_id");
        var cursor = collection.Find(session, filter).Sort(sort).Limit(1).Project(projection).ToCursor()
            .FirstOrDefault();
        if (cursor is null)
        {
            return null;
        }

        var bsonValue = cursor["_id"].ToInt64();
        return (ulong)bsonValue;
    }

    public Task UpdateOffset(string stream, IClientSessionHandle session, ulong nextEventOffset,
        CancellationToken cancellationToken)
    {
        var metadata = new StreamMetadata
        {
            StreamName = stream,
            NextEventOffset = nextEventOffset,
        };

        var updateDefinition = new UpdateDefinitionBuilder<StreamMetadata>()
            .SetOnInsert(it => it.StreamName, stream)
            .Set(it => it.NextEventOffset, metadata.NextEventOffset);

        return Collection.UpdateOneAsync(
            session: session,
            filter: GetFilter(stream),
            updateDefinition,
            options: new UpdateOptions
            {
                IsUpsert = true,
            }, cancellationToken);
    }

    public Task<ulong> GetNextOffset(string stream, IClientSessionHandle? session, CancellationToken cancellationToken)
    {
        var find = session is not null
            ? Collection.Find(session, GetFilter(stream))
            : Collection.Find(GetFilter(stream));
        return find
            .Project(metadata => metadata.NextEventOffset)
            .FirstOrDefaultAsync(cancellationToken);
    }

    static FilterDefinition<StreamMetadata> GetFilter(string streamName) =>
        _filterBuilder.Eq(metadata => metadata.StreamName, streamName);
}

public class OffsetStoreInitFailed : Exception
{
    public OffsetStoreInitFailed(Exception exception, string message): base(message, exception)
    {
    }
}
