// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
extern alias contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using contracts::Dolittle.Runtime.EventHorizon;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Streams;
using Grpc.Core;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents an implementation of <see cref="IFetchEventsFromStreams" />.
    /// </summary>
    public class EventsFromEventHorizonFetcher : IFetchEventsFromStreams
    {
        readonly Action<EventHorizonEvent> _validateEvent;
        readonly AsyncServerStreamingCall<EventHorizonEvent> _call;
        readonly Action _onUnavailableConnection;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsFromEventHorizonFetcher"/> class.
        /// </summary>
        /// <param name="validateEvent">The <see cref="Action{T}" /> callback for whether an <see cref="EventHorizonEvent" /> is valid.</param>
        /// <param name="call">The call.</param>
        /// <param name="onUnavailableConnection">On no connection.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventsFromEventHorizonFetcher(
            Action<EventHorizonEvent> validateEvent,
            AsyncServerStreamingCall<EventHorizonEvent> call,
            Action onUnavailableConnection,
            ILogger logger)
        {
            _validateEvent = validateEvent;
            _call = call;
            _logger = logger;
            _onUnavailableConnection = onUnavailableConnection;
        }

        /// <inheritdoc/>
        public async Task<StreamEvent> Fetch(StreamId streamId, StreamPosition streamPosition, CancellationToken cancellationToken)
        {
            try
            {
                if (!await _call.ResponseStream.MoveNext(cancellationToken).ConfigureAwait(false)) throw new NoEventInStreamAtPosition(streamId, streamPosition);
                var @event = _call.ResponseStream.Current;
                _validateEvent(@event);
                return new StreamEvent(
                    @event.ToCommittedEvent(),
                    streamId,
                    PartitionId.NotSet);
            }
            catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.Unavailable)
            {
                _onUnavailableConnection();
                throw;
            }
        }

        /// <inheritdoc/>
        public Task<IEnumerable<StreamEvent>> FetchRange(StreamId streamId, StreamPositionRange range, CancellationToken cancellationToken = default)
        {
            throw new CannotFetchRangeOfEventsFromEventHorizon();
        }

        /// <inheritdoc/>
        public Task<StreamPosition> FindNext(StreamId streamId, PartitionId partitionId, StreamPosition fromPosition, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<StreamPosition>(uint.MaxValue);
        }
    }
}