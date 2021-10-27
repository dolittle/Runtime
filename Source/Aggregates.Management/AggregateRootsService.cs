// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Aggregates.Management.Contracts;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Tenancy;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using static Dolittle.Runtime.Aggregates.Management.Contracts.AggregateRoots;
using Artifact = Dolittle.Artifacts.Contracts.Artifact;
using AggregateRoot = Dolittle.Runtime.Aggregates.AggregateRoots.AggregateRoot
using GetAllRequest = Dolittle.Runtime.Aggregates.Management.Contracts.GetAllRequest;
using GetAllResponse = Dolittle.Runtime.Aggregates.Management.Contracts.GetAllResponse;
using GetOneRequest = Dolittle.Runtime.Aggregates.Management.Contracts.GetOneRequest;
using GetOneResponse = Dolittle.Runtime.Aggregates.Management.Contracts.GetOneResponse;

namespace Dolittle.Runtime.Aggregates.Management
{
    public class AggregateRootsService : AggregateRootsBase
    {
        readonly FactoryFor<IAggregates> _getAggregates;
        readonly IExecutionContextManager _executionContextManager;
        readonly IPerformActionOnAllTenants _onAllTenants;
        readonly ILogger _logger;
        
        public AggregateRootsService(
            FactoryFor<IAggregates> getAggregates,
            IExecutionContextManager executionContextManager,
            IPerformActionOnAllTenants onAllTenants,
            ILogger logger)
        {
            _getAggregates = getAggregates;
            _executionContextManager = executionContextManager;
            _onAllTenants = onAllTenants;
            _logger = logger;
        }

        public override async Task<GetAllResponse> GetAll(GetAllRequest request, ServerCallContext context)
        {
            _logger.GetAllAggregateRoots();
            // return GetAggregatesInContextOfTenant()
        }

        public override async Task<GetOneResponse> GetOne(GetOneRequest request, ServerCallContext context)
        {
            _logger.GetOneAggregateRoot(request.AggregateRootId.ToGuid());
            var aggregates = await GetAggregatesInContextOfTenant(
                request.TenantId?.ToGuid(),
                aggregates => aggregates.GetFor(new AggregateRoot(new Artifacts.Artifact(request.AggregateRootId.ToGuid(), ArtifactGeneration.First)))).ConfigureAwait(false);

            var response = new GetOneResponse{AggregateRoot = new Contracts.AggregateRoot{}};
            return response;
        }

        public override async Task<GetEventsResponse> GetEvents(GetEventsRequest request, ServerCallContext context)
        {
            _logger.GetEvents(request.Aggregate.AggregateRootId.ToGuid(), request.Aggregate.EventSourceId);
        }

        Task<TResult> GetAggregatesInContextOfTenant<TResult>(TenantId tenant, Func<IAggregates, Task<TResult>> performGetAggregates)
        {
            if (tenant is not null)
            {
                _executionContextManager.CurrentFor(tenant);
                return performGetAggregates(_getAggregates());
            }
            _onAllTenants.PerformAsync()
        }
    }
}
