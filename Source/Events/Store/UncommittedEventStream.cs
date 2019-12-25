// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.Execution;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Represents a stream of uncommitted events.
    /// </summary>
    public class UncommittedEventStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UncommittedEventStream"/> class.
        /// </summary>
        /// <param name="id">The unique id in the form of a <see cref="CommitId" />.</param>
        /// <param name="correlationId">A <see cref="CorrelationId" /> used to relate this event stream to other actions in the system.</param>
        /// <param name="source">The <see cref="VersionedEventSource" /> that this stream applies to.</param>
        /// <param name="timestamp">A timestamp in the form of a <see cref="DateTimeOffset" /> representing when the stream was committed.</param>
        /// <param name="events">An <see cref="EventStream" /> representing the events (an enumerable of <see cref="EventEnvelope" />) that are committed in this commit.</param>
        public UncommittedEventStream(CommitId id, CorrelationId correlationId, VersionedEventSource source, DateTimeOffset timestamp, EventStream events)
        {
            CorrelationId = correlationId;
            Ensure.IsNotNull(nameof(id), id);
            Ensure.IsNotNull(nameof(source), source);
            Ensure.IsNotNull(nameof(events), events);

            Id = id;
            Timestamp = timestamp;
            Source = source;
            CorrelationId = correlationId;
            Events = events;
        }

        /// <summary>
        /// Gets the unique id in the form of a <see cref="CommitId" />.
        /// </summary>
        public CommitId Id { get; }

        /// <summary>
        /// Gets a <see cref="CorrelationId" /> used to relate this event stream to other actions in the system.
        /// </summary>
        public CorrelationId CorrelationId { get; }

        /// <summary>
        /// Gets the <see cref="VersionedEventSource" /> that this stream applies to.
        /// </summary>
        public VersionedEventSource Source { get; }

        /// <summary>
        /// Gets a timestamp in the form of a <see cref="DateTimeOffset" /> representing when the stream was generated.
        /// </summary>
        public DateTimeOffset Timestamp { get; }

        /// <summary>
        /// Gets an <see cref="EventStream" /> representing the events that are committed in this commit.
        /// </summary>
        public EventStream Events { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Id} {CorrelationId} {Source} {Timestamp} {Events.Count()}";
        }
    }
}