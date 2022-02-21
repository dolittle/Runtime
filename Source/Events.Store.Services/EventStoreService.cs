// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.DependencyInjection;
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
        

        var commitEvents = await TryPerformInExecutionContext(
                context, 
                (eventStore, executionContext) => eventStore.CommitEvents(events, executionContext, token))
            .ConfigureAwait(false);

        if (commitEvents.Success)
        {
            Log.EventsSuccessfullyCommitted(_logger);
        }
        else
        {
            Log.ErrorCommittingEvents(_logger, commitEvents.Exception);
        }

        return commitEvents;
    }

    /// <inheritdoc/>
    public async Task<Try<CommittedAggregateEvents>> TryCommitForAggregate(UncommittedAggregateEvents events, ExecutionContext context, CancellationToken token)
    {
        _logger.EventsReceivedForCommitting(true, events.Count);
        
        var commitEvents = await TryPerformInExecutionContext(
                context, 
                (eventStore, executionContext) => eventStore.CommitAggregateEvents(events, executionContext, token))
            .ConfigureAwait(false);
        
        if (commitEvents.Success)
        {
            Log.AggregateEventsSuccessfullyCommitted(_logger);
        }
        else
        {
            Log.ErrorCommittingAggregateEvents(_logger, commitEvents.Exception);
        }

        return commitEvents;
    }

    /// <inheritdoc/>
    public async Task<Try<CommittedAggregateEvents>> TryFetchForAggregate(ArtifactId aggregateRoot, EventSourceId eventSource, ExecutionContext context, CancellationToken token)
    {
        Log.FetchEventsForAggregate(_logger);

        var fetchEvents = await TryPerformInExecutionContext(
                context, 
                (eventStore, _) => eventStore.FetchForAggregate(eventSource, aggregateRoot, token))
            .ConfigureAwait(false);

        if (fetchEvents.Success)
        {
            Log.SuccessfullyFetchedEventsForAggregate(_logger);
        }
        else
        {
            Log.ErrorFetchingEventsFromAggregate(_logger, fetchEvents.Exception);
        }

        return fetchEvents;
    }

    async Task<Try<T>> TryPerformInExecutionContext<T>(ExecutionContext requested, Func<IEventStore, ExecutionContext, Task<T>> action)
    {
        // TODO: This isn't really all that nice, can we move something out another place or make an extension method?
        var createExecutionContext = _executionContextCreator.TryCreateUsing(requested);
        if (!createExecutionContext.Success)
        {
            return createExecutionContext.Exception;
        }

        var executionContext = createExecutionContext.Result;

        try
        {
            var eventStore = _getEventStore(executionContext.Tenant);
            return await action(eventStore, executionContext).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            return exception;
        }
    }
}
