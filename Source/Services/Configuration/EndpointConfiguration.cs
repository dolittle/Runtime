// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Services.Configuration;

/// <summary>
/// Represents the configuration of a gRPC service endpoint.
/// </summary>
public record EndpointConfiguration
{
    /// <summary>
    /// Gets a value indicating whether or not the endpoint should be enabled.
    /// </summary>
    public bool Enabled { get; init; } = true;

    /// <summary>
    /// Gets the port to serve the endpoint on.
    /// </summary>
    public int Port { get; init; }
}
