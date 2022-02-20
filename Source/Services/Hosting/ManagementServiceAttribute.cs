// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Configuration.ConfigurationObjects.Endpoints;

namespace Dolittle.Runtime.Services.Hosting;

/// <summary>
/// Indicates that the class should be registered as a management gRPC service in a DI container.
/// </summary>
public class ManagementServiceAttribute : ServiceAttribute 
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ManagementServiceAttribute"/> class.
    /// </summary>
    public ManagementServiceAttribute()
        : base(EndpointVisibility.Management)
    {
    }
}
