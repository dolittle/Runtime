// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Aggregates.Management.Contracts;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using static Dolittle.Runtime.Aggregates.Management.Contracts.AggregateRoots;
using GetAllRequest = Dolittle.Runtime.Aggregates.Management.Contracts.GetAllRequest;
using GetAllResponse = Dolittle.Runtime.Aggregates.Management.Contracts.GetAllResponse;
using GetOneRequest = Dolittle.Runtime.Aggregates.Management.Contracts.GetOneRequest;
using GetOneResponse = Dolittle.Runtime.Aggregates.Management.Contracts.GetOneResponse;

namespace Dolittle.Runtime.Aggregates.Management
{
    public class AggregateRootsService : AggregateRootsBase
    {
        readonly IGetTenantScopedAggregateRoot _tenantScopedAggregateRoot;
        readonly FactoryFor<IEventStore> _getEventStore;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger _logger;
        
        public AggregateRootsService(
            IGetTenantScopedAggregateRoot tenantScopedAggregateRoot,
            FactoryFor<IEventStore> getEventStore,
            IExecutionContextManager executionContextManager,
            ILogger logger)
        {
            _tenantScopedAggregateRoot = tenantScopedAggregateRoot;
            _getEventStore = getEventStore;
            _executionContextManager = executionContextManager;
            _logger = logger;
        }

        public override async Task<GetAllResponse> GetAll(GetAllRequest request, ServerCallContext context)
        {
            try
            {
                _logger.GetAllAggregateRoots();
                var response = new GetAllResponse();

                var tenant = request.TenantId?.ToGuid();
                var aggregatesRoots = tenant is null
                    ? await _tenantScopedAggregateRoot.GetForAllTenant().ConfigureAwait(false)
                    : await _tenantScopedAggregateRoot.GetFor(tenant).ConfigureAwait(false);
                response.AggregateRoots.AddRange(aggregatesRoots.Select(ToProtobuf));

                return response;
            }
            catch (Exception ex)
            {
                _logger.Failure(ex);
                return new GetAllResponse { Failure = ex.ToProtobuf() };
            }
        }

        public override async Task<GetOneResponse> GetOne(GetOneRequest request, ServerCallContext context)
        {
            try
            {
                _logger.GetOneAggregateRoot(request.AggregateRootId.ToGuid());
                var tenant = request.TenantId?.ToGuid();
                var aggregatesRoot = tenant is null
                    ? await _tenantScopedAggregateRoot.GetForAllTenant(request.AggregateRootId.ToGuid()).ConfigureAwait(false)
                    : await _tenantScopedAggregateRoot.GetFor(tenant, request.AggregateRootId.ToGuid()).ConfigureAwait(false);
                return new GetOneResponse { AggregateRoot = ToProtobuf(aggregatesRoot) };
            }
            catch (Exception ex)
            {
                _logger.Failure(ex);
                return new GetOneResponse { Failure = ex.ToProtobuf() };
            }
        }

        public override async Task<GetEventsResponse> GetEvents(GetEventsRequest request, ServerCallContext context)
        {
            try
            {
                _logger.GetEvents(request.Aggregate.AggregateRootId.ToGuid(), request.Aggregate.EventSourceId);
                _executionContextManager.CurrentFor(request.TenantId.ToGuid());
                var events = await _getEventStore().FetchForAggregate(request.Aggregate.EventSourceId, request.Aggregate.AggregateRootId.ToGuid(), context.CancellationToken).ConfigureAwait(false);
                return new GetEventsResponse { Events = events.ToProtobuf() };
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
                AggregateRoot_ = aggregateRoot.AggregateRoot.Type.ToProtobuf(),
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
}
