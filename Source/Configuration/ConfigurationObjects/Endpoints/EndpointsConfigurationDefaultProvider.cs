// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Configuration.ConfigurationObjects.Endpoints;

/// <summary>
/// Represents a <see cref="ICanProvideDefaultConfigurationFor{TConfiguration}">default provider</see> for <see cref="EndpointsConfiguration"/>.
/// </summary>
public class EndpointsConfigurationDefaultProvider : ICanProvideDefaultConfigurationFor<EndpointsConfiguration>
{
    /// <summary>
    /// The default public port.
    /// </summary>
    public const int DefaultPublicPort = 50052;

    /// <summary>
    /// The default private port.
    /// </summary>
    public const int DefaultPrivatePort = 50053;
        
    /// <summary>
    /// The default management port.
    /// </summary>
    public const int DefaultManagementPort = 51052;

    /// <inheritdoc/>
    public EndpointsConfiguration Provide()
        => new()
        {
            [EndpointVisibility.Public] = new EndpointConfiguration { Port = DefaultPublicPort },
            [EndpointVisibility.Private] = new EndpointConfiguration { Port = DefaultPrivatePort },
            [EndpointVisibility.Management] = new EndpointConfiguration { Port = DefaultManagementPort }
        };
}
