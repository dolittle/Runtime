// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents an abstract implementation  of <see cref="IFilterDefinition" /> for a well-known filter.
    /// </summary>
    public abstract class WellKnownStreamFilterDefinition : IFilterDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WellKnownStreamFilterDefinition"/> class.
        /// </summary>
        /// <param name="sourceStream">The source <see cref="StreamId" />.</param>
        /// <param name="targetStream">The target <see cref="StreamId" />.</param>
        /// <param name="streamName">The name of the stream.</param>
        protected WellKnownStreamFilterDefinition(StreamId sourceStream, StreamId targetStream, string streamName)
        {
            SourceStream = sourceStream;
            TargetStream = targetStream;
            StreamName = streamName;
        }

        /// <summary>
        /// Gets the name of the well-known stream.
        /// </summary>
        public string StreamName { get; }

        /// <inheritdoc/>
        public StreamId SourceStream { get; }

        /// <inheritdoc/>
        public StreamId TargetStream { get; }
    }
}