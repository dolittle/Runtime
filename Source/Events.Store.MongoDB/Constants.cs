// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    /// <summary>
    /// A collection of configuration constants used for the MongoDB event store implementation.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The collection name where stream processor states are stored.
        /// </summary>
        public const string StreamProcessorStateCollection = "stream-processor-states";

        /// <summary>
        /// The collection name where aggregate root instances are stored.
        /// These documents are used to ensure optimistic concurrency transaction boundaries.
        /// </summary>
        public const string AggregateRootInstanceCollection = "aggregates";

        /// <summary>
        /// The collection name where type partition filters are stored.
        /// </summary>
        public const string TypePartitionFilterDefinitionCollection = "type-partition-filters";

        /// <summary>
        /// Gets the collection name where events in the event log are stored.
        /// </summary>
        public const string EventLogCollection = "event-log";

        /// <summary>
        /// Gets the collection name for a stream.
        /// </summary>
        /// <param name="streamId">The <see cref="StreamId" />.</param>
        /// <returns>The event stream collection name.</returns>
        public static string CollectionNameForStream(StreamId streamId) => $"stream-{streamId}";

        /// <summary>
        /// Gets the collection name for a public stream.
        /// </summary>
        /// <param name="streamId">The <see cref="StreamId" />.</param>
        /// <returns>The public event stream collection name.</returns>
        public static string CollectionNameForPublicStream(StreamId streamId) => $"public-stream-{streamId}";

        /// <summary>
        /// Gets the collection name for scoped event log.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <returns>The scoped event log stream collection name.</returns>
        public static string CollectionNameForScopedEventLog(ScopeId scope) => $"x-event-log-{scope}";

        /// <summary>
        /// Gets the collection name for scoped stream.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        /// <returns>The scoped stream collection name.</returns>
        public static string CollectionNameForScopedStream(ScopeId scope, StreamId stream) => $"x-stream-{scope}-{stream}";
    }
}