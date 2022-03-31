// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services.Hosting;
using Grpc.Core;
using static Dolittle.Runtime.Events.Contracts.EventStore;

namespace Dolittle.Runtime.Events.Store.Services.Grpc;

/// <summary>
/// Represents the implementation of.
/// </summary>
[PrivateService]
public class EventStoreGrpcService : EventStoreBase
{
    readonly IEventStoreService _eventStoreService;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreGrpcService"/> class.
    /// </summary>
    /// <param name="eventStoreService"><see cref="IEventStoreService"/>.</param>
    public EventStoreGrpcService(IEventStoreService eventStoreService)
    {
        _eventStoreService = eventStoreService;
    }

    /// <inheritdoc/>
    public override async Task<Contracts.CommitEventsResponse> Commit(Contracts.CommitEventsRequest request, ServerCallContext context)
    {
        var response = new Contracts.CommitEventsResponse();
        var events = request.Events.Select(_ => new UncommittedEvent(_.EventSourceId, new Artifact(_.EventType.Id.ToGuid(), _.EventType.Generation), _.Public, _.Content));
        var commitResult = await _eventStoreService.TryCommit(
            new UncommittedEvents(new ReadOnlyCollection<UncommittedEvent>(events.ToList())),
            request.CallContext.ExecutionContext.ToExecutionContext(),
            context.CancellationToken).ConfigureAwait(false);

        if (commitResult.Success)
        {
            response.Events.AddRange(commitResult.Result.ToProtobuf());
        }
        else
        {
            response.Failure = commitResult.Exception.ToFailure();
        }

        return response;
    }

    /// <inheritdoc/>
    public override async Task<Contracts.CommitAggregateEventsResponse> CommitForAggregate(Contracts.CommitAggregateEventsRequest request, ServerCallContext context)
    {
        var response = new Contracts.CommitAggregateEventsResponse();
        EventSourceId eventSourceId = request.Events.EventSourceId;
        var events = request.Events.Events.Select(_ => new UncommittedEvent(eventSourceId, new Artifact(_.EventType.Id.ToGuid(), _.EventType.Generation), _.Public, _.Content));

        var commitResult = await _eventStoreService.TryCommitForAggregate(
            new UncommittedAggregateEvents(
                eventSourceId,
                new Artifact(request.Events.AggregateRootId.ToGuid(), ArtifactGeneration.First),
                request.Events.ExpectedAggregateRootVersion,
                new ReadOnlyCollection<UncommittedEvent>(events.ToList())),
            request.CallContext.ExecutionContext.ToExecutionContext(),
            context.CancellationToken).ConfigureAwait(false);
        if (commitResult.Success)
        {
            response.Events = commitResult.Result.ToProtobuf();
        }
        else
        {
            response.Failure = commitResult.Exception.ToFailure();
        }

        return response;
    }

    /// <inheritdoc/>
    public override async Task<Contracts.FetchForAggregateResponse> FetchForAggregate(Contracts.FetchForAggregateRequest request, ServerCallContext context)
    {
        var response = new Contracts.FetchForAggregateResponse();
        var fetchResult = await _eventStoreService.TryFetchForAggregate(
            request.Aggregate.AggregateRootId.ToGuid(),
            request.Aggregate.EventSourceId,
            request.CallContext.ExecutionContext.ToExecutionContext(),
            context.CancellationToken).ConfigureAwait(false);
        if (fetchResult.Success)
        {
            response.Events = fetchResult.Result.ToProtobuf();
        }
        else
        {
            response.Failure = fetchResult.Exception.ToFailure();
        }

        return response;
    }
}
