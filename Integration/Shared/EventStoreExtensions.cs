// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Embeddings.Processing;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Protobuf;
using Dolittle.Services.Contracts;
using UncommittedAggregateEvents = Dolittle.Runtime.Events.Store.UncommittedAggregateEvents;
using UncommittedEventContract = Dolittle.Runtime.Events.Contracts.UncommittedEvent;

namespace Integration;

public static class EventStoreExtensions
{
    public static async Task<CommitEventsResponse> Commit(this IEventStore eventStore, UncommittedEvents events, Dolittle.Runtime.Execution.ExecutionContext executionContext)
    {
        var request = new CommitEventsRequest{CallContext = new CallRequestContext{ExecutionContext = executionContext.ToProtobuf()}};
        request.Events.AddRange(events.Select(_ => new UncommittedEventContract
        {
            Content = _.Content,
            Public = _.Public,
            EventType = _.Type.ToProtobuf(),
            EventSourceId = _.EventSource
        }));
        var response = await eventStore.CommitEvents(request, CancellationToken.None);
        if (response.Failure != default)
        {
            throw new Exception(response.Failure.Reason);
        }

        return response;
    }
    public static async Task<CommitAggregateEventsResponse> Commit(this IEventStore eventStore, UncommittedAggregateEvents events, Dolittle.Runtime.Execution.ExecutionContext executionContext)
    {
        var response = await eventStore.CommitAggregateEvents(events.ToCommitRequest(executionContext), CancellationToken.None);
        return response;

    }
    public static async Task<FetchForAggregateResponse> FetchForAggregate(this IEventStore eventStore, ArtifactId aggregateRootId, EventSourceId eventSourceId, Dolittle.Runtime.Execution.ExecutionContext executionContext)
    {
        var response = await eventStore.FetchAggregateEvents(new FetchForAggregateRequest
        {
            CallContext = new CallRequestContext
            {
                ExecutionContext = executionContext.ToProtobuf()
            },
            Aggregate = new Aggregate{AggregateRootId = aggregateRootId.ToProtobuf(), EventSourceId = eventSourceId}
        }, CancellationToken.None);

        return response;
    }
}
