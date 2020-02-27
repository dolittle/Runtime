// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Represents an abstract implementation of <see cref="ICanGetMetadataFromWellKnownStreams" />.
    /// </summary>
    public abstract class AbstractMetadataFromWellKnownStreamsGetter : ICanGetMetadataFromWellKnownStreams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractMetadataFromWellKnownStreamsGetter"/> class.
        /// </summary>
        /// <param name="wellKnownStreams">The well-known <see cref="StreamId"/> streams.</param>
        protected AbstractMetadataFromWellKnownStreamsGetter(IEnumerable<StreamId> wellKnownStreams) => WellKnownStreams = wellKnownStreams;

        /// <inheritdoc/>
        public IEnumerable<StreamId> WellKnownStreams { get; }

        /// <inheritdoc/>
        public bool CanGetMetadataFromStream(StreamId stream) => WellKnownStreams.Contains(stream);

        /// <inheritdoc/>
        public abstract Task<EventLogSequenceNumber> GetLastProcessedEventLogSequenceNumber(StreamId stream, CancellationToken cancellationToken = default);
    }
}