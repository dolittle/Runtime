// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Configuration.Management.Contracts;
using Microservices;
using static Dolittle.Runtime.Configuration.Management.Contracts.Configuration;

namespace Dolittle.Runtime.CLI.Runtime.Configuration;

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
    public async Task<string> GetConfigurationYaml(MicroserviceAddress runtime)
    {
        try
        {
            var client = _clients.CreateClientFor<ConfigurationClient>(runtime);
            var request = new GetConfigurationYamlRequest();
            var response = await client.GetConfigurationYamlAsync(request);
            return response.Value;
        }
        catch (Exception ex)
        {
            throw new CommandFailed(ex.ToString());
        }
    }
}
