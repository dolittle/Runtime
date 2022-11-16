// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Embeddings.Processing;
using Dolittle.Runtime.Events;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Protobuf;
using Dolittle.Services.Contracts;
using Integration.Shared;
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
        var response = await eventStore.CommitEvents(request, CancellationToken.None).ConfigureAwait(false);
        return response;
    }
    public static Task<CommitAggregateEventsResponse> Commit(this IEventStore eventStore, UncommittedAggregateEvents events, Dolittle.Runtime.Execution.ExecutionContext executionContext)
        => eventStore.CommitAggregateEvents(events.ToCommitRequest(executionContext), CancellationToken.None);

    public static IAsyncEnumerable<FetchForAggregateResponse> FetchForAggregate(this IEventStore eventStore, ArtifactId aggregateRootId, EventSourceId eventSourceId, Dolittle.Runtime.Execution.ExecutionContext executionContext)
        => eventStore.FetchAggregateEvents(EventStoreRequests.FetchBatchFor(aggregateRootId, eventSourceId, executionContext), CancellationToken.None);
    
    public static IAsyncEnumerable<FetchForAggregateResponse> FetchForAggregate(this IEventStore eventStore, ArtifactId aggregateRootId, EventSourceId eventSourceId, IEnumerable<ArtifactId> eventTypes, Dolittle.Runtime.Execution.ExecutionContext executionContext)
        => eventStore.FetchAggregateEvents(EventStoreRequests.FetchBatchFor(aggregateRootId, eventSourceId, eventTypes, executionContext), CancellationToken.None);

    public static FetchForAggregateResponse Combine(this FetchForAggregateResponse[] responses)
        => new()
        {
            Failure = responses.Where(_ => _.Failure is not null).Select(_ => _.Failure).FirstOrDefault(),
            Events = new Dolittle.Runtime.Events.Contracts.CommittedAggregateEvents
            {
                Events =
                {
                    responses.SelectMany(_ => _.Events.Events)
                },
                AggregateRootId = responses.First().Events.AggregateRootId,
                EventSourceId = responses.First().Events.EventSourceId,
                CurrentAggregateRootVersion = responses.First().Events.CurrentAggregateRootVersion,
                AggregateRootVersion = responses.First().Events.AggregateRootVersion
            }
        };

}
