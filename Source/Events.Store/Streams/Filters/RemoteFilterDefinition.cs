// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store.Streams.Filters
{
    /// <summary>
    /// Represents a <see cref="IFilterDefinition" /> for a remote filter.
    /// </summary>
    public class RemoteFilterDefinition : IPersistableFilterDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteFilterDefinition"/> class.
        /// </summary>
        /// <param name="sourceStream">The source <see cref="StreamId" />.</param>
        /// <param name="targetStream">The target <see cref="StreamId" />.</param>
        /// <param name="partitioned">Whether the filter is partitioned or not.</param>
        public RemoteFilterDefinition(StreamId sourceStream, StreamId targetStream, bool partitioned)
        {
            SourceStream = sourceStream;
            TargetStream = targetStream;
            Partitioned = partitioned;
        }

        /// <inheritdoc/>
        public StreamId SourceStream { get; }

        /// <inheritdoc/>
        public StreamId TargetStream { get; }

        /// <inheritdoc/>
        public bool Partitioned { get; }
    }
}