// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Events.Store.Actors;
using Dolittle.Runtime.Protobuf;
using Dolittle.Services.Contracts;
using Microsoft.Extensions.Logging;
using Proto;

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// Represents an implementation of <see cref="IEventStore"/> that uses the <see cref="EventStoreActor"/> to commit events.
/// </summary>
public class EventStore : IEventStore
{
    readonly Func<TenantId, EventStoreClient> _getEventStoreClient;
    readonly Func<TenantId, IFetchCommittedEvents> _getCommittedEventsFetcher;
    readonly IRootContext _rootContext;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStore"/> class.
    /// </summary>
    /// <param name="getEventStoreClient">The factory to use to get the <see cref="EventStoreActor"/> client.</param>
    /// <param name="logger">The logger to use for logging.</param>
    /// <param name="getCommittedEventsFetcher"></param>
    /// <param name="rootContext">Proto root context. Allows middleware to be used</param>
    public EventStore(Func<TenantId, EventStoreClient> getEventStoreClient, ILogger logger, Func<TenantId, IFetchCommittedEvents> getCommittedEventsFetcher, IRootContext rootContext)
    {
        _getEventStoreClient = getEventStoreClient;
        _logger = logger;
        _getCommittedEventsFetcher = getCommittedEventsFetcher;
        _rootContext = rootContext;
    }

    /// <inheritdoc />
    public Task<CommitEventsResponse> CommitEvents(CommitEventsRequest request, CancellationToken cancellationToken)
    {
        //_logger.EventsReceivedForCommitting(false, events.Count);
        //Log.EventsSuccessfullyCommitted(_logger))
        //Log.ErrorCommittingEvents(_logger, exception));
        return _getEventStoreClient(GetTenantFrom(request.CallContext))
            .Commit(request, _rootContext, cancellationToken);
    }

    /// <inheritdoc />
    public Task<CommitAggregateEventsResponse> CommitAggregateEvents(CommitAggregateEventsRequest request, CancellationToken cancellationToken)
    {
        //_logger.EventsReceivedForCommitting(true, events.Count);
        //Log.AggregateEventsSuccessfullyCommitted(_logger))
        //Log.ErrorCommittingAggregateEvents(_logger, exception));
        return _getEventStoreClient(GetTenantFrom(request.CallContext))
            .CommitForAggregate(request, _rootContext, cancellationToken);
    }
    
    /// <inheritdoc />
    public async Task<FetchForAggregateResponse> FetchAggregateEvents(FetchForAggregateRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var events = await _getCommittedEventsFetcher(GetTenantFrom(request.CallContext))
                .FetchForAggregate(request.Aggregate.EventSourceId, request.Aggregate.AggregateRootId.ToGuid(), cancellationToken);

            return new FetchForAggregateResponse
            {
                Events = events.ToProtobuf()
            };
        }
        catch (Exception e)
        {
            Log.ErrorFetchingEventsFromAggregate(_logger,e);
            return new FetchForAggregateResponse
            {
                Failure = e.ToFailure()
            };
        }
    }

    static TenantId GetTenantFrom(CallRequestContext context)
        => context.ExecutionContext.TenantId.ToGuid();
}
