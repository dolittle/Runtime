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

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents an implementation of <see cref="IFetchEventsFromStreams" />.
    /// </summary>
    public class EventsFromEventHorizonFetcher : IFetchEventsFromStreams
    {
        readonly EventHorizon _eventHorizon;
        readonly AsyncServerStreamingCall<EventHorizonEvent> _call;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsFromEventHorizonFetcher"/> class.
        /// </summary>
        /// <param name="eventHorizon">The <see cref="EventHorizon" />.</param>
        /// <param name="call">The <see cref="AsyncServerStreamingCall{TResponse}" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventsFromEventHorizonFetcher(
            EventHorizon eventHorizon,
            AsyncServerStreamingCall<EventHorizonEvent> call,
            ILogger logger)
        {
            _eventHorizon = eventHorizon;
            _call = call;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<StreamEvent> Fetch(ScopeId scopeId, StreamId streamId, StreamPosition streamPosition, CancellationToken cancellationToken)
        {
            if (!await _call.ResponseStream.MoveNext(cancellationToken).ConfigureAwait(false)) throw new NoEventInStreamAtPosition(scopeId, streamId, streamPosition);
            var @event = _call.ResponseStream.Current;
            return new StreamEvent(
                @event.ToCommittedEvent(_eventHorizon.ProducerMicroservice, _eventHorizon.ConsumerTenant),
                @event.StreamSequenceNumber,
                streamId,
                PartitionId.NotSet);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<StreamEvent>> FetchRange(ScopeId scopeId, StreamId streamId, StreamPositionRange range, CancellationToken cancellationToken = default)
        {
            throw new CannotFetchRangeOfEventsFromEventHorizon();
        }

        /// <inheritdoc/>
        public Task<StreamPosition> FindNext(ScopeId scopeId, StreamId streamId, PartitionId partitionId, StreamPosition fromPosition, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<StreamPosition>(uint.MaxValue);
        }
    }
}