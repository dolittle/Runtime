// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Management.Contracts;
using Dolittle.Runtime.Microservices;
using Dolittle.Runtime.Protobuf;
using static Dolittle.Runtime.Events.Management.Contracts.EventTypes;

namespace Dolittle.Runtime.CLI.Runtime.EventTypes;

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
    public async Task<IEnumerable<Dolittle.Runtime.Events.EventType>> GetAll(MicroserviceAddress runtime)
    {
        var client = _clients.CreateClientFor<EventTypesClient>(runtime);
        var request = new GetAllRequest();

        var response = await client.GetAllAsync(request);

        if (response.Failure is not null)
        {
            throw new GetAllFailed(response.Failure.Reason);
        }
        return response.EventTypes.Select(FromProtobuf);
    }

    static Dolittle.Runtime.Events.EventType FromProtobuf(EventType eventType)
        => new(eventType.EventType_.ToArtifact(), eventType.Alias);
}