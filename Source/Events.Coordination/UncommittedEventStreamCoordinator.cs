/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using System.Linq;
using doLittle.Execution;
using doLittle.Runtime.Transactions;
using doLittle.Logging;
using doLittle.Runtime.Events.Storage;
using doLittle.Runtime.Events.Publishing;

namespace doLittle.Runtime.Events.Coordination
{
    /// <summary>
    /// Represents a <see cref="IUncommittedEventStreamCoordinator"/>
    /// </summary>
    [Singleton]
    public class UncommittedEventStreamCoordinator : IUncommittedEventStreamCoordinator
    {
        IEventStore _eventStore;
        IEventSourceVersions _eventSourceVersions;
        ICanSendCommittedEventStream _committedEventStreamSender;
        IEventEnvelopes _eventEnvelopes;
        IEventSequenceNumbers _eventSequenceNumbers;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes an instance of a <see cref="UncommittedEventStreamCoordinator"/>
        /// </summary>
        /// <param name="eventStore"><see cref="IEventStore"/> to use for saving the events</param>
        /// <param name="eventSourceVersions"><see cref="IEventSourceVersions"/> for working with the version for the <see cref="IEventSource"/></param>
        /// <param name="committedEventStreamSender"><see cref="ICanSendCommittedEventStream"/> send the <see cref="CommittedEventStream"/></param>
        /// <param name="eventEnvelopes"><see cref="IEventEnvelopes"/> for working with <see cref="EventEnvelope"/></param>
        /// <param name="eventSequenceNumbers"><see cref="IEventSequenceNumbers"/> for allocating <see cref="EventSequenceNumber">sequence number</see> for <see cref="IEvent">events</see></param>
        /// <param name="logger"><see cref="ILogger"/> for doing logging</param>
        public UncommittedEventStreamCoordinator(
            IEventStore eventStore,
            IEventSourceVersions eventSourceVersions,
            ICanSendCommittedEventStream committedEventStreamSender,
            IEventEnvelopes eventEnvelopes,
            IEventSequenceNumbers eventSequenceNumbers,
            ILogger logger)
        {
            _eventStore = eventStore;
            _eventSourceVersions = eventSourceVersions;
            _committedEventStreamSender = committedEventStreamSender;
            _eventEnvelopes = eventEnvelopes;
            _eventSequenceNumbers = eventSequenceNumbers;
            _logger = logger;
        }

        /// <inheritdoc/>
        public void Commit(TransactionCorrelationId correlationId, UncommittedEventStream uncommittedEventStream)
        {
            _logger.Information($"Committing uncommitted event stream with correlationId '{correlationId}'");
            var envelopes = _eventEnvelopes.CreateFrom(uncommittedEventStream.EventSource, uncommittedEventStream.EventsAndVersion);
            var envelopesAsArray = envelopes.ToArray();
            var eventsAsArray = uncommittedEventStream.ToArray();

            _logger.Trace("Create an array of events and envelopes");
            var eventsAndEnvelopes = new List<EventAndEnvelope>();
            for( var eventIndex=0; eventIndex<eventsAsArray.Length; eventIndex++ )
            {
                var envelope = envelopesAsArray[eventIndex];
                var @event = eventsAsArray[eventIndex];
                eventsAndEnvelopes.Add(new EventAndEnvelope(
                    envelope
                        .WithTransactionCorrelationId(correlationId)
                        .WithSequenceNumber(_eventSequenceNumbers.Next())
                        .WithSequenceNumberForEventType(_eventSequenceNumbers.NextForType(envelope.Event)),
                    @event
                ));
            }

            _logger.Trace("Committing events to event store");
            _eventStore.Commit(eventsAndEnvelopes);

            _logger.Trace($"Set event source versions for the event source '{envelopesAsArray[0].EventSource}' with id '{envelopesAsArray[0].EventSourceId}'");
            _eventSourceVersions.SetFor(envelopesAsArray[0].EventSource, envelopesAsArray[0].EventSourceId, envelopesAsArray[envelopesAsArray.Length - 1].Version);

            _logger.Trace("Create a committed event stream");
            var committedEventStream = new CommittedEventStream(uncommittedEventStream.EventSourceId, eventsAndEnvelopes);

            _logger.Trace("Send the committed event stream");
            _committedEventStreamSender.Send(committedEventStream);
        }
    }
}
