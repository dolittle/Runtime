// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Configuration;

namespace Dolittle.Runtime.Metrics.Configuration;

/// <summary>
/// Represents the configuration of a gRPC service endpoint.
/// </summary>
[Configuration("endpoints:metrics")]
public record MetricsServerConfiguration 
{
    /// <summary>
    /// Gets a value indicating whether or not the endpoint should be enabled.
    /// </summary>
    public bool Enabled { get; init; } = true; // TODO: It would be cool if this made it so that the scoped server host didn't start

    /// <summary>
    /// Gets the port to serve the endpoint on.
    /// </summary>
    public int Port { get; init; } = 9700;
}
