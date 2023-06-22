// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Configuration;

namespace Dolittle.Runtime.Services.Configuration;

/// <summary>
/// Represents the configuration of endpoints for gRPC services.
/// </summary>
[Configuration("endpoints")]
public record EndpointsConfiguration
{
    /// <summary>
    /// Gets the configuration for the public endpoint.
    /// </summary>
    public EndpointConfiguration Public { get; set; } = new() {Port = 50052};
    
    /// <summary>
    /// Gets the configuration for the private endpoint.
    /// </summary>
    public EndpointConfiguration Private { get; set; } = new() {Port = 50053};
    
    /// <summary>
    /// Gets the configuration for the management endpoint.
    /// </summary>
    public EndpointConfiguration Management { get; set; } = new() {Port = 51052};
    
    /// <summary>
    /// Gets the configuration for the management web endpoint.
    /// </summary>
    public EndpointConfiguration ManagementWeb {get; init; } = new() {Port = 51152};

    /// <summary>
    /// Gets the configuration for an endpoint by its <see cref="EndpointVisibility"/>.
    /// </summary>
    /// <param name="visibility">The visibility to get the configuration for.</param>
    /// <returns>The configuration for the endpoint.</returns>
    public EndpointConfiguration GetConfigurationFor(EndpointVisibility visibility)
        => visibility switch
        {
            EndpointVisibility.Public => Public,
            EndpointVisibility.Private => Private,
            EndpointVisibility.Management => Management,
            EndpointVisibility.ManagementWeb => ManagementWeb,
            _ => throw new UnknownEndpointVisibility(visibility)
        };
}
