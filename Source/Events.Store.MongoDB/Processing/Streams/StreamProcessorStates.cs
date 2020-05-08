// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Async;
using Dolittle.Runtime.EventHorizon;
using Dolittle.Runtime.Events.Processing.Streams;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="IStreamProcessorStateRepository" />.
    /// </summary>
    public class StreamProcessorStates : IStreamProcessorStateRepository
    {
        readonly FilterDefinitionBuilder<AbstractStreamProcessorState> _streamProcessorFilter;
        readonly FilterDefinitionBuilder<AbstractSubscriptionState> _subscriptionFilter;
        readonly EventStoreConnection _connection;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorStates"/> class.
        /// </summary>
        /// <param name="connection">An <see cref="EventStoreConnection"/> to a MongoDB EventStore.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public StreamProcessorStates(EventStoreConnection connection, ILogger logger)
        {
            _streamProcessorFilter = Builders<AbstractStreamProcessorState>.Filter;
            _subscriptionFilter = Builders<AbstractSubscriptionState>.Filter;
            _connection = connection;
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
        public async Task<Try<IStreamProcessorState>> TryGetFor(IStreamProcessorId id, CancellationToken cancellationToken)
        {
            try
            {
                if (id is SubscriptionId subscriptionId)
                {
                    var states = await _connection.GetSubscriptionStateCollection(subscriptionId.ScopeId, cancellationToken).ConfigureAwait(false);
                    var persistedState = await states.Find(CreateFilter(subscriptionId))
                        .FirstOrDefaultAsync(cancellationToken)
                        .ConfigureAwait(false);
                    return (persistedState != null) ? (true, persistedState.ToRuntimeRepresentation()) : (false, null);
                }
                else if (id is StreamProcessorId streamProcessorId)
                {
                    var states = await _connection.GetStreamProcessorStateCollection(streamProcessorId.ScopeId, cancellationToken).ConfigureAwait(false);
                    var persistedState = await states.Find(CreateFilter(streamProcessorId))
                        .FirstOrDefaultAsync(cancellationToken)
                        .ConfigureAwait(false);
                    return (persistedState != null) ? (true, persistedState.ToRuntimeRepresentation()) : (false, null);
                }
                else
                {
                    throw new StreamProcessorIdOfUnsupportedType(id);
                }
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        /// <summary>
        /// Persist the <see cref="IStreamProcessorState" /> for <see cref="StreamProcessorId"/> and <see cref="SubscriptionId"/>.
        /// Handles <see cref="Partitioned.PartitionedStreamProcessorState"/> separately also.
        /// IsUpsert option creates the document if one isn't found.
        /// </summary>
        /// <param name="id">The <see cref="StreamProcessorId" />.</param>
        /// <param name="baseStreamProcessorState">The <see cref="IStreamProcessorState" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
        public async Task Persist(IStreamProcessorId id, IStreamProcessorState baseStreamProcessorState, CancellationToken cancellationToken)
        {
            try
            {
                if (id is SubscriptionId subscriptionId)
                {
                    if (baseStreamProcessorState is StreamProcessorState streamProcessorState)
                    {
                        var replacementState = new SubscriptionState(
                            subscriptionId.ConsumerTenantId,
                            subscriptionId.ProducerMicroserviceId,
                            subscriptionId.ProducerTenantId,
                            subscriptionId.ScopeId,
                            subscriptionId.StreamId,
                            subscriptionId.PartitionId,
                            streamProcessorState.Position,
                            streamProcessorState.RetryTime,
                            streamProcessorState.FailureReason,
                            streamProcessorState.ProcessingAttempts,
                            streamProcessorState.LastSuccessfullyProcessed);
                        var states = await _connection.GetSubscriptionStateCollection(replacementState.ScopeId, cancellationToken).ConfigureAwait(false);
                        var persistedState = await states.ReplaceOneAsync(
                            CreateFilter(subscriptionId),
                            replacementState,
                            new ReplaceOptions { IsUpsert = true })
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        throw new UnsupportedStreamProcessorStatewithSubscriptionId(subscriptionId, baseStreamProcessorState);
                    }
                }
                else if (baseStreamProcessorState is Runtime.Events.Processing.Streams.Partitioned.StreamProcessorState partitionedStreamProcessorState)
                {
                    var streamProcessorId = id as StreamProcessorId;
                    var states = await _connection.GetStreamProcessorStateCollection(streamProcessorId.ScopeId, cancellationToken).ConfigureAwait(false);
                    var state = await states.ReplaceOneAsync(
                        CreateFilter(streamProcessorId),
                        new Partitioned.PartitionedStreamProcessorState(
                            streamProcessorId.ScopeId,
                            streamProcessorId.EventProcessorId,
                            streamProcessorId.SourceStreamId,
                            partitionedStreamProcessorState.Position,
                            partitionedStreamProcessorState.FailingPartitions.ToDictionary(
                                kvp => kvp.Key.Value.ToString(),
                                kvp => new FailingPartitionState(kvp.Value.Position, kvp.Value.RetryTime, kvp.Value.Reason, kvp.Value.ProcessingAttempts, kvp.Value.LastFailed)),
                            partitionedStreamProcessorState.LastSuccessfullyProcessed),
                        new ReplaceOptions { IsUpsert = true })
                        .ConfigureAwait(false);
                }
                else if (baseStreamProcessorState is Runtime.Events.Processing.Streams.StreamProcessorState streamProcessorState)
                {
                    var streamProcessorId = id as StreamProcessorId;
                    var states = await _connection.GetStreamProcessorStateCollection(streamProcessorId.ScopeId, cancellationToken).ConfigureAwait(false);
                    var state = await states.ReplaceOneAsync(
                        CreateFilter(streamProcessorId),
                        new StreamProcessorState(
                            streamProcessorId.ScopeId,
                            streamProcessorId.EventProcessorId,
                            streamProcessorId.SourceStreamId,
                            streamProcessorState.Position,
                            streamProcessorState.RetryTime,
                            streamProcessorState.FailureReason,
                            streamProcessorState.ProcessingAttempts,
                            streamProcessorState.LastSuccessfullyProcessed),
                        new ReplaceOptions { IsUpsert = true })
                        .ConfigureAwait(false);
                }
                else
                {
                    throw new StreamProcessorStateOfUnsupportedType(id, baseStreamProcessorState);
                }
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        FilterDefinition<AbstractStreamProcessorState> CreateFilter(StreamProcessorId id) =>
            _streamProcessorFilter.Eq(_ => _.EventProcessorId, id.EventProcessorId.Value)
                & _streamProcessorFilter.Eq(_ => _.ScopeId, id.ScopeId.Value)
                & _streamProcessorFilter.Eq(_ => _.SourceStreamId, id.SourceStreamId.Value);

        FilterDefinition<AbstractSubscriptionState> CreateFilter(SubscriptionId id) =>
            _subscriptionFilter.Eq(_ => _.ConsumerTenantId, id.ConsumerTenantId.Value)
                & _subscriptionFilter.Eq(_ => _.ProducerMicroserviceId, id.ProducerMicroserviceId.Value)
                & _subscriptionFilter.Eq(_ => _.ProducerTenantId, id.ProducerTenantId.Value)
                & _subscriptionFilter.Eq(_ => _.ScopeId, id.ScopeId.Value)
                & _subscriptionFilter.Eq(_ => _.StreamId, id.StreamId.Value)
                & _subscriptionFilter.Eq(_ => _.PartitionId, id.PartitionId.Value);
    }
}
