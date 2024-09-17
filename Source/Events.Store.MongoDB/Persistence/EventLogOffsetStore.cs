// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.Persistence;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Persistence;

[Singleton, PerTenant]
public class EventLogOffsetStore: EventStoreConnection, IEventLogOffsetStore
{
    static readonly FilterDefinitionBuilder<EventLogMetadata> _filterBuilder = new();
    static readonly FilterDefinition<EventLogMetadata> _defaultScopeFilter = _filterBuilder.Eq(metadata => metadata.Scope, ScopeId.Default.Value);
    
    
    const string EventLogMetadataCollectionName = "event-log-metadata";

    public IMongoCollection<EventLogMetadata> Collection { get; }

    public EventLogOffsetStore(IDatabaseConnection connection):base(connection)
    {
        Collection = Database.GetCollection<EventLogMetadata>(EventLogMetadataCollectionName);

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

    public Task UpdateOffset(IClientSessionHandle session, ScopeId scopeId, ulong nextEventOffset,
        CancellationToken cancellationToken)
    {
        var metadata = new EventLogMetadata
        {
            Scope = scopeId.Value,
            NextEventOffset = nextEventOffset,
        };
        
        var updateDefinition = new UpdateDefinitionBuilder<EventLogMetadata>()
            .SetOnInsert(it => it.Scope, metadata.Scope)
            .Set(it => it.NextEventOffset, metadata.NextEventOffset);

        return Collection.UpdateOneAsync(
            session:session,
            filter: GetFilter(scopeId),
            updateDefinition,
            options: new UpdateOptions
            {
                IsUpsert = true,
            }, cancellationToken);
    }

    public Task<ulong> GetNextOffset(ScopeId scopeId, CancellationToken cancellationToken)
        => Collection
            .Find(GetFilter(scopeId))
            .Project(metadata => metadata.NextEventOffset)
            .FirstOrDefaultAsync(cancellationToken);

    static FilterDefinition<EventLogMetadata> GetFilter(ScopeId scopeId) => scopeId.IsDefaultScope ? _defaultScopeFilter : _filterBuilder.Eq(metadata => metadata.Scope, scopeId.Value);
}
