// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Store.Services;

/// <summary>
/// Represents the implementation of <see cref="IEventStoreService" />.
/// </summary>
[Singleton]
public class EventStoreService : IEventStoreService
{
    readonly ICreateExecutionContexts _executionContextCreator;
    readonly Func<TenantId, IEventStore> _getEventStore;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreService"/> class.
    /// </summary>
    /// <param name="executionContextCreator">The <see cref="ICreateExecutionContexts"/> to use to validate incoming execution contexts.</param>
    /// <param name="getEventStore">Factory to use to resolve the <see cref="IEventStore"/> for a specific tenant.</param>
    /// <param name="logger">The logger to use for logging.</param>
    public EventStoreService(
        ICreateExecutionContexts executionContextCreator,
        Func<TenantId, IEventStore> getEventStore,
        ILogger logger)
    {
        _executionContextCreator = executionContextCreator;
        _getEventStore = getEventStore;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Try<CommittedEvents>> TryCommit(UncommittedEvents events, ExecutionContext context, CancellationToken token)
    {
        _logger.EventsReceivedForCommitting(false, events.Count);
        return await _executionContextCreator
            .TryCreateUsing(context)
            .Then(executionContext =>
                _getEventStore(executionContext.Tenant).CommitEvents(events, executionContext, token))
            .Then(_ => 
                Log.EventsSuccessfullyCommitted(_logger))
            .Catch(exception => 
                Log.ErrorCommittingEvents(_logger, exception));
    }

    public Task<CommitEventsResponse> Commit(CommitEventsRequest request, CancellationToken token)
        => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<Try<CommittedAggregateEvents>> TryCommitForAggregate(UncommittedAggregateEvents events, ExecutionContext context, CancellationToken token)
    {
        _logger.EventsReceivedForCommitting(true, events.Count);

        return _executionContextCreator
            .TryCreateUsing(context)
            .Then(executionContext =>
                _getEventStore(executionContext.Tenant).CommitAggregateEvents(events, executionContext, token))
            .Then(_ =>
                Log.AggregateEventsSuccessfullyCommitted(_logger))
            .Catch(exception =>
                Log.ErrorCommittingAggregateEvents(_logger, exception));
    }

    public Task<CommitAggregateEventsResponse> CommitForAggregate(CommitAggregateEventsRequest request, CancellationToken token)
        => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<Try<CommittedAggregateEvents>> TryFetchForAggregate(ArtifactId aggregateRoot, EventSourceId eventSource, ExecutionContext context, CancellationToken token)
    {
        Log.FetchEventsForAggregate(_logger);

        return _executionContextCreator
            .TryCreateUsing(context)
            .Then(executionContext =>
                _getEventStore(executionContext.Tenant).FetchForAggregate(eventSource, aggregateRoot, token))
            .Then(_ =>
                Log.SuccessfullyFetchedEventsForAggregate(_logger))
            .Catch(exception =>
                Log.ErrorFetchingEventsFromAggregate(_logger, exception));
    }

    public Task<FetchForAggregateResponse> FetchForAggregate(FetchForAggregateRequest request, CancellationToken token)
        => throw new NotImplementedException();
}
