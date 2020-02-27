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
    /// Represents an abstract implementation of <see cref="ICanWriteEventsToWellKnownStreams" />.
    /// </summary>
    public abstract class AbstractEventsToWellKnownStreamsWriter : ICanWriteEventsToWellKnownStreams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractEventsToWellKnownStreamsWriter"/> class.
        /// </summary>
        /// <param name="streams">The streams it can write to.</param>
        protected AbstractEventsToWellKnownStreamsWriter(IEnumerable<StreamId> streams) => WellKnownStreams = streams;

        /// <inheritdoc/>
        public IEnumerable<StreamId> WellKnownStreams { get; }

        /// <inheritdoc/>
        public bool CanWriteToStream(StreamId stream) => WellKnownStreams.Contains(stream);

        /// <inheritdoc/>
        public abstract Task Write(CommittedEvent @event, StreamId streamId, PartitionId partitionId, CancellationToken cancellationToken = default);
    }
}