// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
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

        CreateCollectionIfNotExists();
    }

    void CreateCollectionIfNotExists()
    {
        var collectionNames = Database.ListCollectionNames().ToList();
        if (!collectionNames.Contains(EventLogMetadataCollectionName))
        {
            Database.CreateCollection(EventLogMetadataCollectionName);
        }
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
