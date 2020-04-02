// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
extern alias contracts;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using contracts::Dolittle.Runtime.EventHorizon;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Grpc.Core;
using Nito.AsyncEx;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents an implementation of <see cref="IFetchEventsFromStreams" />.
    /// </summary>
    public class EventsFromEventHorizonFetcher : IFetchEventsFromStreams
    {
        readonly Subscription _subscription;
        readonly AsyncProducerConsumerQueue<EventHorizonEvent> _events;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsFromEventHorizonFetcher"/> class.
        /// </summary>
        /// <param name="eventHorizon">The <see cref="Subscription" />.</param>
        /// <param name="events">The <see cref="AsyncServerStreamingCall{TResponse}" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventsFromEventHorizonFetcher(
            Subscription eventHorizon,
            AsyncProducerConsumerQueue<EventHorizonEvent> events,
            ILogger logger)
        {
            _subscription = eventHorizon;
            _events = events;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<StreamEvent> Fetch(ScopeId scopeId, StreamId streamId, StreamPosition streamPosition, CancellationToken cancellationToken)
        {
            var @event = await _events.DequeueAsync(cancellationToken).ConfigureAwait(false);
            return new StreamEvent(
                @event.ToCommittedEvent(_subscription.ProducerMicroservice, _subscription.ConsumerTenant),
                @event.StreamSequenceNumber,
                streamId,
                PartitionId.NotSet);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<StreamEvent>> FetchRange(ScopeId scopeId, StreamId streamId, StreamPositionRange range, CancellationToken cancellationToken = default) =>
            throw new CannotFetchRangeOfEventsFromEventHorizon();

        /// <inheritdoc/>
        public Task<StreamPosition> FindNext(ScopeId scopeId, StreamId streamId, PartitionId partitionId, StreamPosition fromPosition, CancellationToken cancellationToken = default) =>
            Task.FromResult<StreamPosition>(uint.MaxValue);
    }
}