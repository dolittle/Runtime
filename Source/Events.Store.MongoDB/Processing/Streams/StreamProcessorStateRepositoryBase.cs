// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.MongoDB;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams;

public abstract class StreamProcessorStateRepositoryBase<TId, TState, TDocument> : IStreamProcessorStateRepository<TId, TState>
    where TId : IStreamProcessorId
    where TState : IStreamProcessorState
{
    readonly Func<ScopeId, CancellationToken, Task<IMongoCollection<TDocument>>> _getCollection;
    readonly ILogger _logger;

    public StreamProcessorStateRepositoryBase(Func<ScopeId, CancellationToken, Task<IMongoCollection<TDocument>>> getCollection, ILogger logger)
    {
        _getCollection = getCollection;
        _logger = logger;
    }

    public async Task<Try<TState>> TryGet(TId streamProcessorId, CancellationToken cancellationToken)
    {
        _logger.GettingStreamProcessorState(streamProcessorId);
        try
        {
            var collection = await _getCollection(streamProcessorId.ScopeId, cancellationToken).ConfigureAwait(false);
            var persistedState = await collection.Find(CreateFilter(streamProcessorId))
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
            return persistedState == null
                ? new StreamProcessorStateDoesNotExist(streamProcessorId)
                : Try<TState>.Succeeded(ConvertToStateWithId(streamProcessorId.ScopeId, persistedState).State);
                
        }
        catch (MongoWaitQueueFullException ex)
        {
            return new EventStoreUnavailable("Mongo wait queue is full", ex);
        }
    }

    public async IAsyncEnumerable<StreamProcessorStateWithId<TId, TState>> GetForScope(ScopeId scopeId, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var collection = await _getCollection(ScopeId.Default, cancellationToken).ConfigureAwait(false);
        var states = collection
            .Find(FilterDefinition<TDocument>.Empty)
            .ToAsyncEnumerable(cancellationToken: cancellationToken)
            .Select(_ => ConvertToStateWithId(scopeId, _));
        await foreach (var state in states.WithCancellation(cancellationToken))
        {
            yield return state;
        }
    }

    public async Task<Try> PersistForScope(ScopeId scope, IReadOnlyDictionary<TId, TState> streamProcessorStates, CancellationToken cancellationToken)
    {
        if (!streamProcessorStates.Keys.All(_ => _.ScopeId.Equals(scope)))
        {
            return new NotAllStreamProcessorStatesInSameScope(scope);
        }
        try
        {
            var collection = await _getCollection(scope, cancellationToken).ConfigureAwait(false);
            var writeResult = await collection
                .BulkWriteAsync(
                    streamProcessorStates.Select(_ => new ReplaceOneModel<TDocument>(CreateFilter(_.Key), CreateDocument(_.Key, _.Value))
                    {
                        IsUpsert = true
                    }),
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            
            // The write result may or may not have written all models. Maybe that should be checked?
            return Try.Succeeded;
        }
        catch (MongoWaitQueueFullException ex)
        {
            return new EventStoreUnavailable("Mongo wait queue is full", ex);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    protected abstract FilterDefinition<TDocument> CreateFilter(TId id);
    protected abstract TDocument CreateDocument(TId id, TState state);
    protected abstract StreamProcessorStateWithId<TId, TState> ConvertToStateWithId(ScopeId scope, TDocument document);
}
