using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Concepts;
using Dolittle.Runtime.Events;
using Dolittle.Events;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// 
    /// </summary>
    public class UncommittedEventStream
    {
        /// <summary>
        /// A stream of events for a specific <see cref="EventSource" /> that represent an atomic commit
        /// </summary>
        /// <param name="id">The unique id in the form of a <see cref="CommitId" /></param>
        /// <param name="correlationId">A <see cref="CorrelationId" /> used to relate this event stream to other actions in the system</param>
        /// <param name="source">The <see cref="VersionedEventSource" /> that this stream applies to</param>
        /// <param name="timestamp">A timestamp in the form of a <see cref="DateTimeOffset" /> representing when the stream was committed</param>
        /// <param name="events">An <see cref="EventStream" /> representing the events (an enumerable of <see cref="EventEnvelope" />) that are committed in this commit</param>
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
        /// The unique id in the form of a <see cref="CommitId" />
        /// </summary>
        /// <value></value>
        public CommitId Id { get; }
        /// <summary>
        /// A <see cref="CorrelationId" /> used to relate this event stream to other actions in the system
        /// </summary>
        /// <value></value>
        public CorrelationId CorrelationId { get; }
        /// <summary>
        /// The <see cref="VersionedEventSource" /> that this stream applies to
        /// </summary>
        /// <value></value>
        public VersionedEventSource Source { get; }
        /// <summary>
        /// A timestamp in the form of a <see cref="DateTimeOffset" /> representing when the stream was generated
        /// </summary>
        /// <value></value>
        public DateTimeOffset Timestamp { get; }
        /// <summary>
        /// An <see cref="EventStream" /> representing the events that are committed in this commit
        /// </summary>
        /// <value></value>
        public EventStream Events { get; }

        /// <summary>
        /// A string representation of this <see cref="UncommittedEventStream" /> 
        /// </summary>
        /// <returns>A string showing Id, CorrelationId, Source, Timestamp and the number of events</returns>
        public override string ToString()
        {
            return $"{Id} {CorrelationId} {Source} {Timestamp} {Events.Count()}";
        }
    }
}