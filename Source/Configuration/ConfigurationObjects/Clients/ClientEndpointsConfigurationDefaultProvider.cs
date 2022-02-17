// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Configuration.ConfigurationObjects.Endpoints;

namespace Dolittle.Runtime.Configuration.ConfigurationObjects.Clients;

/// <summary>
/// Represents the default configuration for <see cref="ClientEndpointsConfiguration"/>.
/// </summary>
public class ClientEndpointsConfigurationDefaultProvider : ICanProvideDefaultConfigurationFor<ClientEndpointsConfiguration>
{
    /// <inheritdoc/>
    public ClientEndpointsConfiguration Provide()
        => new()
        {
            [EndpointVisibility.Public] = new ClientEndpointConfiguration("localhost", EndpointsConfigurationDefaultProvider.DefaultPublicPort),
            [EndpointVisibility.Private] = new ClientEndpointConfiguration("localhost", EndpointsConfigurationDefaultProvider.DefaultPrivatePort),
            [EndpointVisibility.Management] = new ClientEndpointConfiguration("localhost", EndpointsConfigurationDefaultProvider.DefaultManagementPort)
        };
}
