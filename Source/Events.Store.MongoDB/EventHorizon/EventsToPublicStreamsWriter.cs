// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.MongoDB.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters.EventHorizon;

namespace Dolittle.Runtime.Events.Store.MongoDB.EventHorizon;

/// <summary>
/// Represents an implementation of <see cref="IWriteEventsToPublicStreams" />.
/// </summary>
[DisableAutoRegistration] // TODO: This gets resolved for private filters as well
public class EventsToPublicStreamsWriter : IWriteEventsToPublicStreams
{
    readonly IStreams _streams;
    readonly IWriteEventsToStreamCollection _eventsToStreamsWriter;
    readonly IEventConverter _eventConverter;
    readonly IStreamEventWatcher _streamWatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventsToPublicStreamsWriter"/> class.
    /// </summary>
    /// <param name="streams">The <see cref="IStreams" />.</param>
    /// <param name="eventsToStreamsWriter">The <see cref="IWriteEventsToStreamCollection" />.</param>
    /// <param name="eventConverter">THe <see cref="IEventConverter" />.</param>
    /// <param name="streamWatcher">The <see cref="IStreamEventWatcher" />.</param>
    public EventsToPublicStreamsWriter(IStreams streams, IWriteEventsToStreamCollection eventsToStreamsWriter, IEventConverter eventConverter, IStreamEventWatcher streamWatcher)
    {
        _streamWatcher = streamWatcher;
        _streams = streams;
        _eventsToStreamsWriter = eventsToStreamsWriter;
        _eventConverter = eventConverter;
    }

    /// <inheritdoc/>
    public Task Write(CommittedEvent @event, StreamId streamId, PartitionId partitionId, CancellationToken cancellationToken)
        => Write(new[] {(@event, partitionId)}, ScopeId.Default,streamId, cancellationToken);

    /// <inheritdoc/>
    public Task Write(CommittedEvent @event, ScopeId scope, StreamId streamId, PartitionId partitionId, CancellationToken cancellationToken) =>
        Write(@event, streamId, partitionId, cancellationToken);

    /// <inheritdoc/>
    public async Task Write(IEnumerable<(CommittedEvent, PartitionId)> events, ScopeId scope, StreamId streamId, CancellationToken cancellationToken)
    {
        var lastWrittenStreamPosition = await _eventsToStreamsWriter.Write(
            await _streams.GetPublic(streamId, cancellationToken).ConfigureAwait(false),
            streamPosition =>
            {
                return events
                    .Select((eventAndPartition, i) => _eventConverter.ToStoreStreamEvent(eventAndPartition.Item1, streamPosition + (ulong) i, eventAndPartition.Item2))
                    .ToList();
            },
            cancellationToken).ConfigureAwait(false);
        _streamWatcher.NotifyForEvent(streamId, lastWrittenStreamPosition);
    }
}
