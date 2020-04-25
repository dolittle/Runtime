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
        /// Initializes a new instance of the <see cref="StreamDefinition"/> class.
        /// </summary>
        /// <param name="filterDefinition">The <see cref="IFilterDefinition" />.</param>
        /// <param name="isPublic">Whether the stream is public or not.</param>
        public StreamDefinition(IFilterDefinition filterDefinition, bool isPublic)
        {
            FilterDefinition = filterDefinition;
            Public = isPublic;
        }

        /// <summary>
        /// Gets the <see cref="StreamDefinition" /> which represents the definition of the Event Log.
        /// </summary>
        public static StreamDefinition EventLog => new StreamDefinition(new RemoteFilterDefinition(StreamId.AllStreamId, StreamId.AllStreamId, false), false);

        /// <summary>
        /// Gets the <see cref="IFilterDefinition" /> that defines this stream.
        /// </summary>
        public IFilterDefinition FilterDefinition { get; }

        /// <summary>
        /// Gets a value indicating whether this is a public stream.
        /// </summary>
        public bool Public { get; }

        /// <summary>
        /// Gets the <see cref="StreamId" />.
        /// </summary>
        public StreamId StreamId => FilterDefinition.TargetStream;

        /// <summary>
        /// Gets a value indicating whether the stream is partitioned.
        /// </summary>
        public bool Partitioned => FilterDefinition.Partitioned;
    }
}
