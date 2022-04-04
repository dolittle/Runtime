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

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// Represents an implementation of <see cref="IEventStore"/> that uses the <see cref="EventStoreActor"/> to commit events.
/// </summary>
public class EventStore : IEventStore
{
    readonly Func<TenantId, EventStoreClient> _getEventStoreClient;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStore"/> class.
    /// </summary>
    /// <param name="getEventStoreClient">The factory to use to get the <see cref="EventStoreActor"/> client.</param>
    /// <param name="logger">The logger to use for logging.</param>
    public EventStore(Func<TenantId, EventStoreClient> getEventStoreClient, ILogger logger)
    {
        _getEventStoreClient = getEventStoreClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<CommitEventsResponse> CommitEvents(CommitEventsRequest request, CancellationToken cancellationToken)
    {
        //_logger.EventsReceivedForCommitting(false, events.Count);
        //Log.EventsSuccessfullyCommitted(_logger))
        //Log.ErrorCommittingEvents(_logger, exception));
        return _getEventStoreClient(GetTenantFrom(request.CallContext))
            .Commit(request, cancellationToken);
    }

    /// <inheritdoc />
    public Task<CommitAggregateEventsResponse> CommitAggregateEvents(CommitAggregateEventsRequest request, CancellationToken cancellationToken)
    {
        //_logger.EventsReceivedForCommitting(true, events.Count);
        //Log.AggregateEventsSuccessfullyCommitted(_logger))
        //Log.ErrorCommittingAggregateEvents(_logger, exception));
        return _getEventStoreClient(GetTenantFrom(request.CallContext))
            .CommitForAggregate(request, cancellationToken);
    }

    public Task<FetchForAggregateResponse> FetchAggregateEvents(FetchForAggregateRequest request, CancellationToken cancellationToken)
    {
        //Log.FetchEventsForAggregate(_logger);
        //Log.SuccessfullyFetchedEventsForAggregate(_logger))
        //Log.ErrorFetchingEventsFromAggregate(_logger, exception));
        // TODO: Implement
        throw new System.NotImplementedException();
    }

    static TenantId GetTenantFrom(CallRequestContext context)
        => context.ExecutionContext.TenantId.ToGuid();
}
