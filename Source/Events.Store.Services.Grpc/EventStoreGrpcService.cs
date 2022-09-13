// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
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
    readonly StreamOfBatchedMessagesSender<FetchForAggregateResponse, Contracts.CommittedAggregateEvents.Types.CommittedAggregateEvent> _aggregateEventsBatchSender = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreGrpcService"/> class.
    /// </summary>
    /// <param name="eventStore">The event store to use.</param>
    public EventStoreGrpcService(IEventStore eventStore)
    {
        _eventStore = eventStore;
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
        var fetchResult = request.RequestCase == FetchForAggregateInBatchesRequest.RequestOneofCase.FetchEvents
            ? await _eventStore.FetchAggregateEvents(
                eventSourceId,
                aggregateRootId,
                request.FetchEvents.EventTypes.Select(_ => _.ToArtifact().Id),
                request.CallContext.ExecutionContext.TenantId.ToGuid(),
                context.CancellationToken).ConfigureAwait(false)
            : await _eventStore.FetchAggregateEvents(
                eventSourceId,
                aggregateRootId,
                request.CallContext.ExecutionContext.TenantId.ToGuid(),
                context.CancellationToken);
        if (!fetchResult.Success)
        {
            var response = new FetchForAggregateResponse { Failure = fetchResult.Exception.ToFailure() };
            await responseStream.WriteAsync(response).ConfigureAwait(false);
            return;
        }

        await _aggregateEventsBatchSender.Send(
            MaxBatchMessageSize,
            fetchResult.Result.EventStream.GetAsyncEnumerator(context.CancellationToken),
            () => CreateResponse(aggregateRootId, eventSourceId, fetchResult.Result.AggregateRootVersion),
            (batch, aggregateEvent) => batch.Events.Events.Add(aggregateEvent.ToProtobuf()),
            aggregateEvent => aggregateEvent.ToProtobuf(),
            batch => responseStream.WriteAsync(batch, context.CancellationToken)
        ).ConfigureAwait(false);
    }

    static FetchForAggregateResponse CreateResponse(ArtifactId aggregateRootId, EventSourceId eventSourceId, AggregateRootVersion currentAggregateRootVersion)
        => new()
        {
            Events = new Contracts.CommittedAggregateEvents
            {
                AggregateRootId = aggregateRootId.ToProtobuf(),
                EventSourceId = eventSourceId,
                AggregateRootVersion = currentAggregateRootVersion == AggregateRootVersion.Initial
                    ? AggregateRootVersion.Initial
                    : currentAggregateRootVersion - 1,
                CurrentAggregateRootVersion = currentAggregateRootVersion
            }
        };
}
