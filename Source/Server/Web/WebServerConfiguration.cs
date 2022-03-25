// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Configuration;

namespace Dolittle.Runtime.Server.Web;

/// <summary>
/// Represents the configuration of a web rest service endpoint.
/// </summary>
[Configuration("endpoints:web")]
public record WebServerConfiguration 
{
    /// <summary>
    /// Gets a value indicating whether or not the endpoint should be enabled.
    /// </summary>
    public bool Enabled { get; init; } = true; // TODO: It would be cool if this made it so that the scoped server host didn't start

    /// <summary>
    /// Gets the port to serve the endpoint on.
    /// </summary>
    public int Port { get; init; } = 8001;
}
