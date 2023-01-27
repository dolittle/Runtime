// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.EventHorizon.Consumer;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.MongoDB;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SubscriptionState = Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams.EventHorizon.SubscriptionState;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams;

/// <summary>
/// Represents an implementation of <see cref="Store.Streams.IStreamProcessorStates" />.
/// </summary>
public class StreamProcessorStateRepository : IStreamProcessorStateRepository
{
    readonly FilterDefinitionBuilder<AbstractStreamProcessorState> _streamProcessorFilter = Builders<AbstractStreamProcessorState>.Filter;
    readonly IStreamProcessorStateCollections _streamProcessorStateCollections;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamProcessorStateRepository"/> class.
    /// </summary>
    /// <param name="streamProcessorStateCollections">The <see cref="IStreamProcessorStateCollections" />.</param>
    /// <param name="logger">An <see cref="ILogger"/>.</param>
    public StreamProcessorStateRepository(IStreamProcessorStateCollections streamProcessorStateCollections, ILogger logger)
    {
        _streamProcessorStateCollections = streamProcessorStateCollections;
        _logger = logger;
    }

    /// <summary>
    /// Gets the <see cref="IStreamProcessorState" /> for the given <see cref="IStreamProcessorId" /> from the correct
    /// collection, either <see cref="SubscriptionState" /> or <see cref="StreamProcessorState" />.
    /// </summary>
    /// <param name="id">The unique <see cref="IStreamProcessorId" /> representing either the <see cref="AbstractScopedStreamProcessor"/>
    /// or <see cref="SubscriptionId" />.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{TResult}" />.</returns>
    public async Task<Try<IStreamProcessorState>> TryGet(StreamProcessorId id, CancellationToken cancellationToken)
    {
        _logger.GettingStreamProcessorState(id);
        try
        {
            var collection = await _streamProcessorStateCollections.Get(id.ScopeId, cancellationToken).ConfigureAwait(false);
            var persistedState = await collection.Find(CreateFilter(id))
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
            return persistedState == default
                ? new StreamProcessorStateDoesNotExist(id)
                : Try<IStreamProcessorState>.Succeeded(persistedState.ToRuntimeRepresentation());
                
        }
        catch (MongoWaitQueueFullException ex)
        {
            return new EventStoreUnavailable("Mongo wait queue is full", ex);
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<StreamProcessorStateWithId<StreamProcessorId, IStreamProcessorState>> GetForScope(ScopeId scopeId, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        _logger.GettingAllStreamProcessorState();
        var collection = await _streamProcessorStateCollections.Get(ScopeId.Default, cancellationToken).ConfigureAwait(false);
        var states = collection
            .Find(FilterDefinition<AbstractStreamProcessorState>.Empty)
            .ToAsyncEnumerable(cancellationToken: cancellationToken)
            .Select(document => new StreamProcessorStateWithId<StreamProcessorId, IStreamProcessorState>(new StreamProcessorId(ScopeId.Default, document.EventProcessor, document.SourceStream),
                document.ToRuntimeRepresentation()));
        await foreach (var state in states.WithCancellation(cancellationToken))
        {
            yield return state;
        }
    }


    /// <inheritdoc />
    public IAsyncEnumerable<StreamProcessorStateWithId<StreamProcessorId, IStreamProcessorState>> GetNonScoped(CancellationToken cancellationToken)
        => GetForScope(ScopeId.Default, cancellationToken); 

    public async Task<Try> PersistForScope(ScopeId scopeId, IReadOnlyDictionary<StreamProcessorId, IStreamProcessorState> streamProcessorStates, CancellationToken cancellationToken)
    {
        // var collection = await _streamProcessorStateCollections.Get(ScopeId.Default, cancellationToken).ConfigureAwait(false);
        // if (!streamProcessorStates.Keys.All(_ => _.ScopeId.Equals(scopeId)))
        // {
        //     return new 
        // }
        // var tasksWithIds = streamProcessorStates.Select(_ => (_.Key, Persist(_.Key, _.Value, cancellationToken)));
        // await Task.WhenAll(tasksWithIds.Select(_ => _.Item2)).ConfigureAwait(false);
        // return Try.Succeeded;
        return Try.Succeeded;
    }


    // async Task<(IStreamProcessorId, Partial)> Persist(IStreamProcessorId id, IStreamProcessorState baseStreamProcessorState, CancellationToken cancellationToken)
    // {
    //     _logger.PersistingStreamProcessorState(id);
    //     try
    //     {
    //         if (id is SubscriptionId subscriptionId)
    //         {
    //             if (baseStreamProcessorState is Runtime.Events.Processing.Streams.StreamProcessorState streamProcessorState)
    //             {
    //                 var replacementState = new SubscriptionState(
    //                     subscriptionId.ProducerMicroserviceId,
    //                     subscriptionId.ProducerTenantId,
    //                     subscriptionId.StreamId,
    //                     subscriptionId.PartitionId,
    //                     streamProcessorState.Position,
    //                     streamProcessorState.RetryTime.UtcDateTime,
    //                     streamProcessorState.FailureReason,
    //                     streamProcessorState.ProcessingAttempts,
    //                     streamProcessorState.LastSuccessfullyProcessed.UtcDateTime,
    //                     streamProcessorState.IsFailing);
    //                 var states = await _subscriptionStateCollections.Get(subscriptionId.ScopeId, cancellationToken).ConfigureAwait(false);
    //                 await states.ReplaceOneAsync(
    //                         CreateFilter(subscriptionId),
    //                         replacementState,
    //                         new ReplaceOptions { IsUpsert = true })
    //                     .ConfigureAwait(false);
    //             }
    //             else
    //             {
    //                 return (id, Partial.Failed(new UnsupportedStreamProcessorStatewithSubscriptionId(subscriptionId, baseStreamProcessorState)));
    //             }
    //         }
    //         else if (baseStreamProcessorState is Runtime.Events.Processing.Streams.Partitioned.StreamProcessorState partitionedStreamProcessorState)
    //         {
    //             var streamProcessorId = id as StreamProcessorId;
    //             var states = await _streamProcessorStatesCollectionSelector.Get(streamProcessorId.ScopeId, cancellationToken).ConfigureAwait(false);
    //             await states.ReplaceOneAsync(
    //                     CreateFilter(streamProcessorId),
    //                     new Partitioned.PartitionedStreamProcessorState(
    //                         streamProcessorId.EventProcessorId,
    //                         streamProcessorId.SourceStreamId,
    //                         partitionedStreamProcessorState.Position,
    //                         partitionedStreamProcessorState.FailingPartitions.ToDictionary(
    //                             kvp => kvp.Key.Value.ToString(),
    //                             kvp => new FailingPartitionState(
    //                                 kvp.Value.Position,
    //                                 kvp.Value.RetryTime.UtcDateTime,
    //                                 kvp.Value.Reason,
    //                                 kvp.Value.ProcessingAttempts,
    //                                 kvp.Value.LastFailed.UtcDateTime)),
    //                         partitionedStreamProcessorState.LastSuccessfullyProcessed.UtcDateTime),
    //                     new ReplaceOptions { IsUpsert = true })
    //                 .ConfigureAwait(false);
    //         }
    //         else if (baseStreamProcessorState is Runtime.Events.Processing.Streams.StreamProcessorState streamProcessorState)
    //         {
    //             var streamProcessorId = id as StreamProcessorId;
    //             var states = await _streamProcessorStatesCollectionSelector.Get(streamProcessorId.ScopeId, cancellationToken).ConfigureAwait(false);
    //             await states.ReplaceOneAsync(
    //                     CreateFilter(streamProcessorId),
    //                     new StreamProcessorState(
    //                         streamProcessorId.EventProcessorId,
    //                         streamProcessorId.SourceStreamId,
    //                         streamProcessorState.Position,
    //                         streamProcessorState.RetryTime.UtcDateTime,
    //                         streamProcessorState.FailureReason,
    //                         streamProcessorState.ProcessingAttempts,
    //                         streamProcessorState.LastSuccessfullyProcessed.UtcDateTime,
    //                         streamProcessorState.IsFailing),
    //                     new ReplaceOptions { IsUpsert = true })
    //                 .ConfigureAwait(false);
    //         }
    //         else
    //         {
    //             return (id, Partial.Failed(new StreamProcessorStateOfUnsupportedType(id, baseStreamProcessorState)));
    //         }
    //     }
    //     catch (MongoWaitQueueFullException ex)
    //     {
    //         return (id, Partial.PartialSuccess(new EventStoreUnavailable("Mongo wait queue is full", ex)));
    //     }
    //
    //     return (id, Partial.Succeeded());
    // }


    FilterDefinition<AbstractStreamProcessorState> CreateFilter(StreamProcessorId id) =>
        _streamProcessorFilter.Eq(_ => _.EventProcessor, id.EventProcessorId.Value)
        & _streamProcessorFilter.Eq(_ => _.SourceStream, id.SourceStreamId.Value);

}

public abstract class StreamProcessorStateRepositoryBase<TCollection, TDocument>
    where TCollection : IMongoCollection<TDocument>
{
    // TODO: Do common things here
}
