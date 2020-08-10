// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams;
using Nito.AsyncEx;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanFetchEventsFromStream" />.
    /// </summary>
    public class EventsFromEventHorizonFetcher : ICanFetchEventsFromStream
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
        public Task<StreamEvent> Fetch(StreamPosition streamPosition, CancellationToken cancellationToken) =>
            _events.DequeueAsync(cancellationToken);
    }
}
