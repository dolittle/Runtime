// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
extern alias contracts;

using System;
using System.Threading;
using System.Threading.Tasks;
using contracts::Dolittle.Runtime.Events.Processing;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Grpc.Core;

namespace Dolittle.Runtime.EventHorizon
{
    /// <summary>
    /// Represents an implementation of <see cref="IFetchEventsFromStreams" />.
    /// </summary>
    public class EventsFromEventHorizonFetcher : IFetchEventsFromStreams
    {
        readonly AsyncServerStreamingCall<EventHorizonPublisherToSubscriberResponse> _call;
        readonly Action _onUnavailableConnection;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsFromEventHorizonFetcher"/> class.
        /// </summary>
        /// <param name="call">The call.</param>
        /// <param name="onUnavailableConnection">On no connection.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventsFromEventHorizonFetcher(
            AsyncServerStreamingCall<EventHorizonPublisherToSubscriberResponse> call,
            Action onUnavailableConnection,
            ILogger logger)
        {
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

                return new StreamEvent(
                    @_call.ResponseStream.Current.Event.ToCommittedEvent(),
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
        public Task<StreamPosition> FindNext(StreamId streamId, PartitionId partitionId, StreamPosition fromPosition, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<StreamPosition>(uint.MaxValue);
        }
    }
}