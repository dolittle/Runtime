// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Processing.Management.Contracts;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Microservices;
using Dolittle.Runtime.Protobuf;
using Contracts = Dolittle.Runtime.Events.Processing.Management.Contracts;
using static Dolittle.Runtime.Events.Processing.Management.Contracts.EventHandlers;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers
{
    /// <summary>
    /// Represents an implementation of <see cref="IManagementClient"/>.
    /// </summary>
    public class ManagementClient : IManagementClient
    {
        readonly ICanCreateClients _clients;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementClient"/> class.
        /// </summary>
        /// <param name="clients">The client creator to us to create clients that connect to the Runtime.</param>
        public ManagementClient(ICanCreateClients clients)
        {
            _clients = clients;
        }

        /// <inheritdoc />
        public async Task ReprocessEventsFrom(EventHandlerId eventHandler, TenantId tenant, StreamPosition position, MicroserviceAddress runtime)
        {
            var client = _clients.CreateClientFor<EventHandlersClient>(runtime);

            var request = new ReprocessEventsFromRequest
            {
                ScopeId = eventHandler.Scope.ToProtobuf(),
                EventHandlerId = eventHandler.EventHandler.ToProtobuf(),
                TenantId = tenant.ToProtobuf(),
                StreamPosition = position,
            };

            var response = await client.ReprocessEventsFromAsync(request);
            if (response.Failure != null)
            {
                throw new ReprocessEventsFromFailed(response.Failure.Reason);
            }
        }

        /// <inheritdoc />
        public async Task ReprocessAllEvents(EventHandlerId eventHandler, MicroserviceAddress runtime)
        {
            var client = _clients.CreateClientFor<EventHandlersClient>(runtime);

            var request = new ReprocessAllEventsRequest
            {
                ScopeId = eventHandler.Scope.ToProtobuf(),
                EventHandlerId = eventHandler.EventHandler.ToProtobuf(),
            };

            var response = await client.ReprocessAllEventsAsync(request);
            if (response.Failure != null)
            {
                throw new ReprocessAllEventsFailed(response.Failure.Reason);
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<EventHandlerStatus>> GetAll(MicroserviceAddress runtime, TenantId tenant = null)
        {
            var client = _clients.CreateClientFor<EventHandlersClient>(runtime);
            var request = new GetAllRequest
            {
                TenantId = tenant?.ToProtobuf()
            };

            var response = await client.GetAllAsync(request);
            if (response.Failure != null)
            {
                throw new GetAllEventHandlersFailed(response.Failure.Reason);
            }
            return response.EventHandlers.Select(CreateEventHandlerStatus);
        }
        
        static EventHandlerStatus CreateEventHandlerStatus(Contracts.EventHandlerStatus status)
            => new(
                new EventHandlerId(status.ScopeId.ToGuid(), status.EventHandlerId.ToGuid()),
                status.EventTypes.Select(_ => new Artifact(_.Id.ToGuid(), _.Generation)),
                status.Partitioned,
                status.Alias,
                GetStates(status));

        static IEnumerable<TenantScopedStreamProcessorStatus> GetStates(Contracts.EventHandlerStatus status)
            => status.Tenants.Select(_ => _.StatusCase switch
            {
               Contracts.TenantScopedStreamProcessorStatus.StatusOneofCase.Partitioned => CreatePartitionedState(_, _.Partitioned) as TenantScopedStreamProcessorStatus,
               Contracts.TenantScopedStreamProcessorStatus.StatusOneofCase.Unpartitioned => CreateUnpartitionedState(_, _.Unpartitioned) as TenantScopedStreamProcessorStatus,
               _ => throw new InvalidTenantScopedStreamProcessorStatusTypeReceived(_.StatusCase),
            });

        static PartitionedTenantScopedStreamProcessorStatus CreatePartitionedState(Contracts.TenantScopedStreamProcessorStatus status, Contracts.PartitionedTenantScopedStreamProcessorStatus partitionedStatus)
            => new(
                status.TenantId.ToGuid(),
                status.StreamPosition,
                partitionedStatus.FailingPartitions.Select(_ => new FailingPartition(
                    _.PartitionId,
                    _.StreamPosition,
                    _.FailureReason,
                    _.RetryCount,
                    _.RetryTime.ToDateTimeOffset(),
                    _.LastFailed.ToDateTimeOffset())),
                status.LastSuccessfullyProcessed.ToDateTimeOffset());

        static UnpartitionedTenantScopedStreamProcessorStatus CreateUnpartitionedState(Contracts.TenantScopedStreamProcessorStatus status, Contracts.UnpartitionedTenantScopedStreamProcessorStatus unpartitionedStatus)
            => new(
                status.TenantId.ToGuid(),
                status.StreamPosition,
                unpartitionedStatus.IsFailing,
                unpartitionedStatus.FailureReason,
                unpartitionedStatus.RetryCount,
                unpartitionedStatus.RetryTime.ToDateTimeOffset(),
                status.LastSuccessfullyProcessed.ToDateTimeOffset());
    }
}