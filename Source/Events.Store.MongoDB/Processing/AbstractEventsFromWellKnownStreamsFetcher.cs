// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing
{
    /// <summary>
    /// Represents an abstract implementation of <see cref="ICanFetchEventsFromWellKnownStreams" />.
    /// </summary>
    public abstract class AbstractEventsFromWellKnownStreamsFetcher : ICanFetchEventsFromWellKnownStreams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractEventsFromWellKnownStreamsFetcher"/> class.
        /// </summary>
        /// <param name="streams">The streams it can fetch from.</param>
        protected AbstractEventsFromWellKnownStreamsFetcher(IEnumerable<StreamId> streams) => WellKnownStreams = streams;

        /// <inheritdoc/>
        public IEnumerable<StreamId> WellKnownStreams { get; }

        /// <inheritdoc/>
        public bool CanFetchFromStream(StreamId stream) => WellKnownStreams.Contains(stream);

        /// <inheritdoc/>
        public abstract Task<StreamEvent> Fetch(StreamId streamId, StreamPosition streamPosition, CancellationToken cancellationToken = default);

        /// <inheritdoc/>
        public abstract Task<StreamPosition> FindNext(StreamId streamId, PartitionId partitionId, StreamPosition fromPosition, CancellationToken cancellationToken = default);
    }
}