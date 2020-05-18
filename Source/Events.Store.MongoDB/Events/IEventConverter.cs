// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams;
using mongoDB = Dolittle.Runtime.Events.Store.MongoDB.Events;
using runtime = Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events
{
    /// <summary>
    /// Defines a system that can convert between representations of events.
    /// </summary>
    public interface IEventConverter
    {
        /// <summary>
        /// Converts the <see cref="CommittedExternalEvent" /> to a <see cref="mongoDB.Event" /> for a Scoped Event Log.
        /// </summary>
        /// <param name="committedEvent">The <see cref="CommittedExternalEvent" />.</param>
        /// <returns>The converted <see cref="mongoDB.Event" />.</returns>
        mongoDB.Event ToEventLogEvent(CommittedExternalEvent committedEvent);

        /// <summary>
        /// Converts a <see cref="CommittedEvent" /> to <see cref="mongoDB.StreamEvent" />.
        /// </summary>
        /// <param name="committedEvent">The <see cref="CommittedEvent" />.</param>
        /// <param name="streamPosition">The <see cref="StreamPosition" />.</param>
        /// <param name="partition">The <see cref="PartitionId" />.</param>
        /// <returns>The converted <see cref="mongoDB.StreamEvent" />.</returns>
        mongoDB.StreamEvent ToStoreStreamEvent(CommittedEvent committedEvent, StreamPosition streamPosition, PartitionId partition);

        /// <summary>
        /// Converts a <see cref="mongoDB.Event" /> to <see cref="runtime.Streams.StreamEvent" />.
        /// </summary>
        /// <param name="event">The <see cref="mongoDB.Event" />.</param>
        /// <returns>The converted <see cref="runtime.Streams.StreamEvent" />.</returns>
        runtime.Streams.StreamEvent ToRuntimeStreamEvent(mongoDB.Event @event);

        /// <summary>
        /// Converts a <see cref="mongoDB.StreamEvent" /> to <see cref="runtime.Streams.StreamEvent" />.
        /// </summary>
        /// <param name="event">The <see cref="mongoDB.StreamEvent" />.</param>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        /// <param name="partitioned">Whether or not the Event is partitioned.</param>
        /// <returns>The converted <see cref="runtime.Streams.StreamEvent" />.</returns>
        runtime.Streams.StreamEvent ToRuntimeStreamEvent(mongoDB.StreamEvent @event, StreamId stream, bool partitioned);
    }
}
