// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Represents the definition of a Stream.
    /// </summary>
    public class StreamDefinition
    {
        /// <summary>
        /// Gets the <see cref="StreamId" />.
        /// </summary>
        public StreamId StreamId { get; }

        /// <summary>
        /// Gets the <see cref="IFilterDefinition" /> that defines this stream.
        /// </summary>
        public IFilterDefinition FilterDefinition { get; }

        /// <summary>
        /// Gets a value indicating whether the stream is partitioned.
        /// </summary>
        public bool Partitioned { get; }

        /// <summary>
        /// Gets a value indicating whether this is a public stream.
        /// </summary>
        public bool Public { get; }
    }
}
