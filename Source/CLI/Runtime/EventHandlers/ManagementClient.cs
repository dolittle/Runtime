// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.CLI.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Processing.Management.Contracts;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Microservices;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Rudimentary;
using ManagementContracts = Dolittle.Runtime.Events.Processing.Management.Contracts;
using static Dolittle.Runtime.Events.Processing.Management.Contracts.EventHandlers;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers;

/// <summary>
/// Represents an implementation of <see cref="IManagementClient"/>.
/// </summary>
public class ManagementClient : IManagementClient
{
    readonly ICanCreateClients _clients;
    readonly IConvertStreamProcessorStatus _converter;

    /// <summary>
    /// Initializes a new instance of the <see cref="ManagementClient"/> class.
    /// </summary>
    /// <param name="clients">The client creator to us to create clients that connect to the Runtime.</param>
    /// <param name="converter">The converter to use to convert stream processor statuses.</param>
    public ManagementClient(ICanCreateClients clients, IConvertStreamProcessorStatus converter)
    {
        _clients = clients;
        _converter = converter;
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

    /// <inheritdoc />
    public async Task<Try<EventHandlerStatus>> Get(MicroserviceAddress runtime, EventHandlerId eventHandler, TenantId tenant = null)
    {
        var client = _clients.CreateClientFor<EventHandlersClient>(runtime);
        var request = new GetOneRequest
        {
            EventHandlerId = eventHandler.EventHandler.ToProtobuf(),
            ScopeId = eventHandler.Scope.ToProtobuf(),
            TenantId = tenant?.ToProtobuf()
        };

        var response = await client.GetOneAsync(request);
        if (response.Failure != null)
        {
            throw new GetOneEventHandlerFailed(eventHandler, response.Failure.Reason);
        }

        return CreateEventHandlerStatus(response.EventHandlers);
    }

    EventHandlerStatus CreateEventHandlerStatus(ManagementContracts.EventHandlerStatus status)
        => new(
            new EventHandlerId(status.ScopeId.ToGuid(), status.EventHandlerId.ToGuid()),
            status.EventTypes.Select(_ => new Artifact(_.Id.ToGuid(), _.Generation)),
            status.Partitioned,
            status.Alias,
            _converter.Convert(status.Tenants));
}
