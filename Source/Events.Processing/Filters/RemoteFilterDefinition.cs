// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents a <see cref="IFilterDefinition" /> for a remote filter.
    /// </summary>
    public class RemoteFilterDefinition : IFilterDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteFilterDefinition"/> class.
        /// </summary>
        /// <param name="sourceStream">The source <see cref="StreamId" />.</param>
        /// <param name="targetStream">The target <see cref="StreamId" />.</param>
        public RemoteFilterDefinition(StreamId sourceStream, StreamId targetStream)
        {
            SourceStream = sourceStream;
            TargetStream = targetStream;
        }

        /// <inheritdoc/>
        public StreamId SourceStream { get; }

        /// <inheritdoc/>
        public StreamId TargetStream { get; }
    }
}