// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Actors;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;
using Proto;
using Proto.Cluster;
using Artifact = Dolittle.Runtime.Artifacts.Artifact;

namespace Dolittle.Runtime.Events.Store.Services.Actors;

/// <summary>
/// Represents an implementation of <see cref="EventStoreGrainBase"/>.
/// </summary>
[TenantGrain(typeof(EventStoreGrainActor), typeof(EventStoreGrainClient))]
public class EventStoreGrain : EventStoreGrainBase
{
    readonly ClusterIdentity _identity;
    readonly IEventStore _eventStore;
    readonly ICreateExecutionContexts _executionContextCreator;
    readonly ILogger _logger;

    public EventStoreGrain(IContext context, ClusterIdentity identity, IEventStore eventStore, ICreateExecutionContexts executionContextCreator, ILogger logger)
        : base(context)
    {
        _identity = identity;
        _eventStore = eventStore;
        _executionContextCreator = executionContextCreator;
        _logger = logger;
    }

    public override async Task<CommitEventsResponse> Commit(Commit request)
    {
        var result =  await _executionContextCreator
            .TryCreateUsing(request.ExecutionContext.ToExecutionContext())
            .Then(async executionContext =>
            {
                var response = new Contracts.CommitEventsResponse();
                var events = request.Events.Select(_ => new UncommittedEvent(_.EventSourceId, new Artifact(_.EventType.Id.ToGuid(), _.EventType.Generation), _.Public, _.Content));
                var uncommittedEvents = new UncommittedEvents(new ReadOnlyCollection<UncommittedEvent>(events.ToList()));
                _logger.EventsReceivedForCommitting(false, uncommittedEvents.Count);
                var res = await _eventStore.CommitEvents(uncommittedEvents, executionContext, Context.CancellationToken).ConfigureAwait(false);
                response.Events.AddRange(res.ToProtobuf());
                return response;
            })
            .Then(_ => Log.EventsSuccessfullyCommitted(_logger))
            .Catch(exception => Log.ErrorCommittingEvents(_logger, exception));
        return result.Success ? result : new CommitEventsResponse
        {
            Failure = result.Exception.ToFailure()
        };
    }

    public override async Task<CommitAggregateEventsResponse> CommitForAggregate(CommitForAggregate request)
    {
        var result = await _executionContextCreator
            .TryCreateUsing(request.ExecutionContext.ToExecutionContext())
            .Then(async executionContext =>
            {
                var response = new Contracts.CommitAggregateEventsResponse();
                EventSourceId eventSourceId = request.Events.EventSourceId;
                var events = request.Events.Events.Select(_ => new UncommittedEvent(eventSourceId, new Artifact(_.EventType.Id.ToGuid(), _.EventType.Generation), _.Public, _.Content));
                var uncommittedEvents = new UncommittedAggregateEvents(
                    eventSourceId,
                    new Artifact(request.Events.AggregateRootId.ToGuid(), ArtifactGeneration.First),
                    request.Events.ExpectedAggregateRootVersion,
                    new ReadOnlyCollection<UncommittedEvent>(events.ToList()));
                _logger.EventsReceivedForCommitting(false, uncommittedEvents.Count);
                var committedEvents = await _eventStore.CommitAggregateEvents(
                    new UncommittedAggregateEvents(
                        eventSourceId,
                        new Artifact(request.Events.AggregateRootId.ToGuid(), ArtifactGeneration.First),
                        request.Events.ExpectedAggregateRootVersion,
                        new ReadOnlyCollection<UncommittedEvent>(events.ToList())),
                    executionContext,
                    Context.CancellationToken).ConfigureAwait(false);
                response.Events = committedEvents.ToProtobuf();
                return response;
            })
            .Then(_ => Log.AggregateEventsSuccessfullyCommitted(_logger))
            .Catch(exception => Log.ErrorCommittingAggregateEvents(_logger, exception));
        return result.Success ? result : new CommitAggregateEventsResponse()
        {
            Failure = result.Exception.ToFailure()
        };
    }

    public override async Task<FetchForAggregateResponse> FetchForAggregate(FetchForAggregate request)
    {
        var result = await _executionContextCreator
            .TryCreateUsing(request.ExecutionContext.ToExecutionContext())
            .Then(async executionContext =>
            {
                Log.FetchEventsForAggregate(_logger);
                var response = new Contracts.FetchForAggregateResponse();
                EventSourceId eventSourceId = request.Aggregate.EventSourceId;
                var committedEvents = await _eventStore.FetchForAggregate(
                    eventSourceId,
                    request.Aggregate.AggregateRootId.ToGuid(),
                    Context.CancellationToken).ConfigureAwait(false);
                response.Events = committedEvents.ToProtobuf();
                return response;
            })
            .Then(_ => Log.SuccessfullyFetchedEventsForAggregate(_logger))
            .Catch(exception => Log.ErrorFetchingEventsFromAggregate(_logger, exception));
        return result.Success ? result : new FetchForAggregateResponse()
        {
            Failure = result.Exception.ToFailure()
        };
    }
}
