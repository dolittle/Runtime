// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Dolittle.Protobuf.Contracts;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;

namespace Dolittle.Runtime.Events.Store.Streams;

[Singleton, PerTenant]
public class StreamEventSubscriber : IStreamEventSubscriber
{
    readonly IEventLogStream _eventLogStream;

    public StreamEventSubscriber(IEventLogStream eventLogStream) => _eventLogStream = eventLogStream;

    public async IAsyncEnumerable<StreamEvent> SubscribePublic(ProcessingPosition position, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var current = position;
        var channel = _eventLogStream.SubscribePublic(position.EventLogPosition, cancellationToken);
        var eventLogBatch = await channel.ReadAsync(cancellationToken);

        foreach (var evt in eventLogBatch.MatchedEvents)
        {
            if (evt.Public)
            {
                var streamEvent = new StreamEvent(evt.FromProtobuf(), current.StreamPosition, StreamId.EventLog, evt.EventSourceId, true);
                yield return streamEvent;
                current = streamEvent.CurrentProcessingPosition;
            }
            else
            {
                current = current.IncrementEventLogOnly();
            }
        }
    }

    public async IAsyncEnumerable<StreamEvent> Subscribe(ScopeId scopeId, ProcessingPosition position, IReadOnlySet<Uuid> eventTypes, bool partitioned,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var current = position;
        var channel = _eventLogStream.SubscribeAll(scopeId, position.EventLogPosition, cancellationToken);
        var eventLogBatch = await channel.ReadAsync(cancellationToken);

        foreach (var evt in eventLogBatch.MatchedEvents)
        {
            if (eventTypes.Contains(evt.EventType.Id))
            {
                var streamEvent = new StreamEvent(evt.FromProtobuf(), current.StreamPosition, StreamId.EventLog, evt.EventSourceId, partitioned);
                yield return streamEvent;
                current = streamEvent.CurrentProcessingPosition;
            }
            else
            {
                current = current.IncrementEventLogOnly();
            }
        }
    }
}
