// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.MongoDB.Streams;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.EventHorizon;

/// <summary>
/// Represents an implementation of <see cref="IWriteEventHorizonEvents" />.
/// </summary>
public class EventHorizonEventsWriter : IWriteEventHorizonEvents
{
    readonly IStreams _streams;
    readonly IWriteEventsToStreamCollection _eventsToStreamsWriter;
    readonly IEventConverter _eventConverter;
    readonly IStreamEventWatcher _streamWatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHorizonEventsWriter"/> class.
    /// </summary>
    /// <param name="streams">The <see cref="IStreams" />.</param>
    /// <param name="eventsToStreamsWriter">The <see cref="IWriteEventsToStreamCollection" />.</param>
    /// <param name="eventConverter">The <see cref="IEventConverter" />.</param>
    /// <param name="streamWatcher">The <see cref="IStreamEventWatcher" />.</param>
    public EventHorizonEventsWriter(IStreams streams, IWriteEventsToStreamCollection eventsToStreamsWriter, IEventConverter eventConverter, IStreamEventWatcher streamWatcher)
    {
        _streams = streams;
        _eventsToStreamsWriter = eventsToStreamsWriter;
        _eventConverter = eventConverter;
        _streamWatcher = streamWatcher;
    }

    /// <inheritdoc/>
    public async Task<EventLogSequenceNumber> Write(CommittedEvent @event, ConsentId consentId, ScopeId scope, CancellationToken cancellationToken)
    {
        var writtenStreamPosition = await _eventsToStreamsWriter.Write(
            await _streams.GetEventLog(scope, cancellationToken).ConfigureAwait(false),
            streamPosition => _eventConverter.ToEventLogEvent(
                new CommittedExternalEvent(
                    streamPosition.Value,
                    @event.Occurred,
                    @event.EventSource,
                    @event.ExecutionContext,
                    @event.Type,
                    false,
                    @event.Content,
                    @event.EventLogSequenceNumber,
                    DateTimeOffset.UtcNow,
                    consentId)),
            cancellationToken).ConfigureAwait(false);
        _streamWatcher.NotifyForEvent(scope, StreamId.EventLog, new ProcessingPosition(writtenStreamPosition.Value, writtenStreamPosition.Value));
        return writtenStreamPosition.Value;
    }
}
