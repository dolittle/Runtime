using System;
using System.Collections;
using System.Collections.Generic;
using Dolittle.Runtime.Events;
using Dolittle.Events;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// A stream of events for a specific <see cref="IEventSource" /> that represent an atomic commit
    /// </summary>
    public class CommittedEventStream 
    {
        /// <summary>
        /// Instantiates a new instance of a <see cref="CommittedEventStream" />
        /// </summary>
        /// <param name="sequence">The <see cref="CommitSequenceNumber" /> for this committed event stream</param>
        /// <param name="source">The <see cref="VersionedEventSource" /> that this stream applies to</param>
        /// <param name="id">The unique id in the form of a <see cref="CommitId" /></param>
        /// <param name="correlationId">A <see cref="CorrelationId" /> used to relate this event stream to other actions in the system</param>
        /// <param name="timestamp">A timestamp in the form of a <see cref="DateTimeOffset" /> representing when the strwam was committed</param>
        /// <param name="events">An enumerable of <see cref="EventEnvelope" /> representing the events that are committed in this commit</param>
        public CommittedEventStream(CommitSequenceNumber sequence, VersionedEventSource source, CommitId id, CorrelationId correlationId, DateTimeOffset timestamp, EventStream events)
        {
            Sequence = sequence;
            Source = source;
            Id = id;
            Timestamp = timestamp;
            CorrelationId = correlationId;
            Events = events;

        }
        /// <summary>
        /// The <see cref="CommitSequenceNumber" /> for this committed event stream
        /// </summary>
        /// <value></value>
        public CommitSequenceNumber Sequence { get; }

        /// <summary>
        /// The <see cref="CommitSequenceNumber" /> for this committed event stream
        /// </summary>
        /// <value></value>
        public VersionedEventSource Source { get; }
        /// <summary>
        /// The unique id in the form of a <see cref="CommitId" />
        /// </summary>
        /// <value></value>
        public CommitId Id { get; }
        /// <summary>
        /// A timestamp in the form of a <see cref="DateTimeOffset" /> representing when the strwam was committed
        /// </summary>
        /// <value></value>
        public DateTimeOffset Timestamp { get; }
        /// <summary>
        /// An <see cref="EventStream" /> comprising an enumerable of <see cref="EventEnvelope" /> representing the events that are committed in this commit
        /// </summary>
        /// <value></value>
        public EventStream Events { get; }
        /// <summary>
        /// A <see cref="CorrelationId" /> used to relate this event stream to other actions in the system
        /// </summary>
        /// <value></value>
        public CorrelationId CorrelationId { get; }
    }
}