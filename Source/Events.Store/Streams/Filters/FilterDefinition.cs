// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Concepts;

namespace Dolittle.Runtime.Events.Store.Streams.Filters
{
    /// <summary>
    /// Represents a <see cref="IFilterDefinition" /> for a remote filter.
    /// </summary>
    public class FilterDefinition : Value<FilterDefinition>, IFilterDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterDefinition"/> class.
        /// </summary>
        /// <param name="sourceStream">The source <see cref="StreamId" />.</param>
        /// <param name="targetStream">The target <see cref="StreamId" />.</param>
        /// <param name="partitioned">Whether the filter is partitioned or not.</param>
        public FilterDefinition(StreamId sourceStream, StreamId targetStream, bool partitioned)
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

        /// <inheritdoc/>
        public bool Public => false;
    }
}