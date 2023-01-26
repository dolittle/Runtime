// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <param name="getCommittedEventsFetcher">The factory to use to get the <see cref="IFetchCommittedEvents"/> to use for fetching events.</param>
    /// <param name="rootContext">Proto root context. Allows middleware to be used</param>
    /// <param name="logger">The logger to use for logging.</param>
    public EventStore(Func<TenantId, EventStoreClient> getEventStoreClient, Func<TenantId, IFetchCommittedEvents> getCommittedEventsFetcher, IRootContext rootContext, ILogger logger)
    {
        _getEventStoreClient = getEventStoreClient;
        _getCommittedEventsFetcher = getCommittedEventsFetcher;
        _rootContext = rootContext;
        _logger = logger;
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
    public IAsyncEnumerable<FetchForAggregateResponse> FetchAggregateEvents(FetchForAggregateInBatchesRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var fetcher = _getCommittedEventsFetcher(GetTenantFrom(request.CallContext));

            var eventSource = request.Aggregate.EventSourceId;
            var aggregateRoot = request.Aggregate.AggregateRootId.ToGuid();
            
            switch (request.RequestCase)
            {
                case FetchForAggregateInBatchesRequest.RequestOneofCase.FetchAllEvents:
                    return fetcher.FetchStreamForAggregate(eventSource, aggregateRoot, cancellationToken).Select(ConvertAggregateEventsToResponse);
                case FetchForAggregateInBatchesRequest.RequestOneofCase.FetchEvents:
                    var eventTypes = request.FetchEvents.EventTypes.Select(_ => _.ToArtifact().Id);
                    return fetcher.FetchStreamForAggregate(eventSource, aggregateRoot, eventTypes, cancellationToken).Select(ConvertAggregateEventsToResponse);
                case FetchForAggregateInBatchesRequest.RequestOneofCase.None:
                default:
                    return AsyncEnumerable.Repeat(new FetchForAggregateResponse
                    {
                        Failure = new UnsupportedFetchForAggregatesInBatchesType(request.RequestCase).ToFailure(),
                    }, 1);
            }
        }
        catch (Exception e)
        {
            return AsyncEnumerable.Repeat(new FetchForAggregateResponse
            {
                Failure = e.ToFailure(),
            }, 1);
        }
    }

    static FetchForAggregateResponse ConvertAggregateEventsToResponse(CommittedAggregateEvents events)
        => new() { Events = events.ToProtobuf() };

    static TenantId GetTenantFrom(CallRequestContext context)
        => context.ExecutionContext.TenantId.ToGuid();
}
