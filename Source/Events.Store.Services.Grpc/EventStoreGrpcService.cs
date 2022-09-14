// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Rudimentary.AsyncEnumerators;
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
    public override async Task<FetchForAggregateResponse> FetchForAggregate(FetchForAggregateRequest request, ServerCallContext context)
        => await FetchEventsInBatches(
                new FetchForAggregateInBatchesRequest
                {
                    CallContext = request.CallContext,
                    Aggregate = request.Aggregate,
                    FetchAllEvents = new FetchAllEventsForAggregateInBatchesRequest(),
                },
                uint.MaxValue,
                context)
            .SingleAsync(context.CancellationToken).ConfigureAwait(false);

    /// <inheritdoc/>
    public override async Task FetchForAggregateInBatches(FetchForAggregateInBatchesRequest request, IServerStreamWriter<FetchForAggregateResponse> responseStream, ServerCallContext context)
    {
        await foreach (var batch in FetchEventsInBatches(request, MaxBatchMessageSize, context).WithCancellation(context.CancellationToken))
        {
            await responseStream.WriteAsync(batch, context.CancellationToken).ConfigureAwait(false);
        }
    }

    IAsyncEnumerable<FetchForAggregateResponse> FetchEventsInBatches(FetchForAggregateInBatchesRequest request, uint maxBatchSize, ServerCallContext context)
        => _eventStore
            .FetchAggregateEvents(request, context.CancellationToken)
            .BatchReduceMessagesOfSize(AddAllEventsToFirstResponse, maxBatchSize, context.CancellationToken);

    static FetchForAggregateResponse AddAllEventsToFirstResponse(FetchForAggregateResponse first, FetchForAggregateResponse next)
    {
        first.Failure ??= next.Failure;
        first.Events.Events.AddRange(next.Events.Events);
        
        return first;
    }
}
