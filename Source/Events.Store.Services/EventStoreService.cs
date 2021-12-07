// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;
using DolittleExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using IExecutionContextManager = Dolittle.Runtime.Execution.IExecutionContextManager;

namespace Dolittle.Runtime.Events.Store.Services;

/// <summary>
/// Represents the implementation of <see cref="IEventStoreService" />.
/// </summary>
[Singleton]
public class EventStoreService : IEventStoreService
{
    readonly FactoryFor<IEventStore> _eventStoreFactory;
    readonly IExecutionContextManager _executionContextManager;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreService"/> class.
    /// </summary>
    /// <param name="eventStoreFactory"><see cref="IEventStore"/>.</param>
    /// <param name="executionContextManager"><see cref="IExecutionContextManager" />.</param>
    /// <param name="logger"><see cref="ILogger"/> for logging.</param>
    public EventStoreService(
        FactoryFor<IEventStore> eventStoreFactory,
        IExecutionContextManager executionContextManager,
        ILogger logger)
    {
        _eventStoreFactory = eventStoreFactory;
        _executionContextManager = executionContextManager;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Try<CommittedEvents>> TryCommit(UncommittedEvents events, DolittleExecutionContext context, CancellationToken token)
    {
        try
        {
            _executionContextManager.CurrentFor(context);
            _logger.EventsReceivedForCommitting(false, events.Count);
            var committedEvents = await _eventStoreFactory().CommitEvents(events, token).ConfigureAwait(false);
            Log.EventsSuccessfullyCommitted(_logger);
            return committedEvents;
        }
        catch (Exception ex)
        {
            Log.ErrorCommittingEvents(_logger, ex);
            return ex;
        }
    }

    /// <inheritdoc/>
    public async Task<Try<CommittedAggregateEvents>> TryCommitForAggregate(UncommittedAggregateEvents events, DolittleExecutionContext context, CancellationToken token)
    {
        try
        {
            _executionContextManager.CurrentFor(context);
            _logger.EventsReceivedForCommitting(true, events.Count);
            var committedEvents = await _eventStoreFactory().CommitAggregateEvents(events, token).ConfigureAwait(false);
            Log.AggregateEventsSuccessfullyCommitted(_logger);
            return committedEvents;
        }
        catch (Exception ex)
        {
            Log.ErrorCommittingAggregateEvents(_logger, ex);
            return ex;
        }
    }

    /// <inheritdoc/>
    public async Task<Try<CommittedAggregateEvents>> TryFetchForAggregate(ArtifactId aggregateRoot, EventSourceId eventSource, DolittleExecutionContext context, CancellationToken token)
    {
        try
        {
            Log.FetchEventsForAggregate(_logger);
            _executionContextManager.CurrentFor(context);
            var committedEvents = await _eventStoreFactory().FetchForAggregate(eventSource, aggregateRoot, token).ConfigureAwait(false);
            Log.SuccessfullyFetchedEventsForAggregate(_logger);
            return committedEvents;
        }
        catch (Exception ex)
        {
            Log.ErrorFetchingEventsFromAggregate(_logger, ex);
            return ex;
        }
    }
}
