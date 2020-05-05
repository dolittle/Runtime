// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.EventHorizon.Producer;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.MongoDB.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.EventHorizon
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanFetchEventsFromPublicStreams" />.
    /// </summary>
    public class EventsFromPublicStreamsFetcher : ICanFetchEventsFromPublicStreams
    {
        readonly FilterDefinitionBuilder<Events.StreamEvent> _filter = Builders<Events.StreamEvent>.Filter;
        readonly IMongoCollection<Events.StreamEvent> _publicEvents;
        readonly StreamId _publicStreamId;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsFromPublicStreamsFetcher"/> class.
        /// </summary>
        /// <param name="publicEvents">The <see cref="IMongoCollection{T}" /> for the public event stream.</param>
        /// <param name="publicStreamId">The public <see cref="StreamId" />.</param>
        public EventsFromPublicStreamsFetcher(IMongoCollection<Events.StreamEvent> publicEvents, StreamId publicStreamId)
        {
            _publicEvents = publicEvents;
            _publicStreamId = publicStreamId;
        }

        /// <inheritdoc/>
        public Task<Store.Streams.StreamEvent> Fetch(StreamPosition streamPosition, CancellationToken cancellationToken) =>
            EventsFromStreamsFetcher.Fetch(
                _publicEvents,
                _filter,
                _ => _.StreamPosition,
                Builders<Events.StreamEvent>.Projection.Expression(_ => _.ToRuntimeStreamEvent(_publicStreamId)),
                streamPosition,
                cancellationToken);

        public Task<Store.Streams.StreamEvent> FetchInPartition(PartitionId partitionId, StreamPosition streamPosition, CancellationToken cancellationToken) =>
            EventsFromStreamsFetcher.FetchInPartition(
                _publicEvents,
                _filter,
                _ => _.StreamPosition,
                Builders<Events.StreamEvent>.Projection.Expression(_ => _.ToRuntimeStreamEvent(_publicStreamId)),
                streamPosition,
                cancellationToken);
    }
}
