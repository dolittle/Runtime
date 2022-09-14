// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Aggregates.Management.Contracts;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Rudimentary.AsyncEnumerators;
using Dolittle.Runtime.Services.Hosting;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using static Dolittle.Runtime.Aggregates.Management.Contracts.AggregateRoots;

namespace Dolittle.Runtime.Aggregates.Management;

/// <summary>
/// Represents an implementation of <see cref="AggregateRootsBase"/>.
/// </summary>
[ManagementService]
public class AggregateRootsService : AggregateRootsBase
{
    readonly IGetTenantScopedAggregateRoot _tenantScopedAggregateRoot;
    readonly Func<TenantId, IFetchCommittedEvents> _getCommittedEventsFetcher;
    readonly ILogger _logger;
        
    public AggregateRootsService(
        IGetTenantScopedAggregateRoot tenantScopedAggregateRoot,
        Func<TenantId, IFetchCommittedEvents> getCommittedEventsFetcher,
        ILogger logger)
    {
        _tenantScopedAggregateRoot = tenantScopedAggregateRoot;
        _getCommittedEventsFetcher = getCommittedEventsFetcher;
        _logger = logger;
    }

    /// <inheritdoc />
    public override async Task<GetAllResponse> GetAll(GetAllRequest request, ServerCallContext context)
    {
        try
        {
            _logger.GetAllAggregateRoots();
            var response = new GetAllResponse();

            var tenant = request.TenantId?.ToGuid();
            var aggregatesRoots = tenant is null
                ? await _tenantScopedAggregateRoot.GetAllAggregateRootsForAllTenants().ConfigureAwait(false)
                : await _tenantScopedAggregateRoot.GetAllAggregateRootsFor(tenant).ConfigureAwait(false);
            response.AggregateRoots.AddRange(aggregatesRoots.Select(ToProtobuf));

            return response;
        }
        catch (Exception ex)
        {
            _logger.Failure(ex);
            return new GetAllResponse { Failure = ex.ToProtobuf() };
        }
    }

    /// <inheritdoc />
    public override async Task<GetOneResponse> GetOne(GetOneRequest request, ServerCallContext context)
    {
        try
        {
            _logger.GetOneAggregateRoot(request.AggregateRootId.ToGuid());
            var tenant = request.TenantId?.ToGuid();
            var aggregatesRoot = tenant is null
                ? await _tenantScopedAggregateRoot.GetAggregateRootForAllTenants(request.AggregateRootId.ToGuid()).ConfigureAwait(false)
                : await _tenantScopedAggregateRoot.GetAggregateRootFor(tenant, request.AggregateRootId.ToGuid()).ConfigureAwait(false);
            return new GetOneResponse { AggregateRoot = ToProtobuf(aggregatesRoot) };
        }
        catch (Exception ex)
        {
            _logger.Failure(ex);
            return new GetOneResponse { Failure = ex.ToProtobuf() };
        }
    }

    /// <inheritdoc />
    public override async Task<GetEventsResponse> GetEvents(GetEventsRequest request, ServerCallContext context)
    {
        try
        {
            var aggregateRootId = request.Aggregate.AggregateRootId.ToGuid();
            var eventSourceId = request.Aggregate.EventSourceId;
            var tenant = request.TenantId.ToGuid();
            
            _logger.GetEvents(aggregateRootId, eventSourceId);

            var singleBatch = await _getCommittedEventsFetcher(tenant)
                .FetchStreamForAggregate(eventSourceId, aggregateRootId, context.CancellationToken)
                .Select(_ => _.ToProtobuf())
                .BatchReduceMessagesOfSize(
                    (first, next) =>
                    {
                        first.Events.AddRange(next.Events);
                        return first;
                    },
                    uint.MaxValue,
                    context.CancellationToken)
                .SingleAsync(context.CancellationToken);
            
            return new GetEventsResponse { Events = singleBatch };
        }
        catch (Exception ex)
        {
            _logger.Failure(ex);
            return new GetEventsResponse { Failure = ex.ToProtobuf() };
        }
    }

    static Contracts.AggregateRoot ToProtobuf(AggregateRootWithTenantScopedInstances aggregateRoot)
    {
        var result = new Contracts.AggregateRoot
        {
            Alias = aggregateRoot.AggregateRoot.Alias,
            AggregateRoot_ = aggregateRoot.AggregateRoot.Identifier.ToProtobuf(),
        };
        result.EventSources.AddRange(aggregateRoot.Aggregates.Select(_ => new TenantScopedEventSource
        {
            TenantId = _.Tenant.ToProtobuf(),
            AggregateRootVersion = _.Instance.Version,
            EventSourceId = _.Instance.EventSource
        }));
        return result;
    }
}
