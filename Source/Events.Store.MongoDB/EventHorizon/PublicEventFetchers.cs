// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.EventHorizon.Producer;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.EventHorizon
{
    /// <summary>
    /// Represents an implementation of <see cref="IPublicEventFetchers" />.
    /// </summary>
    public class PublicEventFetchers : IPublicEventFetchers
    {
        readonly EventStoreConnection _connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicEventFetchers"/> class.
        /// </summary>
        /// <param name="connection">The <see cref="EventStoreConnection" />.</param>
        public PublicEventFetchers(EventStoreConnection connection)
        {
            _connection = connection;
        }

        /// <inheritdoc/>
        public async Task<ICanFetchEventsFromPublicStreams> GetFetcherFor(StreamId publicStreamId, CancellationToken cancellationToken)
        {
            var collection = await _connection.GetPublicStreamCollection(publicStreamId, cancellationToken).ConfigureAwait(false);
            return new EventsFromPublicStreamsFetcher(collection, publicStreamId);
        }
    }
}
