// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Nito.AsyncEx;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents an implementation of <see cref="IFetchEventsFromStreams" />.
    /// </summary>
    public class EventsFromEventHorizonFetcher : IFetchEventsFromStreams
    {
        readonly AsyncProducerConsumerQueue<StreamEvent> _events;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsFromEventHorizonFetcher"/> class.
        /// </summary>
        /// <param name="events">The <see cref="AsyncProducerConsumerQueue{TResponse}" />.</param>
        public EventsFromEventHorizonFetcher(AsyncProducerConsumerQueue<StreamEvent> events)
        {
            _events = events;
        }

        /// <inheritdoc/>
        public Task<StreamEvent> Fetch(ScopeId scopeId, StreamId streamId, StreamPosition streamPosition, CancellationToken cancellationToken) =>
            _events.DequeueAsync(cancellationToken);

        /// <inheritdoc/>
        public Task<IEnumerable<StreamEvent>> FetchRange(ScopeId scopeId, StreamId streamId, StreamPositionRange range, CancellationToken cancellationToken) =>
            throw new CannotFetchRangeOfEventsFromEventHorizon();

        /// <inheritdoc/>
        public Task<StreamPosition> FindNext(ScopeId scopeId, StreamId streamId, PartitionId partitionId, StreamPosition fromPosition, CancellationToken cancellationToken) =>
            Task.FromResult<StreamPosition>(uint.MaxValue);
    }
}