// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services;
using Dolittle.Runtime.Services.Hosting;
using Grpc.Core;
using static Dolittle.Runtime.Events.Contracts.EventStore;

namespace Dolittle.Runtime.Events.Store.Services.Grpc;

/// <summary>
/// Represents the implementation of <see cref="EventStoreBase"/>.
/// </summary>
[PrivateService]
public class EventStoreGrpcService : EventStoreBase
{
    const uint MaxBatchMessageSize = 2097152; // 2 MB
    readonly IEventStore _eventStore;
    readonly ISendStreamOfBatchedMessages<FetchForAggregateResponse, Contracts.CommittedAggregateEvents.Types.CommittedAggregateEvent> _aggregateEventsBatchSender;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreGrpcService"/> class.
    /// </summary>
    /// <param name="eventStore">The event store to use.</param>
    /// <param name="aggregateEventsBatchSender">The batched aggregate events sender.</param>
    public EventStoreGrpcService(
        IEventStore eventStore,
        ISendStreamOfBatchedMessages<FetchForAggregateResponse, Contracts.CommittedAggregateEvents.Types.CommittedAggregateEvent> aggregateEventsBatchSender)
    {
        _eventStore = eventStore;
        _aggregateEventsBatchSender = aggregateEventsBatchSender;
    }

    /// <inheritdoc/>
    public override Task<CommitEventsResponse> Commit(CommitEventsRequest request, ServerCallContext context)
        => _eventStore.CommitEvents(request, context.CancellationToken);

    /// <inheritdoc/>
    public override Task<CommitAggregateEventsResponse> CommitForAggregate(CommitAggregateEventsRequest request, ServerCallContext context)
        => _eventStore.CommitAggregateEvents(request, context.CancellationToken);

    /// <inheritdoc/>
    public override Task<FetchForAggregateResponse> FetchForAggregate(FetchForAggregateRequest request, ServerCallContext context)
        => _eventStore.FetchAggregateEvents(request, context.CancellationToken);

    public override async Task FetchForAggregateInBatches(FetchForAggregateInBatchesRequest request, IServerStreamWriter<FetchForAggregateResponse> responseStream, ServerCallContext context)
    {
        var eventSourceId = request.Aggregate.EventSourceId;
        var aggregateRootId = request.Aggregate.AggregateRootId.ToGuid(); 
        var fetchResult = await _eventStore.FetchAggregateEvents(
            eventSourceId,
            aggregateRootId,
            request.EventTypes.Select(_ => _.ToArtifact()),
            request.CallContext.ExecutionContext.TenantId.ToGuid(),
            context.CancellationToken).ConfigureAwait(false);

        if (!fetchResult.Success)
        {
            var response = new FetchForAggregateResponse() { Failure = fetchResult.Exception.ToFailure() };
            await responseStream.WriteAsync(response).ConfigureAwait(false);
            return;
        }
        await _aggregateEventsBatchSender.Send(
            MaxBatchMessageSize,
            fetchResult.Result.EventStream.GetAsyncEnumerator(context.CancellationToken),
            () => new FetchForAggregateResponse
            {
                Events = new Contracts.CommittedAggregateEvents
                {
                    AggregateRootId = aggregateRootId.ToProtobuf(),
                    EventSourceId = eventSourceId,
                    AggregateRootVersion = fetchResult.Result.AggregateRootVersion
                }
            },
            (response, aggregateEvent) =>
            {
                response.Events.Events.Add(aggregateEvent.ToProtobuf());
            },
            _ => _.ToProtobuf(),
            _ => responseStream.WriteAsync(_, context.CancellationToken)
        ).ConfigureAwait(false);
    }
}
