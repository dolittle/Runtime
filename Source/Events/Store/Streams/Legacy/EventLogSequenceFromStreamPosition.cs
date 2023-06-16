// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Store.Streams.Legacy;

[Singleton, PerTenant]
public class EventLogSequenceFromStreamPosition: IGetEventLogSequenceFromStreamPosition
{
    readonly IEventFetchers _eventFetchers;

    public EventLogSequenceFromStreamPosition(IEventFetchers eventFetchers)
    {
        _eventFetchers = eventFetchers;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="partitioned"></param>
    /// <param name="streamPosition"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Try<EventLogSequenceNumber>> TryGetEventLogPositionForStreamProcessor(StreamProcessorId id, bool partitioned, StreamPosition streamPosition,
        CancellationToken cancellationToken)
    {
        var sourceStream = new StreamId(id.SourceStreamId);
        IStreamDefinition streamDefinition = new StreamDefinition(new FilterDefinition(sourceStream, sourceStream, partitioned));
        var fetcher = await _eventFetchers.GetFetcherFor(id.ScopeId, streamDefinition, cancellationToken);

        var eventFromStream = await fetcher.FetchSingle(streamPosition, cancellationToken);

        if (!eventFromStream.Success)
        {
            // We are at the end of the stream, get the last event
            var lastEvent = await fetcher.FetchLast(cancellationToken);
            if (lastEvent.Success)
            {
                return lastEvent.Result.Event.EventLogSequenceNumber.Increment();
            }
            // No Events, start from the beginning
            return EventLogSequenceNumber.Initial;
        }
        
        

        return eventFromStream.Result.Event.EventLogSequenceNumber;
    }
}
