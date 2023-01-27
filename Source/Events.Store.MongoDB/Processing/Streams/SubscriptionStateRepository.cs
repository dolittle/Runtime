// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams.EventHorizon;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Dolittle.Runtime.EventHorizon.Consumer;
using Dolittle.Runtime.Events.Store.MongoDB.Legacy;
using MongoSubscriptionState = Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams.EventHorizon.SubscriptionState;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.MongoDB;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams;

/// <summary>
/// Represents an implementation of <see cref="Store.Streams.IStreamProcessorStates" />.
/// </summary>
public class SubscriptionStateRepository : ISubscriptionStateRepository
{
    readonly FilterDefinitionBuilder<MongoSubscriptionState> _subscriptionFilter;
    readonly ISubscriptionStateCollections _subscriptionStateCollections;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamProcessorStateRepository"/> class.
    /// </summary>
    /// <param name="subscriptionStateCollections">The <see cref="ISubscriptionStateCollections" />.</param>
    /// <param name="logger">An <see cref="ILogger"/>.</param>
    public SubscriptionStateRepository(
        ISubscriptionStateCollections subscriptionStateCollections,
        ILogger logger)
    {
        _subscriptionStateCollections = subscriptionStateCollections;
        _subscriptionFilter = Builders<MongoSubscriptionState>.Filter;
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
    public async Task<Try<Runtime.Events.Processing.Streams.StreamProcessorState>> TryGet(SubscriptionId id, CancellationToken cancellationToken)
    {
        _logger.GettingStreamProcessorState(id);
        try
        {
            var states = await _subscriptionStateCollections.Get(id.ScopeId, cancellationToken).ConfigureAwait(false);
            var persistedState = await states.Find(CreateFilter(id))
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            return persistedState == default
                ? new StreamProcessorStateDoesNotExist(id)
                : persistedState.ToRuntimeRepresentation();
        }
        catch (MongoWaitQueueFullException ex)
        {
            return new EventStoreUnavailable("Mongo wait queue is full", ex);
        }
    }

    public IAsyncEnumerable<StreamProcessorStateWithId<SubscriptionId, Runtime.Events.Processing.Streams.StreamProcessorState>> GetForScope(ScopeId scope, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
        // _logger.GettingAllStreamProcessorState();
        // var stateCollection = await _subscriptionStateCollections.Get(scope, cancellationToken).ConfigureAwait(false);
        // var states = stateCollection
        //     .Find(_subscriptionFilter.Empty)
        //     .ToAsyncEnumerable(cancellationToken: cancellationToken)
        //     .Select(document => new StreamProcessorStateWithId<SubscriptionId, Runtime.Events.Processing.Streams.StreamProcessorState>(new StreamProcessorId(ScopeId.Default, document.EventProcessor, document.SourceStream),
        //         document.ToRuntimeRepresentation()));
        // await foreach (var state in states.WithCancellation(cancellationToken))
        // {
        //     yield return state;
        // }
    }

    public async Task<Try> PersistForScope(ScopeId scopeId, IReadOnlyDictionary<SubscriptionId, Runtime.Events.Processing.Streams.StreamProcessorState> streamProcessorStates, CancellationToken cancellationToken)
    {
        // var tasksWithIds = streamProcessorStates.Select(_ => (_.Key, Persist(_.Key, _.Value, cancellationToken)));
        // await Task.WhenAll(tasksWithIds.Select(_ => _.Item2)).ConfigureAwait(false);
        return Try.Succeeded;
    }


    // async Task<(IStreamProcessorId, Partial)> Persist(IStreamProcessorId id, IStreamProcessorState baseStreamProcessorState,
    //     CancellationToken cancellationToken)
    // {
    //     _logger.PersistingStreamProcessorState(id);
    //     try
    //     {
    //         if (id is SubscriptionId subscriptionId)
    //         {
    //             if (baseStreamProcessorState is Runtime.Events.Processing.Streams.StreamProcessorState streamProcessorState)
    //             {
    //                 var replacementState = new MongoSubscriptionState(
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

    FilterDefinition<MongoSubscriptionState> CreateFilter(SubscriptionId id) =>
        _subscriptionFilter.Eq(_ => _.Microservice, id.ProducerMicroserviceId.Value)
        & _subscriptionFilter.Eq(_ => _.Tenant, id.ProducerTenantId.Value)
        & _subscriptionFilter.Eq(_ => _.Stream, id.StreamId.Value)
        & _subscriptionFilter.EqStringOrGuid(_ => _.Partition, id.PartitionId.Value);
}
