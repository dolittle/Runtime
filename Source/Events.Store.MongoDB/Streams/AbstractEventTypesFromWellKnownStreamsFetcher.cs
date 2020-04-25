// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Artifacts;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Represents an abstract implementation of <see cref="ICanFetchEventsFromWellKnownStreams" />.
    /// </summary>
    public abstract class AbstractEventTypesFromWellKnownStreamsFetcher : ICanFetchEventTypesFromWellKnownStreams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractEventTypesFromWellKnownStreamsFetcher"/> class.
        /// </summary>
        /// <param name="streams">The streams it can fetch from.</param>
        protected AbstractEventTypesFromWellKnownStreamsFetcher(IEnumerable<StreamId> streams) => WellKnownStreams = streams;

        /// <inheritdoc/>
        public IEnumerable<StreamId> WellKnownStreams { get; }

        /// <inheritdoc/>
        public bool CanFetchFromStream(StreamId stream) => WellKnownStreams.Contains(stream);

        /// <inheritdoc/>
        public abstract Task<IEnumerable<Artifact>> FetchInRange(ScopeId scope, StreamId stream, StreamPositionRange range, CancellationToken cancellationToken);

        /// <inheritdoc/>
        public abstract Task<IEnumerable<Artifact>> FetchInRangeAndPartition(ScopeId scope, StreamId stream, PartitionId partition, StreamPositionRange range, CancellationToken cancellationToken);
    }
}
