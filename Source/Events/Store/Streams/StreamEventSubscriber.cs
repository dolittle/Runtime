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
    const int ChannelCapacity = 100;
    readonly IEventLogStream _eventLogStream;

    public StreamEventSubscriber(IEventLogStream eventLogStream) => _eventLogStream = eventLogStream;

    // public ChannelReader<StreamEvent> SubscribePublic(ProcessingPosition position, CancellationToken cancellationToken)
    // {
    //     var channel = Channel.CreateBounded<StreamEvent>(ChannelCapacity);
    //     ToStreamEvents(
    //         _eventLogStream.SubscribePublic(position.EventLogPosition, cancellationToken),
    //         channel.Writer,
    //         position,
    //         evt => evt.Public,
    //         false,
    //         cancellationToken);
    //     return channel.Reader;
    // }

    public ChannelReader<StreamEvent> Subscribe(ScopeId scopeId,
        IReadOnlyCollection<ArtifactId> artifactIds,
        ProcessingPosition from,
        bool partitioned,
        string subscriptionName,
        Predicate<Contracts.CommittedEvent>? until,
        CancellationToken cancellationToken)
    {
        var eventTypes = artifactIds.Select(_ => _.Value.ToProtobuf()).ToHashSet();

        var channel = Channel.CreateBounded<StreamEvent>(ChannelCapacity);
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

    static void ToStreamEvents(ChannelReader<EventLogBatch> reader, ChannelWriter<StreamEvent> writer, ProcessingPosition startingPosition,
        Func<Contracts.CommittedEvent, bool> include, bool partitioned,
        Predicate<Contracts.CommittedEvent>? until,
        CancellationTokenSource linkedTokenSource) =>
        _ = Task.Run(async () =>
        {
            var current = startingPosition.StreamPosition;
            using var cts = linkedTokenSource;
            try
            {
                while (!linkedTokenSource.Token.IsCancellationRequested)
                {
                    var eventLogBatch = await reader.ReadAsync(linkedTokenSource.Token);

                    foreach (var evt in eventLogBatch.MatchedEvents)
                    {
                        if (until != null && !cts.IsCancellationRequested && until(evt))
                        {
                            cts.Cancel();
                            return;
                        }
                        
                        if (include(evt))
                        {
                            var streamEvent = new StreamEvent(evt.FromProtobuf(), current, StreamId.EventLog, evt.EventSourceId, partitioned);

                            // ReSharper disable once MethodSupportsCancellation
                            await writer.WriteAsync(streamEvent);
                            current = current.Increment();
                        }
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
