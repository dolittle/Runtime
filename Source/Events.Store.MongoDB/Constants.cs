// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    /// <summary>
    /// A collection of configuration constants used for the MongoDB event store implementation.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The collection where events in the event log are stored.
        /// </summary>
        public static readonly string EventLogCollection = "eventlog";

        /// <summary>
        /// The collection where events in streams are stored.
        /// </summary>
        public static readonly string StreamEventCollection = "stream_events";

        /// <summary>
        /// The collection where aggregate root instances are stored.
        /// These documents are used to ensure optimistic concurrency transaction boundaries.
        /// </summary>
        public static readonly string AggregateRootInstanceCollection = "aggregates";

        /// <summary>
        /// The collection where stream processor states are stored.
        /// </summary>
        public static readonly string StreamProcessorStateCollection = "stream_processor_states";
    }
}