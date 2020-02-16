// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    /// <summary>
    /// A collection of configuration constants used for the MongoDB event store implementation.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The collection where stream processor states are stored.
        /// </summary>
        public static readonly string StreamProcessorStateCollection = "stream-processor-states";

        /// <summary>
        /// The collection where aggregate root instances are stored.
        /// These documents are used to ensure optimistic concurrency transaction boundaries.
        /// </summary>
        public static readonly string AggregateRootInstanceCollection = "aggregates";

        /// <summary>
        /// Gets the collection where events in the event log are stored.
        /// </summary>
        public static string AllStreamCollection => CollectionNameForStream(StreamId.AllStreamId);

        /// <summary>
        /// Gets the collection name for a stream.
        /// </summary>
        /// <param name="streamId">The <see cref="StreamId" />.</param>
        /// <returns>The event stream collection name.</returns>
        public static string CollectionNameForStream(StreamId streamId) => $"stream-{streamId.Value}";
    }
}