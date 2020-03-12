// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using Dolittle.Artifacts;
using Dolittle.Collections;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Represents a sequence of events applied by an AggregateRoot to an Event Source that have been committed to the Event Store.
    /// </summary>
    public class CommittedAggregateEvents : IReadOnlyList<CommittedAggregateEvent>
    {
        readonly NullFreeList<CommittedAggregateEvent> _events;
        readonly AggregateRootVersion _currentCheckedVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommittedAggregateEvents"/> class.
        /// </summary>
        /// <param name="eventSource">The <see cref="EventSourceId"/> that the Events were applied to.</param>
        /// <param name="aggregateRoot">The <see cref="ArtifactId"/> representing the type of the Aggregate Root that applied the Event to the Event Source.</param>
        /// <param name="events">The <see cref="CommittedAggregateEvent">events</see>.</param>
        public CommittedAggregateEvents(EventSourceId eventSource, ArtifactId aggregateRoot, IReadOnlyList<CommittedAggregateEvent> events)
        {
            EventSource = eventSource;
            AggregateRoot = aggregateRoot;
            for (var i = 0; i < events.Count; i++)
            {
                if (i == 0) _currentCheckedVersion = events[0].AggregateRootVersion;
                var @event = events[i];
                ThrowIfEventIsNull(@event);
                ThrowIfEventWasAppliedToOtherEventSource(@event);
                ThrowIfEventWasAppliedByOtherAggregateRoot(@event);
                ThrowIfAggreggateRootVersionIsOutOfOrder(@event);
                if (i > 0) ThrowIfEventLogSequenceIsOutOfOrder(@event, events[i - 1]);
                _currentCheckedVersion++;
            }

            _events = new NullFreeList<CommittedAggregateEvent>(events);
        }

        /// <summary>
        /// Gets the Event Source that the Events were applied to.
        /// </summary>
        public EventSourceId EventSource { get; }

        /// <summary>
        /// Gets the <see cref="ArtifactId"/> representing the type of the Aggregate Root that applied the Event to the Event Source.
        /// </summary>
        public ArtifactId AggregateRoot { get; }

        /// <summary>
        /// Gets a value indicating whether or not there are any events in the committed sequence.
        /// </summary>
        public bool HasEvents => Count > 0;

        /// <inheritdoc/>
        public int Count => _events.Count;

        /// <inheritdoc/>
        public CommittedAggregateEvent this[int index] => _events[index];

        /// <inheritdoc/>
        public IEnumerator<CommittedAggregateEvent> GetEnumerator() => _events.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => _events.GetEnumerator();

        void ThrowIfEventIsNull(CommittedAggregateEvent @event)
        {
            if (@event == null) throw new EventCanNotBeNull();
        }

        void ThrowIfEventWasAppliedToOtherEventSource(CommittedAggregateEvent @event)
        {
            if (@event.EventSource != EventSource) throw new EventWasAppliedToOtherEventSource(@event.EventSource, EventSource);
        }

        void ThrowIfEventWasAppliedByOtherAggregateRoot(CommittedAggregateEvent @event)
        {
            if (@event.AggregateRoot.Id != AggregateRoot) throw new EventWasAppliedByOtherAggregateRoot(@event.AggregateRoot.Id, AggregateRoot);
        }

        void ThrowIfAggreggateRootVersionIsOutOfOrder(CommittedAggregateEvent @event)
        {
            if (@event.AggregateRootVersion != _currentCheckedVersion) throw new AggregateRootVersionIsOutOfOrder(@event.AggregateRootVersion, _currentCheckedVersion);
        }

        void ThrowIfEventLogSequenceIsOutOfOrder(CommittedAggregateEvent @event, CommittedAggregateEvent previousEvent)
        {
            if (@event.EventLogSequenceNumber <= previousEvent.EventLogSequenceNumber) throw new EventLogSequenceIsOutOfOrder(@event.EventLogSequenceNumber, previousEvent.EventLogSequenceNumber);
        }
    }
}