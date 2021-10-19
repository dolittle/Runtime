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
using Google.Protobuf.Collections;
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
        public async Task<IEnumerable<EventHandlerStatus>> GetAll(MicroserviceAddress runtime)
        {
            var client = _clients.CreateClientFor<EventHandlersClient>(runtime);
            var request = new GetAllRequest();
            var response = await client.GetAllAsync(request);
            if (response.Failure != null)
            {
                throw new GetAllEventHandlers(response.Failure.Reason);
            }
            return response.EventHandlers.Select(CreateEventHandlerStatus);
        }
        static EventHandlerStatus CreateEventHandlerStatus(Events.Processing.Management.Contracts.EventHandlerStatus status)
            => new(
                new EventHandlerId(status.ScopeId.ToGuid(), status.EventHandlerId.ToGuid()),
                status.EventTypes.Select(_ => new Artifact(_.Id.ToGuid(), _.Generation)),
                status.Partitioned,
                status.Alias,
                GetStates(status));
            static IDictionary<TenantId, IStreamProcessorState> GetStates(Events.Processing.Management.Contracts.EventHandlerStatus status)
            => status.Tenants.ToDictionary(
                _ => new TenantId(_.TenantId.ToGuid()),
                _ =>
            {
                IStreamProcessorState state = status.Partitioned
                    ? CreatePartitionedState(_, _.StreamPosition, _.LastSuccessfullyProcessed.ToDateTimeOffset())
                    : CreateUnpartitionedState(_);
                return state;
            });

        static Events.Processing.Streams.Partitioned.StreamProcessorState CreatePartitionedState(Events.Processing.Management.Contracts.TenantScopedStreamProcessorStatus status, StreamPosition streamPosition, DateTimeOffset lastSuccessfullyProcessed)
            => new(
                streamPosition,
                status.Partitioned.FailingPartitions.ToDictionary(
                    _ => new PartitionId(_.Key),
                    _ => new Events.Processing.Streams.Partitioned.FailingPartitionState(
                        _.Value.StreamPosition,
                        _.Value.RetryTime.ToDateTimeOffset(),
                        _.Value.FailureReason,
                        _.Value.RetryCount,
                        _.Value.LastFailed.ToDateTimeOffset())),
                lastSuccessfullyProcessed);

        static Events.Processing.Streams.StreamProcessorState CreateUnpartitionedState(Events.Processing.Management.Contracts.TenantScopedStreamProcessorStatus status)
            => new(
                status.StreamPosition,
                status.Unpartitioned.FailureReason,
                status.Unpartitioned.RetryTime.ToDateTimeOffset(),
                status.Unpartitioned.RetryCount,
                status.LastSuccessfullyProcessed.ToDateTimeOffset(),
                status.Unpartitioned.IsFailing);
    }
}