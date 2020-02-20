// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
extern alias contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using contracts::Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Grpc.Core;

namespace Dolittle.Runtime.Events.Processing.EventHorizon
{
    /// <summary>
    /// Represents an implementation of <see cref="IFetchEventsFromStreams" />.
    /// </summary>
    public class EventsFromEventHorizonFetcher : IFetchEventsFromStreams
    {
        readonly StreamPosition _streamBeginning;
        readonly AsyncServerStreamingCall<EventHorizonPublisherToSubscriberResponse> _call;
        readonly IList<CommittedEvent> _events;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsFromEventHorizonFetcher"/> class.
        /// </summary>
        /// <param name="call">The call.</param>
        /// <param name="streamBeginning">The beginning position of the stream.</param>
        public EventsFromEventHorizonFetcher(
            AsyncServerStreamingCall<EventHorizonPublisherToSubscriberResponse> call,
            StreamPosition streamBeginning)
        {
            _call = call;
            _streamBeginning = streamBeginning;
            _events = new List<CommittedEvent>();
        }

        /// <inheritdoc/>
        public async Task<StreamEvent> Fetch(StreamId streamId, StreamPosition streamPosition, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!await _call.ResponseStream.MoveNext(cancellationToken).ConfigureAwait(false)) throw new EventStoreUnavailable("event horizon is down",  new Exception("Event horizon is down."));
                _events.Add(_call.ResponseStream.Current.Event.ToCommittedEvent());
                var index = (int)(streamPosition.Value - _streamBeginning.Value);
                if (index >= _events.Count) throw new NoEventInStreamAtPosition(streamId, streamPosition);
                return new StreamEvent(_events[index], streamId, PartitionId.NotSet);
            }
            catch (Exception ex)
            {
                throw new EventStoreUnavailable("Cannot get event from event horizon", ex);
            }
        }

        /// <inheritdoc/>
        public Task<StreamPosition> FindNext(StreamId streamId, PartitionId partitionId, StreamPosition fromPosition, CancellationToken cancellationToken = default)
        {
            var index = (int)(fromPosition.Value - _streamBeginning.Value);
            return Task.FromResult(index < 0 || index >= _events.Count ? new StreamPosition(uint.MaxValue) : new StreamPosition((uint)index));
        }
    }
}