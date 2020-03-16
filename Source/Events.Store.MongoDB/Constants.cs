// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Applications;
using Dolittle.Runtime.Events.Streams;

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
        /// Gets the collection name where public events are stored.
        /// </summary>
        public const string PublicEventsCollection = "public-events";

        /// <summary>
        /// Gets the collection name for a stream.
        /// </summary>
        /// <param name="streamId">The <see cref="StreamId" />.</param>
        /// <returns>The event stream collection name.</returns>
        public static string CollectionNameForStream(StreamId streamId) => $"stream-{streamId.Value}";

        /// <summary>
        /// Gets the collection name for a stream.
        /// </summary>
        /// <param name="microservice">The <see cref="Microservice" />.</param>
        /// <returns>The event stream collection name.</returns>
        public static string CollectionNameForReceivedEvents(Microservice microservice) => $"event-Horizon-{microservice.Value}";
    }
}