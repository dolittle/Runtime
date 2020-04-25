// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store.Streams.Filters.EventHorizon
{
    /// <summary>
    /// Represents an implementation of <see cref="IFilterDefinition" /> for a public filter.
    /// </summary>
    public class PublicFilterDefinition : IFilterDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublicFilterDefinition"/> class.
        /// </summary>
        /// <param name="sourceStream">The source <see cref="StreamId" />.</param>
        /// <param name="targetStream">The target <see cref="StreamId" />.</param>
        public PublicFilterDefinition(StreamId sourceStream, StreamId targetStream)
        {
            SourceStream = sourceStream;
            TargetStream = targetStream;
        }

        /// <inheritdoc/>
        public StreamId SourceStream { get; }

        /// <inheritdoc/>
        public StreamId TargetStream { get; }

        /// <inheritdoc/>
        public bool Partitioned => true;

        /// <inheritdoc/>
        public bool Public => true;
    }
}