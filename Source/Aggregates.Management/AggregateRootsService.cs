// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Aggregates.Management.Contracts;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using static Dolittle.Runtime.Aggregates.Management.Contracts.AggregateRoots;

namespace Dolittle.Runtime.Aggregates.Management
{
    /// <summary>
    /// Represents an implementation of <see cref="AggregateRootsBase"/>.
    /// </summary>
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
}