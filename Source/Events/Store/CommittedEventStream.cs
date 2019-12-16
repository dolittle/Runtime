// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Events;
using Dolittle.Execution;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// A stream of events for a specific <see cref="IEventSource" /> that represent an atomic commit.
    /// </summary>
    public class CommittedEventStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommittedEventStream"/> class.
        /// </summary>
        /// <param name="sequence">The <see cref="CommitSequenceNumber" /> for this committed event stream.</param>
        /// <param name="source">The <see cref="VersionedEventSource" /> that this stream applies to.</param>
        /// <param name="id">The unique id in the form of a <see cref="CommitId" />.</param>
        /// <param name="correlationId">A <see cref="CorrelationId" /> used to relate this event stream to other actions in the system.</param>
        /// <param name="timestamp">A timestamp in the form of a <see cref="DateTimeOffset" /> representing when the stream was committed.</param>
        /// <param name="events">An enumerable of <see cref="EventEnvelope" /> representing the events that are committed in this commit.</param>
        public CommittedEventStream(CommitSequenceNumber sequence, VersionedEventSource source, CommitId id, CorrelationId correlationId, DateTimeOffset timestamp, EventStream events)
        {
            Sequence = sequence;
            Source = source;
            Id = id;
            Timestamp = timestamp;
            CorrelationId = correlationId;
            Events = events;
            LastEventVersion = source.ToCommittedEventVersion(sequence);
        }

        /// <summary>
        /// Gets the <see cref="CommitSequenceNumber" /> for this committed event stream.
        /// </summary>
        public CommitSequenceNumber Sequence { get; }

        /// <summary>
        /// Gets the <see cref="CommitSequenceNumber" /> for this committed event stream.
        /// </summary>
        public VersionedEventSource Source { get; }

        /// <summary>
        /// Gets the unique id in the form of a <see cref="CommitId" />.
        /// </summary>
        public CommitId Id { get; }

        /// <summary>
        /// Gets a timestamp in the form of a <see cref="DateTimeOffset" /> representing when the strwam was committed.
        /// </summary>
        public DateTimeOffset Timestamp { get; }

        /// <summary>
        /// Gets an <see cref="EventStream" /> comprising an enumerable of <see cref="EventEnvelope" /> representing the events that are committed in this commit.
        /// </summary>
        public EventStream Events { get; }

        /// <summary>
        /// Gets a <see cref="CorrelationId" /> used to relate this event stream to other actions in the system.
        /// </summary>
        public CorrelationId CorrelationId { get; }

        /// <summary>
        /// Gets the <see cref="CommittedEventVersion">Version</see> of the last event in this commit.
        /// </summary>
        public CommittedEventVersion LastEventVersion { get; }
    }
}