// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Events.Store.Streams;

[Singleton, PerTenant]
public class StreamEventSubscriber : IStreamEventSubscriber
{
    const int ChannelCapacity = 20;
    readonly IEventLogStream _eventLogStream;

    public StreamEventSubscriber(IEventLogStream eventLogStream) => _eventLogStream = eventLogStream;

    public ChannelReader<(StreamEvent? streamEvent, EventLogSequenceNumber nextSequenceNumber)> Subscribe(ScopeId scopeId,
        IReadOnlyCollection<ArtifactId> artifactIds,
        ProcessingPosition from,
        bool partitioned,
        string subscriptionName,
        Predicate<Contracts.CommittedEvent>? until,
        CancellationToken cancellationToken)
    {
        var eventTypes = artifactIds.Select(_ => _.Value.ToProtobuf()).ToHashSet();

        var channel = Channel.CreateBounded<(StreamEvent? streamEvent, EventLogSequenceNumber nextSequenceNumber)>(ChannelCapacity);
        var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        ToStreamEvents(
            _eventLogStream.Subscribe(scopeId, from.EventLogPosition, artifactIds, subscriptionName, linkedTokenSource.Token),
            channel.Writer,
            from,
            evt => eventTypes.Contains(evt.EventType.Id),
            partitioned,
            until,
            linkedTokenSource);
        return channel.Reader;
    }

    static void ToStreamEvents(ChannelReader<EventLogBatch> reader, ChannelWriter<(StreamEvent? streamEvent, EventLogSequenceNumber nextSequenceNumber)> writer, ProcessingPosition startingPosition,
        Func<Contracts.CommittedEvent, bool> include, bool partitioned,
        Predicate<Contracts.CommittedEvent>? until,
        CancellationTokenSource linkedTokenSource) =>
        _ = Task.Run(async () =>
        {
            var currentStreamPosition = startingPosition.StreamPosition;
            
            // This method owns the linkedTokenSource and will dispose it when the reader is done, even if it got it as a parameter.
            using var cts = linkedTokenSource;
            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    var eventLogBatch = await reader.ReadAsync(linkedTokenSource.Token);

                    var includedEvents = eventLogBatch.MatchedEvents.Where(include).ToList();
                    if (includedEvents.Count == 0)
                    {
                        // No events included, send the next sequence number and continue
                        await writer.WriteAsync((null, eventLogBatch.To.Increment()), cts.Token);
                        continue;
                    }
                    for (var index = 0; index < includedEvents.Count; index++)
                    {
                        var evt = includedEvents[index];
                        if (until != null && !cts.IsCancellationRequested && until(evt))
                        {
                            cts.Cancel();
                            return;
                        }
                        var nextEvent = includedEvents.ElementAtOrDefault(index + 1);
                        
                        // Skip any event that are not included in the artifact set.
                        // If the next event is null, we are at the end of the batch, so we use the next sequence number from the batch.
                        EventLogSequenceNumber nextEventLogSequenceNumber = nextEvent?.EventLogSequenceNumber ?? eventLogBatch.To.Increment();
                        
                        var streamEvent = new StreamEvent(evt.FromProtobuf(), currentStreamPosition, StreamId.EventLog,
                            evt.EventSourceId, partitioned, nextEventLogSequenceNumber);

                        // ReSharper disable once MethodSupportsCancellation
                        await writer.WriteAsync((streamEvent, nextEventLogSequenceNumber));
                        currentStreamPosition = currentStreamPosition.Increment();
                    }
                }

                writer.TryComplete();
            }
            catch (ChannelClosedException)
            {
                writer.TryComplete();
            }
            catch (Exception e)
            {
                writer.TryComplete(e);
            }
        }, CancellationToken.None);
}
