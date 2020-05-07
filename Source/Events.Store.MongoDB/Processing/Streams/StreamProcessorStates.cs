// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
                    var persistedState = await states.Find(
                        _subscriptionFilter.Eq(_ => _.ConsumerTenantId, subscriptionId.ConsumerTenantId.Value)
                            & _subscriptionFilter.Eq(_ => _.ProducerMicroserviceId, subscriptionId.ProducerMicroserviceId.Value)
                            & _subscriptionFilter.Eq(_ => _.ProducerTenantId, subscriptionId.ProducerTenantId.Value)
                            & _subscriptionFilter.Eq(_ => _.ScopeId, subscriptionId.ScopeId.Value)
                            & _subscriptionFilter.Eq(_ => _.StreamId, subscriptionId.StreamId.Value)
                            & _subscriptionFilter.Eq(_ => _.PartitionId, subscriptionId.PartitionId.Value))
                        .FirstOrDefaultAsync(cancellationToken)
                        .ConfigureAwait(false);
                    return (persistedState != null) ? (true, persistedState.ToRuntimeRepresentation()) : (false, null);
                }
                else
                {
                    var streamProcessorId = id as StreamProcessorId;
                    var states = await _connection.GetStreamProcessorStateCollection(streamProcessorId.ScopeId, cancellationToken).ConfigureAwait(false);
                    var persistedState = await states.Find(
                        _streamProcessorFilter.Eq(_ => _.ScopeId, streamProcessorId.ScopeId.Value)
                            & _streamProcessorFilter.Eq(_ => _.EventProcessorId, streamProcessorId.EventProcessorId.Value)
                            & _streamProcessorFilter.Eq(_ => _.SourceStreamId, streamProcessorId.SourceStreamId.Value))
                        .FirstOrDefaultAsync(cancellationToken)
                        .ConfigureAwait(false);
                    return (persistedState != null) ? (true, persistedState.ToRuntimeRepresentation()) : (false, null);
                }
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        /// <inheritdoc/>
        public async Task Persist(IStreamProcessorId streamProcessorId, IStreamProcessorState streamProcessorState, CancellationToken cancellationToken)
        {
            try
            {
                // TODO: There are now two different collections for stream proecssor states. One for StreamProcessorId (Events.Processing) and one for SubscriptionId (EventHorizon)
                var states = await _connection.GetStreamProcessorStateCollection(streamProcessorId.ScopeId, cancellationToken).ConfigureAwait(false);
                var state = await states.Find(
                    _streamProcessorFilter.Eq(_ => _.ScopeId, streamProcessorId.ScopeId.Value)
                        & _streamProcessorFilter.Eq(_ => _.EventProcessorId, streamProcessorId.EventProcessorId.Value)
                        & _streamProcessorFilter.Eq(_ => _.SourceStreamId, streamProcessorId.SourceStreamId.Value))
                    .FirstOrDefaultAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }
    }
}
